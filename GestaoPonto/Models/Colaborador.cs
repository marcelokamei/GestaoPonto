using System.ComponentModel.DataAnnotations;

namespace GestaoPonto.Models
{
    public class Colaborador
    {
        public int Id { get; set; }
        public string? IdentityUserId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 20 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A função é obrigatória.")]
        public string Role { get; set; }
    }

}
