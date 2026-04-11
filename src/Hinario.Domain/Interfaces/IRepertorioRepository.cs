using Hinario.Domain.Models;

namespace Hinario.Domain.Interfaces
{
    public interface IRepertorioRepository
    {
        List<Repertorio> GetAll();
        Repertorio? GetById(int id);
        Repertorio? GetAtivo();
        void Add(Repertorio repertorio);
        void Update(Repertorio repertorio);
        void Delete(Repertorio repertorio);
        void DesativarTodos();
    }
}
