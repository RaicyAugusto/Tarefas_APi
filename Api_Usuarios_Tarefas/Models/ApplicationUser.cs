using Microsoft.AspNetCore.Identity;

namespace Api_Usuarios_Tarefas.Models
{
    public class ApplicationUser : IdentityUser
    {
        public  virtual ICollection<Tarefa> Tarefas { get; set; }
        public string Nome { get; set; } 
        public DateTimeOffset DataDeNascimento { get; set; } 
        public string Logradouro { get; set; } 
        public int Numero { get; set; } 
        public string Cidade { get; set; } 
        public string Estado { get; set; } 
        public string Cep { get; set; } 
        public string Pais { get; set; }
    }
}
