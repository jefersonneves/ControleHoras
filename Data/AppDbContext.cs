using Microsoft.EntityFrameworkCore;
using ControleHoras.Models;

namespace ControleHoras.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<RegistroPonto> RegistrosPonto { get; set; }

    public DbSet<Configuracao> Configuracoes { get; set; }
}