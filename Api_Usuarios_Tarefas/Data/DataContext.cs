using Api_Usuarios_Tarefas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api_Usuarios_Tarefas.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {   
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
        
        public DbSet<Tarefa> Tarefas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tarefa>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Tarefa>()
                .HasOne(u => u.Usuario)
                .WithMany(p => p.Tarefas)
                .HasForeignKey(u => u.UsuarioId)
                .IsRequired();

        }
    }
}
