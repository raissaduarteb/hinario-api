using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hinario.Domain.Models
{
    [Table("hinos")]
    public class Hino
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id {get ; set;}
        [Column("identificador")]
        [MaxLength(50)]
        public string? Identificador {get ; set;}
        [Column("letra")]
        public string Letra {get ; set;}
        [Column("titulo")]
        public string Titulo {get ; set;}

        // Coluna gerada pelo banco, somente leitura
        public NpgsqlTsVector LetraIdx { get; set; }

        public Hino()
        {
        }

        public Hino(string letra, string titulo)
        {
            Letra = letra ?? throw new ArgumentNullException(nameof(letra));
            Titulo = titulo;
        }
    }

    public class Usuario
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
        public bool Ativo { get; set; }
    }
}