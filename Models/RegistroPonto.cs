namespace ControleHoras.Models;

public class RegistroPonto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }

    public TimeSpan? EntradaManha { get; set; }
    public TimeSpan? SaidaManha { get; set; }
    public TimeSpan? EntradaTarde { get; set; }
    public TimeSpan? SaidaTarde { get; set; }

    public string? Observacao { get; set; }

    // -------------------------
    // MÉTODOS CALCULADOS
    // -------------------------

    public TimeSpan TotalTrabalhado()
    {
        TimeSpan total = TimeSpan.Zero;

        if (EntradaManha.HasValue && SaidaManha.HasValue)
            total += SaidaManha.Value - EntradaManha.Value;

        if (EntradaTarde.HasValue && SaidaTarde.HasValue)
            total += SaidaTarde.Value - EntradaTarde.Value;

        return total;
    }

    public TimeSpan SaldoDiario(double jornadaHoras)
    {
        return TotalTrabalhado()
            - TimeSpan.FromHours(jornadaHoras);
    }

    public string ProximaAcao()
    {
        if (!EntradaManha.HasValue)
            return "Entrada da manhã";

        if (!SaidaManha.HasValue)
            return "Saída para almoço";

        if (!EntradaTarde.HasValue)
            return "Retorno do almoço";

        if (!SaidaTarde.HasValue)
            return "Saída do dia";

        return "Dia encerrado";
    }

    public void RegistrarBatida(TimeSpan horario)
    {
        if (!EntradaManha.HasValue)
        {
            EntradaManha = horario;
            return;
        }

        if (!SaidaManha.HasValue)
        {
            SaidaManha = horario;
            return;
        }

        if (!EntradaTarde.HasValue)
        {
            EntradaTarde = horario;
            return;
        }

        if (!SaidaTarde.HasValue)
        {
            SaidaTarde = horario;
            return;
        }
    }

    public string Status(double jornadaHoras = 8.5)
    {
        if (!DiaEncerrado())
            return "EM_ANDAMENTO";

        var saldo = SaldoDiario(jornadaHoras);

        if (saldo.TotalMinutes == 0)
            return "OK";

        if (saldo.TotalMinutes > 0)
            return "EXTRA";

        return "DEVEDOR";
    }

    public void DesfazerUltimaBatida()
    {
        if (SaidaTarde.HasValue)
        {
            SaidaTarde = null;
            return;
        }

        if (EntradaTarde.HasValue)
        {
            EntradaTarde = null;
            return;
        }

        if (SaidaManha.HasValue)
        {
            SaidaManha = null;
            return;
        }

        if (EntradaManha.HasValue)
        {
            EntradaManha = null;
            return;
        }
    }

    public string? ValidarHorarios(int intervaloMinimoMinutos = 60)
    {
        if (EntradaManha.HasValue &&
            SaidaManha.HasValue &&
            SaidaManha <= EntradaManha)
        {
            return "Saída almoço deve ser maior que Entrada manhã";
        }

        if (SaidaManha.HasValue &&
            EntradaTarde.HasValue &&
            EntradaTarde <= SaidaManha)
        {
            return "Retorno almoço deve ser maior que Saída almoço";
        }

        if (EntradaTarde.HasValue &&
            SaidaTarde.HasValue &&
            SaidaTarde <= EntradaTarde)
        {
            return "Saída dia deve ser maior que Retorno almoço";
        }

        if (SaidaManha.HasValue && EntradaTarde.HasValue)
        {
            var intervalo =
                EntradaTarde.Value - SaidaManha.Value;

            if (intervalo.TotalMinutes <
                intervaloMinimoMinutos)
            {
                return
                    $"Intervalo de almoço inferior ao mínimo configurado ({intervaloMinimoMinutos} min).";
            }
        }

        return null;
    }

    public string SaldoFormatado(double jornadaHoras)
    {
        var saldo = SaldoDiario(jornadaHoras);

        if (saldo == TimeSpan.Zero)
            return "00:00";

        var sinal = saldo < TimeSpan.Zero ? "-" : "+";

        saldo = saldo.Duration();

        return $"{sinal}{saldo.Hours:D2}:{saldo.Minutes:D2}";
    }

    public string TotalFormatado()
    {
        var total = TotalTrabalhado();

        return $"{(int)total.TotalHours:D2}:{total.Minutes:D2}";
    }

    public bool DiaEncerrado()
    {
        return SaidaTarde.HasValue;
    }
}