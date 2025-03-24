using System.ComponentModel.DataAnnotations;

namespace GestaoPonto.Models
{
    public class RegistoPonto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do colaborador é obrigatório.")]
        public int? ColaboradorId { get; set; }
        public Colaborador Colaborador { get; set; }

        [Required(ErrorMessage = "A data e hora são obrigatórias.")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório.")]
        [RegularExpression("^(Entrada|Saída)$", ErrorMessage = "O tipo deve ser 'Entrada' ou 'Saída'.")]
        public string Tipo { get; set; } // entrada ou saída
    }
}
