using Api_Usuarios_Tarefas.DTO_s;
using Api_Usuarios_Tarefas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api_Usuarios_Tarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUsuario(string Id, [FromBody] ApplicationUserDto userDto)
        {
            // Verifica se o modelo enviado na requisição é válido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Busca o usuário pelo Id passado como parâmetro
            var user = await _userManager.FindByIdAsync(Id);

            // Se o usuário não for encontrado, retorna o status 404 (Not Found)
            if (user == null)
            {
                return NotFound();
            }

            // Atualiza os dados do usuário com base no DTO enviado no corpo da requisição
            user.Id = Id;
            user.Nome = userDto.Nome;
            user.DataDeNascimento = userDto.DataDeNascimento;
            user.Logradouro = userDto.Logradouro;
            user.Numero = userDto.Numero;
            user.Cidade = userDto.Cidade;
            user.Estado = userDto.Estado;
            user.Cep = userDto.Cep;
            user.Pais = userDto.Pais;

            // Realiza a atualização do usuário no banco de dados
            var user_update = await _userManager.UpdateAsync(user);

            // Verifica se a atualização foi bem sucedida. Se sim, retorna o status 204 (No Content)
            if (user_update.Succeeded)
            {
                return NoContent();
            }

            // Se a atualização não foi bem sucedida, retorna um BadRequest com a mensagem de erro
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Usuario não atualizado"
                },
                Result = false
            });
        }



        [HttpDelete]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletarUsuario()
        {
            // Verifica se o ModelState é válido
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Obtém o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtém o usuário pelo ID
            var user = await _userManager.FindByIdAsync(userId);

            // Se o usuário não existir, retorna NotFound (404)
            if (user == null)
            {
                return NotFound();
            }

            // Remove o usuário
            var result = await _userManager.DeleteAsync(user);

            // Se não for possível remover o usuário, retorna BadRequest (400)
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Retorna uma resposta de sucesso sem conteúdo (204)
            return NoContent();
        }

    }
}
