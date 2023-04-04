using Api_Usuarios_Tarefas.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Api_Usuarios_Tarefas.Repository;
using Api_Usuarios_Tarefas.Models;
using Api_Usuarios_Tarefas.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api_Usuarios_Tarefas.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController] 
    public class TarefaController : ControllerBase
    {
        private readonly ITarefasRepository _tarefasRepository;

        public TarefaController(ITarefasRepository tarefasRepository)
        {
            _tarefasRepository = tarefasRepository;
        }

        [HttpGet("minhas_tarefas")] 
        [ProducesResponseType(typeof(IEnumerable<TarefaDto>), 200)] 
        [ProducesResponseType(401)] 
        public IActionResult MinhasTarefas()
        {
            // Obtém o ID do usuário a partir do token de autenticação
            var usuarioId = JwtUtils.GetUserIdFromToken(Request);

            // Obtém as tarefas associadas ao usuário
            var tarefas = _tarefasRepository.GetTarefasByUserId(usuarioId); 

            // Converte as tarefas para objetos TarefaDto e as adiciona a uma lista de objetos TarefaDto
            var tarefasDto = tarefas.Select(tarefa => new TarefaDto
            {
                Nome = tarefa.Nome,
                Descrição = tarefa.Descrição,
                Status = tarefa.Status,
                Dificuldade = tarefa.Dificuldade
            }).ToList();

            // Retorna a lista de tarefas como um objeto JSON na resposta HTTP
            return Ok(tarefasDto); 
        }



        [HttpGet("{tarefaId}")]
        [ProducesResponseType(typeof(TarefaDto), 200)] 
        [ProducesResponseType(404)]
        public IActionResult Tarefa(int tarefaId)
        {
            // Obtém o id do usuário a partir do token JWT
            var usuarioId = JwtUtils.GetUserIdFromToken(Request);

            // Verifica se a tarefa existe
            if (!_tarefasRepository.TarefaExiste(tarefaId))
            {
                return NotFound(); // Retorna 404 caso a tarefa não exista
            }

            // Obtém a tarefa pelo id
            var tarefa = _tarefasRepository.GetTarefaId(tarefaId);


            // Converte a tarefa em um DTO para enviar como resposta
            var tarefaDto = new TarefaDto
            {
                Nome = tarefa.Nome,
                Descrição = tarefa.Descrição,
                Status = tarefa.Status,
                Dificuldade = tarefa.Dificuldade,
            };

            return Ok(tarefaDto); // Retorna a tarefa como DTO em caso de sucesso
        }



        [HttpPost("nova_tarefa")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTarefa([FromBody] TarefaDto tarefaDto)
        {

            // Obtém o id do usuário a partir do token JWT
            var usuarioId = JwtUtils.GetUserIdFromToken(Request);

            // validar as informações da tarefa
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(tarefaDto.Nome))
                {
                    return BadRequest("O nome da tarefa não pode ser vazio.");
                }

                if (!Enum.IsDefined(typeof(Dificuldade), tarefaDto.Dificuldade))
                {
                    return BadRequest("A dificuldade da tarefa deve estar entre 1.facil, 2.medio ou 3.dificil.");
                }

                if (!Enum.IsDefined(typeof(Status), tarefaDto.Status))
                {
                    return BadRequest(
                        "O Status da tarefa deve estar entre 1.Não Iniciado, 2.Em Progresso ou 3.Concluido");
                }
            }

            // novo objeto Tarefa a partir do DTO
            var tarefa = new Tarefa
            {
                Nome = tarefaDto.Nome,
                Descrição = tarefaDto.Descrição,
                Status = tarefaDto.Status,
                Dificuldade = tarefaDto.Dificuldade,
                UsuarioId = usuarioId
            };

            // usar o repositório para salvar a nova tarefa no banco de dados
            var use = _tarefasRepository.TarefaAdd(tarefa);
           

            // retornar um código HTTP 204 em caso de sucesso na criação da tarefa
            return NoContent();
        }




        [HttpPut("{tarefaId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTarefa(int tarefaId, [FromBody] TarefaDto updateTarefa)
        {
            // Obtém o id do usuário a partir do token JWT
            var usuarioId = JwtUtils.GetUserIdFromToken(Request);

            if (usuarioId == null)
            {
                return BadRequest("usuario não existe");
            }

            // verificar se a tarefa existe
            var tarefa = _tarefasRepository.GetTarefaId(tarefaId);

            if (tarefa == null)
            {
                return NotFound();
            }

            // verificar se o usuário tem permissão para atualizar a tarefa
            if (tarefa.UsuarioId != usuarioId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                // validar as informações da tarefa
                if (string.IsNullOrWhiteSpace(updateTarefa.Nome))
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "O nome da tarefa não pode ser vazio."
                        }
                    });
                }

                if (!Enum.IsDefined(typeof(Dificuldade), updateTarefa.Dificuldade))
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "A dificuldade da tarefa deve estar entre 1.facil, 2.medio ou 3.dificil."
                        }
                    });
                }

                if (!Enum.IsDefined(typeof(Status), updateTarefa.Status))
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "O Status da tarefa deve estar entre 1.Não Iniciado, 2.Em Progresso ou 3.Concluido"
                        }
                    });

                }
            }


            // atualizar a tarefa com as informações do DTO
            tarefa.Nome = updateTarefa.Nome;
            tarefa.Descrição = updateTarefa.Descrição;
            tarefa.Status = updateTarefa.Status;
            tarefa.Dificuldade = updateTarefa.Dificuldade;

            // usar o repositório para atualizar a tarefa no banco de dados
            _tarefasRepository.TarefasUpdate(tarefa);

            return NoContent();


        }

        [HttpDelete("{tarefaId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTarefa(int tarefaId)
        {
            // Obtém a tarefa a ser excluída do repositório com base no ID fornecido
            var tarefa_delete = _tarefasRepository.GetTarefaId(tarefaId);

            // Verifica se a tarefa existe no banco de dados. Se não existir, retorna um código HTTP 404 - Not Found.
            if (tarefa_delete == null)
            {
                return NotFound();
            }

            // Exclui a tarefa do banco de dados e verifica se a exclusão foi bem-sucedida
            var tarefaDelete = _tarefasRepository.DeleteTarefa(tarefa_delete);

            // Se a exclusão não foi bem-sucedida, retorna um código HTTP 500 - Internal Server Error
            if (!tarefaDelete)
            {
                return BadRequest();
            }

            // Retorna um código HTTP 204 - No Content em caso de sucesso na exclusão da tarefa.
            return NoContent();
        }

    }
}



    