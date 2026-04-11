namespace Hinario.Domain.Models
{
    /// <summary>
    /// Resultado da pesquisa de hinos, incluindo um trecho (preview) com a primeira linha e a linha onde o termo foi encontrado,
    /// com o termo destacado em negrito (formato markdown **termo**).
    /// </summary>
    public class HinoResultadoPesquisaDto
    {
        public int Id { get; set; }
        public string? Identificador { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Letra { get; set; } = string.Empty;

        /// <summary>
        /// Trecho para preview: no máximo a primeira linha + a linha onde o termo foi encontrado.
        /// Se o termo estiver na primeira linha, retorna as 2 primeiras linhas.
        /// O termo encontrado aparece em negrito (markdown: **termo**).
        /// </summary>
        public string Trecho { get; set; } = string.Empty;
    }
}
