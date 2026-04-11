using Hinario.Infra.Context;
using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hinario.Infra.Repositories;

public class HinoRepository : IHinoRepository
{
    private readonly HinarioApiContext _context;

    public HinoRepository(HinarioApiContext context)
    {
        _context = context;
    }

    public List<Hino> GetAll() => _context.Hinos.ToList();

    public Hino? GetById(int id) => _context.Hinos.Find(id);

    public Hino? GetByIdentificador(string identificador)
    {
        var idNormalizado = identificador.ToUpper().Replace("-", "");
        return _context.Hinos.FirstOrDefault(h =>
            h.Identificador != null &&
            h.Identificador.ToUpper().Replace("-", "") == idNormalizado);
    }

    public List<Hino> PesquisarPorTsQuery(string tsQuery)
    {
        return _context.Hinos
            .AsNoTracking()
            .Where(h => h.LetraIdx.Matches(EF.Functions.ToTsQuery("portuguese", tsQuery)))
            .ToList();
    }

    public Hino? ObterPrimeiroPorTipo(string tipo)
    {
        return _context.Hinos
            .Where(h => h.Identificador != null && h.Identificador.StartsWith(tipo + "-"))
            .AsEnumerable()
            .OrderBy(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0"))
            .FirstOrDefault();
    }

    public Hino? ObterUltimoPorTipo(string tipo)
    {
        return _context.Hinos
            .Where(h => h.Identificador != null && h.Identificador.StartsWith(tipo + "-"))
            .AsEnumerable()
            .OrderByDescending(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0"))
            .FirstOrDefault();
    }

    public Hino? ObterProximoNoTipo(string tipo, int numero)
    {
        return _context.Hinos
            .Where(h => h.Identificador != null && h.Identificador.StartsWith(tipo + "-"))
            .AsEnumerable()
            .Where(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0") > numero)
            .OrderBy(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0"))
            .FirstOrDefault();
    }

    public Hino? ObterAnteriorNoTipo(string tipo, int numero)
    {
        return _context.Hinos
            .Where(h => h.Identificador != null && h.Identificador.StartsWith(tipo + "-"))
            .AsEnumerable()
            .Where(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0") < numero)
            .OrderByDescending(h => int.Parse(h.Identificador?.Replace(tipo + "-", string.Empty) ?? "0"))
            .FirstOrDefault();
    }

    public void Add(Hino hino)
    {
        _context.Hinos.Add(hino);
        _context.SaveChanges();
    }

    public void AddRange(List<Hino> hinos)
    {
        _context.Hinos.AddRange(hinos);
        _context.SaveChanges();
    }

    public void Update(Hino hino)
    {
        _context.Hinos.Update(hino);
        _context.SaveChanges();
    }
}
