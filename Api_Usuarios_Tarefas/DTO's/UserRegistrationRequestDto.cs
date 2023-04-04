
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class UserRegistrationRequestDto
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
       
        [Required]
        public DateTimeOffset DataDeNascimento { get; set; }
        
        [Required]
        public string Logradouro { get; set; }
    
        [Required]
        public int Numero { get; set; }
            
        [Required]
        public string Cidade { get; set; }
           
        [Required]
        public string Estado { get; set; }
            
        [Required]
        public string Cep { get; set; }
        
        [Required]
        public string Pais { get; set; }

       
    }
