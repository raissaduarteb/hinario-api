using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hinario.Domain.Models
{
    [Table("repertorios")]
    public class Repertorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Column("data")]
        public DateOnly? Data { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }

        public List<RepertorioItem> Itens { get; set; } = [];
    }
}
