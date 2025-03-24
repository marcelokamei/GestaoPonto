using GestaoPonto.Models;

namespace GestaoPonto.Repositories
{
    public interface IColaboradorRepository
    {

        Task<int> GetTotalRegistrosAsync();
        Task<IEnumerable<RegistoPonto>> GetRegistrosAsync(int pageNumber, int pageSize);


        Task<IEnumerable<RegistoPonto>> GetAllRegistrosAsync();
        Task<int> GetTotalRegistrosByColaboradorIdAsync(int colaboradorId);
        Task<IEnumerable<RegistoPonto>> GetRegistrosByColaboradorIdAsync(int colaboradorId, int pageNumber, int pageSize);
        Task AddRegistroAsync(RegistoPonto registro);

        Task<Colaborador> GetByIdAsync(int id);
        Task<IEnumerable<Colaborador>> GetAllAsync();
        Task AddAsync(Colaborador colaborador);
        Task UpdateAsync(Colaborador colaborador);
        Task DeleteAsync(Colaborador colaborador);
        Task<Colaborador> GetByIdentityUserIdAsync(string identityUserId);

    }
}
