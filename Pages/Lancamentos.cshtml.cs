using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ControleHoras.Models;
using ControleHoras.Data;
using ControleHoras.Services;

namespace ControleHoras.Pages;

public class LancamentosModel : PageModel
{
    private readonly AppDbContext _context;

    private readonly ConfiguracaoService _configuracaoService;

    public LancamentosModel(
        AppDbContext context,
        ConfiguracaoService configuracaoService)
    {
        _context = context;
        _configuracaoService = configuracaoService;
    }

    private RegistroPonto? ObterRegistroSelecionado()
    {
        return _context.RegistrosPonto
            .FirstOrDefault(r => r.Data.Date == DataSelecionada.Date);
    }

    public RegistroPonto RegistroHoje { get; set; } = null!;

    public TimeSpan SaldoAcumulado { get; set; }

    [BindProperty]
    public string? EntradaManha { get; set; }

    [BindProperty]
    public string? SaidaManha { get; set; }

    [BindProperty]
    public string? EntradaTarde { get; set; }

    [BindProperty]
    public string? SaidaTarde { get; set; }

    public string? MensagemErro { get; set; }

    public string MetaDiariaFormatada { get; set; } = "";

    [BindProperty]
    public string? Observacao { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime DataSelecionada { get; set; } = DateTime.Today;

    public double JornadaHoras { get; set; }

    public void OnGet()
    {
        Console.WriteLine("=== ONGET EXECUTOU ===");
        Console.WriteLine($"DataSelecionada = {DataSelecionada:yyyy-MM-dd}");

        if (DataSelecionada.Date > DateTime.Today)
        {
            DataSelecionada = DateTime.Today;
        }

        var registro = _context.RegistrosPonto
            .FirstOrDefault(r => r.Data.Date == DataSelecionada.Date);

        Console.WriteLine($"Registro encontrado? {registro != null}");

        if (registro == null)
        {
            Console.WriteLine("Criando registro para a data");

            registro = new RegistroPonto
            {
                Data = DataSelecionada.Date
            };

            _context.RegistrosPonto.Add(registro);
            _context.SaveChanges();
        }

        RegistroHoje = registro;

        Console.WriteLine($"RegistroHoje null? {RegistroHoje == null}");

        var jornada = _configuracaoService.JornadaHoras();

    }

    public IActionResult OnPostRegistrar()
    {

        Console.WriteLine("=== ONPOST REGISTRAR EXECUTOU ===");

        var registroHoje = ObterRegistroSelecionado();

        if (registroHoje == null)
        {
            registroHoje = new RegistroPonto
            {
                Data = DataSelecionada.Date
            };

            _context.RegistrosPonto.Add(registroHoje);
        }

        registroHoje.RegistrarBatida(
            DateTime.Now.TimeOfDay
        );

        _context.SaveChanges();

        return RedirectToPage(new
        {
            DataSelecionada = DataSelecionada.ToString("yyyy-MM-dd")
        });
    }

    public IActionResult OnPostDesfazer()
    {

        Console.WriteLine("=== ONPOST DESFAZER EXECUTOU ===");
        
        var registroHoje = ObterRegistroSelecionado();

        if (registroHoje == null)
        {
            return RedirectToPage();
        }

        registroHoje.DesfazerUltimaBatida();

        _context.SaveChanges();

        return RedirectToPage(new
{
            DataSelecionada = DataSelecionada.ToString("yyyy-MM-dd")
        });
    }

    public IActionResult OnPostSalvarHorarios()
    {
        MensagemErro = null;

        var registroHoje = ObterRegistroSelecionado();

        if (registroHoje == null)
        {
            return RedirectToPage();
        }

        var registroTemp = new RegistroPonto
        {
            Data = registroHoje.Data,

            EntradaManha = string.IsNullOrWhiteSpace(EntradaManha)
                ? registroHoje.EntradaManha
                : TimeSpan.Parse(EntradaManha),

            SaidaManha = string.IsNullOrWhiteSpace(SaidaManha)
                ? registroHoje.SaidaManha
                : TimeSpan.Parse(SaidaManha),

            EntradaTarde = string.IsNullOrWhiteSpace(EntradaTarde)
                ? registroHoje.EntradaTarde
                : TimeSpan.Parse(EntradaTarde),

            SaidaTarde = string.IsNullOrWhiteSpace(SaidaTarde)
                ? registroHoje.SaidaTarde
                : TimeSpan.Parse(SaidaTarde)
        };

        var erro = registroTemp.ValidarHorarios(
            _configuracaoService
                .Obter()
                .IntervaloMinimoMinutos
        );

        if (erro != null)
        {
            MensagemErro = erro;
            RegistroHoje = registroHoje;
            return Page();
        }

        registroHoje.EntradaManha = registroTemp.EntradaManha;
        registroHoje.SaidaManha = registroTemp.SaidaManha;
        registroHoje.EntradaTarde = registroTemp.EntradaTarde;
        registroHoje.SaidaTarde = registroTemp.SaidaTarde;

        _context.SaveChanges();

        MensagemErro = null;

        return RedirectToPage(new
{
            DataSelecionada = DataSelecionada.ToString("yyyy-MM-dd")
        });
    }

    public IActionResult OnPostSalvarObservacao()
    {
        var registroHoje = ObterRegistroSelecionado();

        if (registroHoje == null)
        {
            return RedirectToPage();
        }

        registroHoje.Observacao = Observacao;

        _context.SaveChanges();

        return RedirectToPage(new
{
            DataSelecionada = DataSelecionada.ToString("yyyy-MM-dd")
        });
    }

}