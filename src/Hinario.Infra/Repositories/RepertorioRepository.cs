using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;
using Hinario.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Hinario.Infra.Repositories
{
    public class RepertorioRepository(HinarioApiContext context) : IRepertorioRepository
    {
        public List<Repertorio> GetAll() =>
            context.Repertorios
                .Include(r => r.Itens)
                    .ThenInclude(i => i.Hino)
                .OrderByDescending(r => r.Data)
                .ToList();

        public Repertorio? GetById(int id) =>
            context.Repertorios
                .Include(r => r.Itens)
                    .ThenInclude(i => i.Hino)
                .FirstOrDefault(r => r.Id == id);

        public Repertorio? GetAtivo() =>
            context.Repertorios
                .Include(r => r.Itens)
                    .ThenInclude(i => i.Hino)
                .FirstOrDefault(r => r.Ativo);

        public void Add(Repertorio repertorio)
        {
            context.Repertorios.Add(repertorio);
            context.SaveChanges();
        }

        public void Update(Repertorio repertorio)
        {
            context.Repertorios.Update(repertorio);
            context.SaveChanges();
        }

        public void Delete(Repertorio repertorio)
        {
            context.Repertorios.Remove(repertorio);
            context.SaveChanges();
        }

        public void DesativarTodos()
        {
            context.Repertorios
                .Where(r => r.Ativo)
                .ExecuteUpdate(s => s.SetProperty(r => r.Ativo, false));
        }
    }
}
