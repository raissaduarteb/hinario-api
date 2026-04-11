using Hinario.Domain.Models;

namespace Hinario.Domain.Interfaces
{
    public interface IRepertorioService
    {
        List<Repertorio> GetAll();
        Repertorio? GetById(int id);
        Repertorio? GetAtivo();
        Repertorio Criar(string nome, DateOnly? data);
        Repertorio? Atualizar(int id, string? nome, DateOnly? data);
        bool Deletar(int id);
        bool Ativar(int id);
        Repertorio? AdicionarHino(int repertorioId, int hinoId);
        Repertorio? RemoverHino(int repertorioId, int hinoId);
        Repertorio? ReordenarHinos(int repertorioId, List<int> itemIds);
        RepertorioItem? GetProximoItem(int repertorioId, int ordemAtual);
        RepertorioItem? GetAnteriorItem(int repertorioId, int ordemAtual);
    }
}
