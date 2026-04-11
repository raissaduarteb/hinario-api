using Hinario.Domain.Models;

namespace Hinario.Domain.Interfaces
{
    public interface IHinoService
    {
        List<Hino> GetAll();
        Hino? GetById(int id);
        Hino? GetByIdentificador(string identificador);
        List<HinoResultadoPesquisaDto> Pesquisar(string texto);
        Hino? ObterProximo(string tipo, int numero);
        Hino? ObterAnterior(string tipo, int numero);
        void Add(Hino hino);
        (int importados, List<string> erros) ImportarHinos(List<HinoImportDto> hinosImport);
        Hino? Update(int id, Hino hino);
        Task<Hino?> MinerarCantico(int numero, CancellationToken cancellationToken);
    }
}
