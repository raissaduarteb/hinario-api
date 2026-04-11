using System.Text;
using HtmlAgilityPack;
using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;

namespace Hinario.Services
{
    public class CampinaGrandeMineracaoService : ICampinaGrandeMineracaoService
    {
        private const string BaseUrl = "https://www.igrejaemcampinagrande.com.br/old/Hinario_Online/Canticos/";
        private static readonly Encoding IsoLatin1 = Encoding.GetEncoding("ISO-8859-1");

        private readonly HttpClient _httpClient;

        public CampinaGrandeMineracaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "HinarioApi/1.0");
        }

        public async Task<Hino?> ExtrairCanticoAsync(int numero, CancellationToken cancellationToken = default)
        {
            // URL usa número com 2 dígitos (01, 02, ... 99, 100)
            var segmento = numero.ToString("00");
            var url = $"{BaseUrl}{segmento}.htm";

            byte[] bytes;
            try
            {
                bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);
            }
            catch
            {
                return null;
            }

            var (encoding, bomLength) = DetectarEncoding(bytes);
            var html = encoding.GetString(bytes, bomLength, bytes.Length - bomLength);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string? titulo = null;
            string? referencia = null;
            string? letra = null;

            var rows = doc.DocumentNode.SelectNodes("//table//tr");
            if (rows == null)
                return null;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 2)
                    continue;

                // Rótulos podem vir com entidades HTML (ex: T&iacute;tulo em vez de Título)
                var labelCell = DecodeHtml(cells[0].InnerText.Trim());
                var valueCell = cells[1];

                if (labelCell.Contains("Título", StringComparison.OrdinalIgnoreCase))
                {
                    titulo = NormalizarEspacamento(DecodeHtml(valueCell.InnerText.Trim()));
                }
                else if (labelCell.Contains("Referencia", StringComparison.OrdinalIgnoreCase) ||
                         labelCell.Contains("Referência", StringComparison.OrdinalIgnoreCase))
                {
                    referencia = DecodeHtml(valueCell.InnerText.Trim());
                }
                else if (labelCell.Contains("Letra", StringComparison.OrdinalIgnoreCase))
                {
                    // Preservar quebras de linha: <br> vira \n
                    var innerHtml = valueCell.InnerHtml;
                    if (!string.IsNullOrEmpty(innerHtml))
                    {
                        var text = HtmlEntity.DeEntitize(innerHtml);
                        text = System.Net.WebUtility.HtmlDecode(text);
                        text = text.Replace("<br>", "\n", StringComparison.OrdinalIgnoreCase)
                                   .Replace("<br/>", "\n", StringComparison.OrdinalIgnoreCase)
                                   .Replace("<br />", "\n", StringComparison.OrdinalIgnoreCase);
                        var docLetra = new HtmlDocument();
                        docLetra.LoadHtml("<div>" + text + "</div>");
                        letra = DecodeHtml(docLetra.DocumentNode.InnerText.Trim());
                        letra = NormalizarEspacamento(letra);
                    }
                    else
                    {
                        letra = NormalizarEspacamento(DecodeHtml(valueCell.InnerText.Trim()));
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(letra))
                return null;

            // Remover título duplicado no início da letra (evita aparecer duas vezes no app)
            letra = RemoverTituloDoInicioDaLetra(letra, titulo);

            // Identificador: C e o número escolhido (ex: C-5)
            var identificador = $"C-{numero}";

            return new Hino
            {
                Identificador = identificador,
                Titulo = titulo,
                Letra = letra
            };
        }

        /// <summary>
        /// Detecta a codificação dos bytes pelo BOM e retorna o encoding e quantos bytes pular (BOM).
        /// Se não houver BOM, usa ISO-8859-1 (declarado na página).
        /// </summary>
        private static (Encoding encoding, int bomLength) DetectarEncoding(byte[] bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return (Encoding.UTF8, 3);
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return (Encoding.Unicode, 2); // UTF-16 LE
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return (Encoding.BigEndianUnicode, 2); // UTF-16 BE
            return (IsoLatin1, 0);
        }

        private static string DecodeHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return System.Net.WebUtility.HtmlDecode(HtmlEntity.DeEntitize(text)).Trim();
        }

        /// <summary>
        /// Se a letra começar com o título (repetido do site), remove para não duplicar no app.
        /// </summary>
        private static string RemoverTituloDoInicioDaLetra(string letra, string titulo)
        {
            if (string.IsNullOrWhiteSpace(letra) || string.IsNullOrWhiteSpace(titulo))
                return letra;

            var letraTrimmed = letra.TrimStart();
            if (!letraTrimmed.StartsWith(titulo, StringComparison.OrdinalIgnoreCase))
                return letra;

            return letraTrimmed.Substring(titulo.Length).TrimStart();
        }

        /// <summary>
        /// Normaliza espaçamento: cada linha é trimada; várias linhas vazias consecutivas viram uma só.
        /// Assim preserva-se uma linha em branco entre estrofes/refrões (como no site de Campina Grande),
        /// sem o excesso de linhas vazias que às vezes vem no HTML.
        /// </summary>
        private static string NormalizarEspacamento(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto?.Trim() ?? string.Empty;

            var linhas = texto.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            var resultado = new List<string>();
            var ultimaFoiVazia = false;

            foreach (var linha in linhas)
            {
                var trimmed = linha.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    if (ultimaFoiVazia)
                    {
                        resultado.Add(string.Empty);
                        ultimaFoiVazia = false;
                    }
                    else
                    {
                        ultimaFoiVazia = true;
                    }
                }
                else
                {
                    resultado.Add(trimmed);
                    ultimaFoiVazia = false;
                }
            }

            return string.Join("\n", resultado).Trim();
        }
    }
}
