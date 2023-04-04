using Api_Usuarios_Tarefas.Models;

namespace Api_Usuarios_Tarefas.Repository
{
    public interface ITarefasRepository
    { 
        ICollection<Tarefa> GetTarefasByUserId(string usuarioId); 
        Tarefa GetTarefaId(int tarefaId);
        bool TarefaAdd(Tarefa tarefa);
        bool TarefasUpdate(Tarefa tarefa);
        bool DeleteTarefa(Tarefa tarefa);
        bool TarefaExiste(int tarefaId); 
        bool Save();
    }
}
