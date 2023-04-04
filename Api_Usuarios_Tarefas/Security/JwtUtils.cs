using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api_Usuarios_Tarefas.Security
{
    public class JwtUtils
    {
        // Método estático para obter o ID do usuário a partir do token de autorização na requisição HTTP
        public static string GetUserIdFromToken(HttpRequest request)
        {
            // Obtém o cabeçalho de autorização da requisição
            string authorizationHeader = request.Headers["Authorization"].FirstOrDefault();

            // Verifica se o cabeçalho está vazio
            if (authorizationHeader == null)
            {
                // Retorna nulo se o cabeçalho estiver vazio
                return null;
            }

            // Divide o cabeçalho em duas partes (Bearer e o token)
            string token = authorizationHeader.Split(' ')[1];

            // Cria um manipulador de tokens JWT
            var handler = new JwtSecurityTokenHandler();

            // Lê o token JWT
            var jwtToken = handler.ReadJwtToken(token);

            // Obtém o valor do ID do usuário a partir do token
            string userId = jwtToken.Claims.First(claim => claim.Type == "Id").Value;

            // Retorna o ID do usuário obtido
            return userId;
        }


    }
}
