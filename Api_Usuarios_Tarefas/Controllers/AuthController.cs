using Api_Usuarios_Tarefas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api_Usuarios_Tarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public AuthController(UserManager<ApplicationUser> userManager, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;

        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            // Verifica se o modelo de requisição é válido
            if (ModelState.IsValid)
            {
                // Verifica se o email já existe no sistema
                var user_existe = await _userManager.FindByEmailAsync(requestDto.Email);

                if (user_existe != null)
                {
                    // Se o email já existir, retorna uma resposta de erro
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                {
                    "Email, já existe"
                }
                    });
                }

                // Cria um novo usuário com os dados fornecidos na requisição
                var novo_usuario = new ApplicationUser()
                {
                    Nome = requestDto.Nome,
                    UserName = requestDto.Email,
                    Email = requestDto.Email,
                    DataDeNascimento = requestDto.DataDeNascimento,
                    Logradouro = requestDto.Logradouro,
                    Numero = requestDto.Numero,
                    Cidade = requestDto.Cidade,
                    Estado = requestDto.Estado,
                    Cep = requestDto.Cep,
                    Pais = requestDto.Pais,
                };

                // Tenta criar o usuário com a senha fornecida
                var criar = await _userManager.CreateAsync(novo_usuario, requestDto.Password);

                if (criar.Succeeded)
                {
                    // Se o usuário foi criado com sucesso, gera um token JWT para autenticação
                    var token = GenerateJwtToken(novo_usuario);

                    // Retorna uma resposta de sucesso com o token
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }

                // Se não foi possível criar o usuário, retorna uma resposta de erro
                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
            {
                "Server error"
            },
                    Result = false
                });
            }

            // Se o modelo de requisição for inválido, retorna uma resposta de erro
            return BadRequest();
        }


        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                // Procura um usuário com o email informado
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);

                // Se não encontrar, retorna um BadRequest com mensagem de erro
                if (existing_user == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Credenciais inválidas"
                        },
                        Result = false
                    });
                }

                // Verifica se a senha informada está correta para o usuário encontrado
                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);

                // Se a senha estiver incorreta, retorna um BadRequest com mensagem de erro
                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Credenciais inválidas"
                        },
                        Result = false
                    });
                }

                // Se o email e a senha estiverem corretos, gera um token JWT e retorna um Ok com o token gerado
                var jwtToken = GenerateJwtToken(existing_user);

                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            // Se o ModelState não for válido, retorna um BadRequest com mensagem de erro
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Credenciais inválidas"
                },
                Result = false
            });
        }




        private string GenerateJwtToken(ApplicationUser user)
        {

            // Cria um objeto JwtSecurityTokenHandler para manipular tokens JWT
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Obtém a chave secreta para assinatura JWT a partir do arquivo de configuração
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            // Define os dados do token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                // Define as informações que estarão armazenadas no token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",  user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, value: user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                // Define o tempo de expiração do token
                Expires = DateTime.UtcNow.AddHours(1),

                // Define a chave de assinatura e o algoritmo de assinatura
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Cria um novo token JWT com base nas informações definidas acima
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            // Retorna o token JWT como uma string
            return jwtTokenHandler.WriteToken(token);
        }

    }
}


