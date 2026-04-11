using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;
using Hinario.Domain.Utils;

namespace Hinario.Application.Services
{
    public class HinoService(
        IHinoRepository hinoRepository, 
        ICampinaGrandeMineracaoService mineracaoService) : IHinoService
    {
        private readonly List<string> _ordemTipos = ["H", "C", "S", "HC", "L"];

        private const int TamanhoPagina = 10;

        public List<Hino> GetAll() => hinoRepository.GetAll();

        public Hino? GetById(int id) => hinoRepository.GetById(id);

        public Hino? GetByIdentificador(string identificador) => hinoRepository.GetByIdentificador(identificador);

        public List<HinoResultadoPesquisaDto> Pesquisar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return [];

            var textoNormalizado = TextNormalizer.Normalizar(texto);
            var palavras = textoNormalizado.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (palavras.Length == 0)
                return [];

            var tsQuery = string.Join(" | ", palavras);
            var hinos = hinoRepository.PesquisarPorTsQuery(tsQuery);

            var termosNegrito = new[] { textoNormalizado }
                .Concat(palavras)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .OrderByDescending(t => t.Length)
                .ToArray();

            return [.. hinos
                .Select(h =>
                {
                    var letraNormalizada = TextNormalizer.Normalizar(h.Letra ?? "");
                    var temFraseExata = letraNormalizada.Contains(textoNormalizado);
                    var totalPalavrasEncontradas = palavras.Count(p => letraNormalizada.Contains(p));
                    return new
                    {
                        Hino = h,
                        TemFraseExata = temFraseExata,
                        TemTodasPalavras = totalPalavrasEncontradas == palavras.Length,
                        TotalPalavrasEncontradas = totalPalavrasEncontradas
                    };
                })
                .OrderByDescending(x => x.TemFraseExata)
                .ThenByDescending(x => x.TemTodasPalavras)
                .ThenByDescending(x => x.TotalPalavrasEncontradas)
                .Take(TamanhoPagina)
                .Select(x => new HinoResultadoPesquisaDto
                {
                    Id = x.Hino.Id,
                    Identificador = x.Hino.Identificador,
                    Titulo = x.Hino.Titulo ?? string.Empty,
                    Letra = x.Hino.Letra,
                    Trecho = TrechoPesquisaHelper.BuildTrecho(x.Hino, termosNegrito)
                })];
        }

        public Hino? ObterProximo(string tipo, int numero)
        {
            var proximo = hinoRepository.ObterProximoNoTipo(tipo, numero);
            if (proximo != null) return proximo;

            var index = _ordemTipos.IndexOf(tipo);
            for (int i = index + 1; i < _ordemTipos.Count; i++)
            {
                var primeiro = hinoRepository.ObterPrimeiroPorTipo(_ordemTipos[i]);
                if (primeiro != null) return primeiro;
            }

            return null;
        }

        public Hino? ObterAnterior(string tipo, int numero)
        {
            var anterior = hinoRepository.ObterAnteriorNoTipo(tipo, numero);
            if (anterior != null) return anterior;

            var index = _ordemTipos.IndexOf(tipo);
            for (int i = index - 1; i >= 0; i--)
            {
                var ultimo = hinoRepository.ObterUltimoPorTipo(_ordemTipos[i]);
                if (ultimo != null) return ultimo;
            }

            return null;
        }

        public void Add(Hino hino) => hinoRepository.Add(hino);

        public Hino? Update(int id, Hino hino)
        {
            var hinoExistente = hinoRepository.GetById(id);
            if (hinoExistente == null) return null;

            hinoExistente.Titulo = string.IsNullOrEmpty(hino.Titulo) ? hinoExistente.Titulo : hino.Titulo;
            hinoExistente.Letra = string.IsNullOrEmpty(hino.Letra) ? hinoExistente.Letra : hino.Letra;
            hinoExistente.Identificador = string.IsNullOrEmpty(hino.Identificador) ? hinoExistente.Identificador : hino.Identificador;

            hinoRepository.Update(hinoExistente);
            return hinoExistente;
        }

        public (int importados, List<string> erros) ImportarHinos(List<HinoImportDto> hinosImport)
        {
            var hinos = new List<Hino>();
            var erros = new List<string>();

            foreach (var hinoImport in hinosImport)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(hinoImport.Title))
                    {
                        erros.Add($"Hino com ID {hinoImport.Id}: Título é obrigatório");
                        continue;
                    }

                    var letra = hinoImport.Lyrics?.FullText ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(letra))
                    {
                        erros.Add($"Hino '{hinoImport.Title}': Letra não encontrada");
                        continue;
                    }

                    var identificador = hinoImport.Id.ToString();
                    if (hinoRepository.GetByIdentificador(identificador) != null)
                    {
                        erros.Add($"Hino '{hinoImport.Title}' (ID: {hinoImport.Id}): Já existe um hino com este identificador");
                        continue;
                    }

                    hinos.Add(new Hino
                    {
                        Identificador = identificador,
                        Titulo = hinoImport.Title,
                        Letra = letra
                    });
                }
                catch (Exception ex)
                {
                    erros.Add($"Erro ao processar hino '{hinoImport.Title}': {ex.Message}");
                }
            }

            if (hinos.Count > 0)
                hinoRepository.AddRange(hinos);

            return (hinos.Count, erros);
        }

        public async Task<Hino?> MinerarCantico(int numero, CancellationToken cancellationToken)
        {
            var hino = await mineracaoService.ExtrairCanticoAsync(numero, cancellationToken);
            if (hino == null) return null;

            hinoRepository.Add(hino);
            return hino;
        }
    }
}
