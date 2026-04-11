using Hinario.Domain.Models;

namespace Hinario.Domain.Interfaces
{
    /// <summary>
    /// Serviço de mineração de hinos do site Igreja em Campina Grande (Cânticos).
    /// </summary>
    public interface ICampinaGrandeMineracaoService
    {
        /// <summary>
        /// Extrai dados de um cântico (1-100) do site e retorna o hino parseado, ou null se falhar.
        /// </summary>
        Task<Hino?> ExtrairCanticoAsync(int numero, CancellationToken cancellationToken = default);
    }
}
