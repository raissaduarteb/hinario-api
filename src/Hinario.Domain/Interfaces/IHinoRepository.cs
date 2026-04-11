using Hinario.Domain.Models;

namespace Hinario.Domain.Interfaces
{
    public interface IHinoRepository
    {
        List<Hino> GetAll();
        Hino? GetById(int id);
        Hino? GetByIdentificador(string identificador);
        List<Hino> PesquisarPorTsQuery(string tsQuery);
        Hino? ObterPrimeiroPorTipo(string tipo);
        Hino? ObterUltimoPorTipo(string tipo);
        Hino? ObterProximoNoTipo(string tipo, int numero);
        Hino? ObterAnteriorNoTipo(string tipo, int numero);
        void Add(Hino hino);
        void AddRange(List<Hino> hinos);
        void Update(Hino hino);
    }
}
