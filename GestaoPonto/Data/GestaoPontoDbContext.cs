using GestaoPonto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoPonto.Data
{
    public class GestaoPontoDbContext : IdentityDbContext<IdentityUser>
    {
        public GestaoPontoDbContext(DbContextOptions<GestaoPontoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<RegistoPonto> RegistosPonto { get; set; }
    }
}
