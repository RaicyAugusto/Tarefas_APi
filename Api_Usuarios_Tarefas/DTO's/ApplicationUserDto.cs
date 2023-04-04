    namespace Api_Usuarios_Tarefas.DTO_s
{
    public class ApplicationUserDto
    {
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
