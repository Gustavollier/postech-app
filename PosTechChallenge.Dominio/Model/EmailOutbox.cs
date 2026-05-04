using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Model;

public sealed class EmailOutbox
{
    public int Id { get; set; }
    public string Destinatario { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public EStatusEmailOutbox Status { get; set; }
    public int Tentativas { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? ProcessadoEm { get; set; }
}
