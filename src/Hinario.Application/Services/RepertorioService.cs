using Hinario.Domain.Interfaces;
using Hinario.Domain.Models;

namespace Hinario.Application.Services
{
    public class RepertorioService(
        IRepertorioRepository repertorioRepository,
        IHinoRepository hinoRepository) : IRepertorioService
    {
        public List<Repertorio> GetAll() => repertorioRepository.GetAll();

        public Repertorio? GetById(int id) => repertorioRepository.GetById(id);

        public Repertorio? GetAtivo() => repertorioRepository.GetAtivo();

        public Repertorio Criar(string nome, DateOnly? data)
        {
            var repertorio = new Repertorio
            {
                Nome = nome,
                Data = data,
                Ativo = false
            };
            repertorioRepository.Add(repertorio);
            return repertorio;
        }

        public Repertorio? Atualizar(int id, string? nome, DateOnly? data)
        {
            var repertorio = repertorioRepository.GetById(id);
            if (repertorio == null) return null;

            if (!string.IsNullOrWhiteSpace(nome))
                repertorio.Nome = nome;

            if (data.HasValue)
                repertorio.Data = data;

            repertorioRepository.Update(repertorio);
            return repertorio;
        }

        public bool Deletar(int id)
        {
            var repertorio = repertorioRepository.GetById(id);
            if (repertorio == null) return false;

            repertorioRepository.Delete(repertorio);
            return true;
        }

        public bool Ativar(int id)
        {
            var repertorio = repertorioRepository.GetById(id);
            if (repertorio == null) return false;

            repertorioRepository.DesativarTodos();
            repertorio.Ativo = true;
            repertorioRepository.Update(repertorio);
            return true;
        }

        public Repertorio? AdicionarHino(int repertorioId, int hinoId)
        {
            var repertorio = repertorioRepository.GetById(repertorioId);
            if (repertorio == null) return null;

            if (hinoRepository.GetById(hinoId) == null) return null;

            var proximaOrdem = repertorio.Itens.Count == 0
                ? 1
                : repertorio.Itens.Max(i => i.Ordem) + 1;

            repertorio.Itens.Add(new RepertorioItem
            {
                HinoId = hinoId,
                Ordem = proximaOrdem
            });

            repertorioRepository.Update(repertorio);
            return repertorio;
        }

        public Repertorio? RemoverHino(int repertorioId, int itemId)
        {
            var repertorio = repertorioRepository.GetById(repertorioId);
            if (repertorio == null) return null;

            var item = repertorio.Itens.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return null;

            repertorio.Itens.Remove(item);

            // Reordenar sequencialmente após remoção
            var itensOrdenados = repertorio.Itens.OrderBy(i => i.Ordem).ToList();
            for (int i = 0; i < itensOrdenados.Count; i++)
                itensOrdenados[i].Ordem = i + 1;

            repertorioRepository.Update(repertorio);
            return repertorio;
        }

        public Repertorio? ReordenarHinos(int repertorioId, List<int> itemIds)
        {
            var repertorio = repertorioRepository.GetById(repertorioId);
            if (repertorio == null) return null;

            // Validar que todos os itemIds pertencem ao repertório
            var itensDoRepertorio = repertorio.Itens.ToDictionary(i => i.Id);
            if (itemIds.Any(id => !itensDoRepertorio.ContainsKey(id)))
                return null;

            for (int i = 0; i < itemIds.Count; i++)
                itensDoRepertorio[itemIds[i]].Ordem = i + 1;

            repertorioRepository.Update(repertorio);
            return repertorio;
        }

        public RepertorioItem? GetProximoItem(int repertorioId, int ordemAtual)
        {
            var repertorio = repertorioRepository.GetById(repertorioId);
            if (repertorio == null) return null;

            return repertorio.Itens
                .Where(i => i.Ordem > ordemAtual)
                .OrderBy(i => i.Ordem)
                .FirstOrDefault();
        }

        public RepertorioItem? GetAnteriorItem(int repertorioId, int ordemAtual)
        {
            var repertorio = repertorioRepository.GetById(repertorioId);
            if (repertorio == null) return null;

            return repertorio.Itens
                .Where(i => i.Ordem < ordemAtual)
                .OrderByDescending(i => i.Ordem)
                .FirstOrDefault();
        }
    }
}
