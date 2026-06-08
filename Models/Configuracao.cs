namespace ControleHoras.Models;

public class Configuracao
{
    public int Id { get; set; }

    public double JornadaDiariaHoras { get; set; }

    public int ToleranciaMinutos { get; set; } = 5;

    public int BancoHorasMeses { get; set; } = 6;

    public int IntervaloMinimoMinutos { get; set; } = 60;
}