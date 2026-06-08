using ControleHoras.Data;
using ControleHoras.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ControleHoras.Services;

namespace ControleHoras.Pages;

public class HistoricoModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ConfiguracaoService _configuracaoService;

    public HistoricoModel(
        AppDbContext context,
        ConfiguracaoService configuracaoService)
    {
        _context = context;
        _configuracaoService = configuracaoService;
    }

    public List<RegistroPonto> Registros { get; set; } = new();

    public TimeSpan SaldoPeriodo { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataInicial { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataFinal { get; set; }

    public double JornadaPadraoHoras { get; set; }

    public void OnGet()
    {

        JornadaPadraoHoras =
            _configuracaoService.JornadaHoras();

        var query = _context.RegistrosPonto
            .Where(r =>
                r.EntradaManha != null ||
                r.SaidaManha != null ||
                r.EntradaTarde != null ||
                r.SaidaTarde != null);

        if (DataInicial.HasValue)
        {
            query = query.Where(r =>
                r.Data.Date >= DataInicial.Value.Date);
        }

        if (DataFinal.HasValue)
        {
            query = query.Where(r =>
                r.Data.Date <= DataFinal.Value.Date);
        }

        Registros = query
            .OrderBy(r => r.Data)
            .ToList();

        SaldoPeriodo = TimeSpan.FromMinutes(
            Registros.Sum(r =>
                r.SaldoDiario(JornadaPadraoHoras)
                .TotalMinutes)
        );
    }

    public IActionResult OnPostExcluir(int id)
    {
        var registro = _context.RegistrosPonto
            .FirstOrDefault(r => r.Id == id);

        if (registro != null)
        {
            _context.RegistrosPonto.Remove(registro);
            _context.SaveChanges();
        }

        return RedirectToPage();
    }

}
