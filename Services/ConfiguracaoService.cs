using ControleHoras.Data;
using ControleHoras.Models;

namespace ControleHoras.Services;

public class ConfiguracaoService
{
    private readonly AppDbContext _context;

    public ConfiguracaoService(AppDbContext context)
    {
        _context = context;
    }

    public Configuracao Obter()
    {
        return _context.Configuracoes.FirstOrDefault()
               ?? new Configuracao();
    }

    public double JornadaHoras()
    {
        return Obter().JornadaDiariaHoras;
    }

    public int ToleranciaMinutos()
    {
        return Obter().ToleranciaMinutos;
    }

    public int BancoHorasMeses()
    {
        return Obter().BancoHorasMeses;
    }
}