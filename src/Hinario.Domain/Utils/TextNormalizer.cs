using System.Globalization;
using System.Text;

namespace Hinario.Domain.Utils
{
    public static class TextNormalizer
    {
        /// <summary>
        /// Normaliza o texto removendo acentos e convertendo para minúsculas.
        /// </summary>
        /// <param name="texto">Texto a ser normalizado</param>
        /// <returns>Texto normalizado sem acentos e em minúsculas</returns>
        public static string Normalizar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return RemoverAcentos(texto.ToLowerInvariant());
        }

        /// <summary>
        /// Remove os acentos (diacríticos) de uma string.
        /// </summary>
        /// <param name="texto">Texto do qual remover os acentos</param>
        /// <returns>Texto sem acentos</returns>
        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

