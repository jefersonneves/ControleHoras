using ControleHoras.Data;
using ControleHoras.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace ControleHoras.Pages;

public class ConfiguracoesModel : PageModel
{
    private readonly AppDbContext _context;

    public string CaminhoBanco { get; set; } = "";
    public string TamanhoBanco { get; set; } = "";

    public ConfiguracoesModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Configuracao Config { get; set; } = new();

    public void OnGet()
    {
        Config = _context.Configuracoes.FirstOrDefault();

        if (Config == null)
        {
            Config = new Configuracao();
        }

        var caminhoBanco = Path.Combine(
            Directory.GetCurrentDirectory(),
            "controlehoras.db"
        );

        CaminhoBanco = Path.GetFileName(caminhoBanco);

        if (System.IO.File.Exists(caminhoBanco))
        {
            var info = new FileInfo(caminhoBanco);

            TamanhoBanco =
                $"{info.Length / 1024.0:F2} KB";
        }

    }

    public IActionResult OnPost()
    {
        var configBanco =
            _context.Configuracoes.FirstOrDefault();

        if (configBanco == null)
        {
            _context.Configuracoes.Add(Config);
        }
        else
        {
            configBanco.JornadaDiariaHoras =
                Config.JornadaDiariaHoras;

            configBanco.ToleranciaMinutos =
                Config.ToleranciaMinutos;

            configBanco.BancoHorasMeses =
                Config.BancoHorasMeses;

            configBanco.IntervaloMinimoMinutos =
                Config.IntervaloMinimoMinutos;
        }

        _context.SaveChanges();

        return RedirectToPage();
    }
}