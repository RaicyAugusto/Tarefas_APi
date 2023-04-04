using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Usuarios_Tarefas.Models
{
    public class Tarefa
    {
        [Key]
        public int  Id { get; set; }
        public string Nome { get; set; }
        public string Descrição { get; set; }
        public Status Status  { get; set; }
        public Dificuldade Dificuldade { get; set; }

        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
    }
    

    public enum Status
    {
        Nao_Iniciado,
        Em_Progresso,
        Concluido
    }

    public enum Dificuldade
    {
        
        Facil,
        Medio,
        Dificil
    }
}
