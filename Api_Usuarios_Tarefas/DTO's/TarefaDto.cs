using Api_Usuarios_Tarefas.Models;

namespace Api_Usuarios_Tarefas.DTO_s
{
    public class TarefaDto
    {
        public string Nome { get; set; }
        public string Descrição { get; set; }
        public Status Status { get; set; }
        public Dificuldade Dificuldade { get; set; }
    }
}
