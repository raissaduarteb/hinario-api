using System.Text;
using Hinario.Domain.Models;

namespace Hinario.Domain.Utils
{
    /// <summary>
    /// Monta o trecho (preview) para resultado de pesquisa: primeira linha + linha onde o termo foi encontrado,
    /// com o termo em negrito (html <b>termo</b>).
    /// </summary>
    public static class TrechoPesquisaHelper
    {
        private const string NegritoMarcadorInicial = "<b>";
        private const string NegritoMarcadorFinal = "</b>";

        /// <summary>
        /// Gera o trecho para exibição na pesquisa.
        /// Regras: no máximo primeira linha + linha onde a palavra foi encontrada;
        /// se a palavra estiver na primeira linha, retorna as 2 primeiras linhas;
        /// a palavra encontrada é retornada em negrito (<b>palavra</b>).
        /// </summary>
        public static string BuildTrecho(Hino hino, string[] palavrasNormalizadas)
        {
            if (string.IsNullOrWhiteSpace(hino.Letra) || palavrasNormalizadas.Length == 0)
                return string.Empty;

            var linhas = hino.Letra.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
            if (linhas.Length == 0)
                return string.Empty;

            var primeirasLinhas = linhas[0];
            if (linhas.Length > 1)
                primeirasLinhas += '\n' + linhas[1];

            var frasePesquisada = string.Join(" ", palavrasNormalizadas);

            // Pontuar cada linha seguindo a mesma lógica de prioridade
            var melhorLinha = linhas
                .Select((linha, indice) =>
                {
                    var linhaNorm = TextNormalizer.Normalizar(linha);
                    var temFraseExata = linhaNorm.Contains(frasePesquisada);
                    var totalPalavrasEncontradas = palavrasNormalizadas.Count(p => !string.IsNullOrEmpty(p) && linhaNorm.Contains(p));

                    return new
                    {
                        Indice = indice,
                        Linha = linha,
                        TemFraseExata = temFraseExata,
                        TemTodasPalavras = totalPalavrasEncontradas == palavrasNormalizadas.Length,
                        TotalPalavrasEncontradas = totalPalavrasEncontradas
                    };
                })
                .Where(x => x.TotalPalavrasEncontradas > 0) // só linhas com ao menos uma palavra
                .OrderByDescending(x => x.TemFraseExata)
                .ThenByDescending(x => x.TemTodasPalavras)
                .ThenByDescending(x => x.TotalPalavrasEncontradas)
                .FirstOrDefault();

            // Nenhuma linha encontrada
            if (melhorLinha == null)
                return ColocarTermoEmNegrito(primeirasLinhas, primeirasLinhas, palavrasNormalizadas);

            // Melhor linha é a primeira ou segunda: retorna as 2 primeiras
            if (melhorLinha.Indice <= 1)
            {
                var linhasPreview = linhas.Length >= 2
                    ? new[] { linhas[0], linhas[1] }
                    : new[] { linhas[0] };

                return string.Join("\n", linhasPreview.Select(l =>
                    ColocarTermoEmNegrito(l, l, palavrasNormalizadas)));
            }

            // Melhor linha está em outro trecho: primeiras linhas + melhor linha
            var primeiraComNegrito = ColocarTermoEmNegrito(primeirasLinhas, primeirasLinhas, palavrasNormalizadas);
            var melhorLinhaComNegrito = ColocarTermoEmNegrito(melhorLinha.Linha, melhorLinha.Linha, palavrasNormalizadas);
            return primeiraComNegrito + "[...]\n" + melhorLinhaComNegrito;
        }

        /// <summary>
        /// Coloca em negrito (**) a primeira ocorrência de qualquer palavra normalizada que exista na linha original.
        /// Usa mapeamento normalizado -> original para destacar o texto original (com acentos).
        /// </summary>
        private static string ColocarTermoEmNegrito(string linhaOriginal, string linhaParaBuscar, string[] palavrasNormalizadas)
        {
            var linhaNorm = TextNormalizer.Normalizar(linhaParaBuscar);
            (int start, int len)? segmentoNorm = null;
            string? palavraMatch = null;

            foreach (var palavra in palavrasNormalizadas)
            {
                if (string.IsNullOrEmpty(palavra)) continue;
                var idx = linhaNorm.IndexOf(palavra, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    segmentoNorm = (idx, palavra.Length);
                    palavraMatch = palavra;
                    break;
                }
            }

            if (!segmentoNorm.HasValue || string.IsNullOrEmpty(palavraMatch))
                return linhaOriginal;

            var (normStart, normLen) = segmentoNorm.Value;

            // Mapear posições normalizadas de volta para a linha original (para acentos: "água" vs "agua")
            var normToOrig = new List<int>();
            var normSb = new StringBuilder();
            for (int i = 0; i < linhaOriginal.Length; i++)
            {
                var n = TextNormalizer.Normalizar(linhaOriginal[i].ToString());
                for (int k = 0; k < n.Length; k++)
                {
                    normToOrig.Add(i);
                    normSb.Append(n[k]);
                }
            }

            if (normStart + normLen > normToOrig.Count)
                return linhaOriginal;

            int origStart = normToOrig[normStart];
            int origEnd = normToOrig[normStart + normLen - 1] + 1;
            if (origEnd > linhaOriginal.Length) origEnd = linhaOriginal.Length;

            var termoOriginal = linhaOriginal.Substring(origStart, origEnd - origStart);
            return linhaOriginal.Substring(0, origStart)
                 + NegritoMarcadorInicial + termoOriginal + NegritoMarcadorFinal
                 + linhaOriginal.Substring(origEnd);
        }
    }
}
