using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hinario.Domain.Models
{
    [Table("repertorio_itens")]
    public class RepertorioItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("repertorio_id")]
        public int RepertorioId { get; set; }

        [Column("hino_id")]
        public int HinoId { get; set; }

        [Column("ordem")]
        public int Ordem { get; set; }

        [JsonIgnore]
        public Repertorio Repertorio { get; set; } = null!;
        public Hino Hino { get; set; } = null!;
    }
}
