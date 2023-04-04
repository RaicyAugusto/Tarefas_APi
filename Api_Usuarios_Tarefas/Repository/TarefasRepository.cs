using System.Security.Claims;
using Api_Usuarios_Tarefas.Data;
using Api_Usuarios_Tarefas.Models;
using Api_Usuarios_Tarefas.Security;


namespace Api_Usuarios_Tarefas.Repository   
{
    public class TarefasRepository:ITarefasRepository
    {
        private readonly DataContext _context;

        public TarefasRepository(DataContext context)
        {
            _context = context;
        }

        public  ICollection<Tarefa> GetTarefasByUserId(string usuarioId)
        {
            return _context.Tarefas.ToList();
        }
            
        public Tarefa GetTarefaId(int tarefaId)
        { 
            return  _context.Tarefas.Where(x=>x.Id == tarefaId).FirstOrDefault();
        }

        public bool TarefaAdd(Tarefa tarefa)
        {
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return true;
        }

        public bool TarefasUpdate(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
             Save();
             return true;
        }

        public bool DeleteTarefa(Tarefa tarefa)
        {
            _context.Tarefas.Remove(tarefa); 
            Save();
            return true;
        }

        public  bool Save()
        {
            var save =  _context.SaveChanges();
            return save > 0;
        }

        public bool TarefaExiste(int tarefaId)
        {
            return _context.Tarefas.Any(t => t.Id == tarefaId);
        }
    }
}
    