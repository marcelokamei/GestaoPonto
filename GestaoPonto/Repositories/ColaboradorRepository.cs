using GestaoPonto.Data;
using GestaoPonto.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoPonto.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly GestaoPontoDbContext _context;
        public ColaboradorRepository(GestaoPontoDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalRegistrosAsync()
        {
            return await _context.RegistosPonto.CountAsync();
        }

        public async Task<IEnumerable<RegistoPonto>> GetRegistrosAsync(int pageNumber, int pageSize)
        {
            return await _context.RegistosPonto
                .Include(r => r.Colaborador)
                .OrderByDescending(r => r.DataHora)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<RegistoPonto>> GetAllRegistrosAsync()
        {
            return await _context.RegistosPonto
                .Include(r => r.Colaborador)
                .ToListAsync();
        }

        public async Task<Colaborador> GetByIdAsync(int id)
        {
            return await _context.Colaboradores.FindAsync(id);
        }

        public async Task<int> GetTotalRegistrosByColaboradorIdAsync(int colaboradorId)
        {
            return await _context.RegistosPonto.CountAsync(r => r.ColaboradorId == colaboradorId);
        }

        public async Task<IEnumerable<RegistoPonto>> GetRegistrosByColaboradorIdAsync(int colaboradorId, int pageNumber, int pageSize)
        {
            return await _context.RegistosPonto
                .Where(r => r.ColaboradorId == colaboradorId)
                .OrderByDescending(r => r.DataHora)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddRegistroAsync(RegistoPonto registro)
        {
            _context.RegistosPonto.Add(registro);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            return await _context.Colaboradores.ToListAsync();
        }

        public async Task AddAsync(Colaborador colaborador)
        {
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Colaborador colaborador)
        {
            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Colaborador colaborador)
        {
            _context.Colaboradores.Update(colaborador);
            await _context.SaveChangesAsync();
        }

        public async Task<Colaborador> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Colaboradores.FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);
        }
    }
}
