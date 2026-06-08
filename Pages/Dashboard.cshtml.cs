using ControleHoras.Data;
using ControleHoras.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ControleHoras.Services;

namespace ControleHoras.Pages;

public class DashboardModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ConfiguracaoService _configuracaoService;

    public DashboardModel(
        AppDbContext context,
        ConfiguracaoService configuracaoService)
    {
        _context = context;
        _configuracaoService = configuracaoService;
    }

    public int DiasTrabalhados { get; set; }

    public TimeSpan SaldoBancoHoras { get; set; }

    public TimeSpan TotalHorasTrabalhadas { get; set; }

    public TimeSpan TotalHorasExtras { get; set; }

    public TimeSpan TotalHorasDevedoras { get; set; }

    public TimeSpan MaiorCredito { get; set; }

    public DateTime? DataMaiorCredito { get; set; }

    public TimeSpan MaiorDebito { get; set; }

    public DateTime? DataMaiorDebito { get; set; }

    public TimeSpan DiaMaisTrabalhado { get; set; }

    public DateTime? DataDiaMaisTrabalhado { get; set; }

    public TimeSpan MediaHorasDia { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataInicial { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataFinal { get; set; }

    public TimeSpan SaldoPeriodo { get; set; }

    public double JornadaPadraoHoras { get; set; }

    public List<(DateTime Data, TimeSpan Saldo)> EvolucaoBanco { get; set; }
        = new();

    public List<(DateTime Data, TimeSpan Horas)> HorasPorDia { get; set; }
        = new();

    public void OnGet()
    {
        var query = _context.RegistrosPonto.AsQueryable();

        JornadaPadraoHoras =
            _configuracaoService.JornadaHoras();

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

        var registros = query.ToList();

        DiasTrabalhados = registros
            .Count(r => r.DiaEncerrado());

        SaldoBancoHoras = registros
            .Where(r => r.DiaEncerrado())
            .Select(r => r.SaldoDiario(JornadaPadraoHoras))
            .Aggregate(
                TimeSpan.Zero,
                (total, saldo) => total + saldo
            );

        TotalHorasTrabalhadas = registros
            .Where(r => r.DiaEncerrado())
            .Select(r => r.TotalTrabalhado())
            .Aggregate(
                TimeSpan.Zero,
                (total, horas) => total + horas
            );

        TotalHorasExtras = registros
            .Where(r =>
                r.DiaEncerrado() &&
                r.SaldoDiario(JornadaPadraoHoras) > TimeSpan.Zero)
            .Select(r => r.SaldoDiario(JornadaPadraoHoras))
            .Aggregate(
                TimeSpan.Zero,
                (total, saldo) => total + saldo
            );

        TotalHorasDevedoras = registros
            .Where(r =>
                r.DiaEncerrado() &&
                r.SaldoDiario(JornadaPadraoHoras) < TimeSpan.Zero)
            .Select(r => r.SaldoDiario(JornadaPadraoHoras).Duration())
            .Aggregate(
                TimeSpan.Zero,
                (total, saldo) => total + saldo
            );

        var todosRegistros = _context.RegistrosPonto
            .ToList();

        SaldoBancoHoras = todosRegistros
            .Where(r => r.DiaEncerrado())
            .Select(r => r.SaldoDiario(JornadaPadraoHoras))
            .Aggregate(
                TimeSpan.Zero,
                (total, saldo) => total + saldo
            );

        SaldoPeriodo = registros
            .Where(r => r.DiaEncerrado())
            .Select(r => r.SaldoDiario(JornadaPadraoHoras))
            .Aggregate(
                TimeSpan.Zero,
                (total, saldo) => total + saldo
            );

        TimeSpan acumulado = TimeSpan.Zero;

        foreach (var registro in registros
            .Where(r => r.DiaEncerrado())
            .OrderBy(r => r.Data))
        {
            acumulado += registro.SaldoDiario(JornadaPadraoHoras);

            EvolucaoBanco.Add(
                (registro.Data, acumulado)
            );
        }

        foreach (var registro in registros
            .Where(r => r.DiaEncerrado())
            .OrderBy(r => r.Data))
        {
            HorasPorDia.Add(
                (
                    registro.Data,
                    registro.TotalTrabalhado()
                )
            );
        }

        var encerrados = registros
            .Where(r => r.DiaEncerrado())
            .ToList();

        var maiorCreditoRegistro = encerrados
            .OrderByDescending(r => r.SaldoDiario(JornadaPadraoHoras))
            .FirstOrDefault();

        if (maiorCreditoRegistro != null &&
            maiorCreditoRegistro.SaldoDiario(JornadaPadraoHoras) > TimeSpan.Zero)
        {
            MaiorCredito = maiorCreditoRegistro.SaldoDiario(JornadaPadraoHoras);
            DataMaiorCredito = maiorCreditoRegistro.Data;
        }

        var maiorDebitoRegistro = encerrados
            .OrderBy(r => r.SaldoDiario(JornadaPadraoHoras))
            .FirstOrDefault();

        if (maiorDebitoRegistro != null &&
            maiorDebitoRegistro.SaldoDiario(JornadaPadraoHoras) < TimeSpan.Zero)
        {
            MaiorDebito =
                maiorDebitoRegistro.SaldoDiario(JornadaPadraoHoras).Duration();

            DataMaiorDebito =
                maiorDebitoRegistro.Data;
        }

        var diaMaisTrabalhadoRegistro = encerrados
            .OrderByDescending(r => r.TotalTrabalhado())
            .FirstOrDefault();

        if (diaMaisTrabalhadoRegistro != null)
        {
            DiaMaisTrabalhado =
                diaMaisTrabalhadoRegistro.TotalTrabalhado();

            DataDiaMaisTrabalhado =
                diaMaisTrabalhadoRegistro.Data;
        }

        if (encerrados.Any())
        {
            MediaHorasDia = TimeSpan.FromMinutes(

                encerrados
                    .Average(r =>
                        r.TotalTrabalhado().TotalMinutes)

            );
        }
    }
}