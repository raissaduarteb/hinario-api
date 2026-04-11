namespace Hinario.Domain.Dtos
{
    public class CriarRepertorioDto
    {
        public string Nome { get; set; } = string.Empty;
        public DateOnly? Data { get; set; }
    }

    public class AtualizarRepertorioDto
    {
        public string? Nome { get; set; }
        public DateOnly? Data { get; set; }
    }

    public class AdicionarHinoRepertorioDto
    {
        public int HinoId { get; set; }
    }

    public class ReordenarHinosDto
    {
        /// <summary>
        /// IDs dos RepertorioItens na nova ordem desejada.
        /// </summary>
        public List<int> ItemIds { get; set; } = [];
    }
}
