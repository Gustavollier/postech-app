using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Model;

public sealed class Orcamento
{
    public int Id { get; set; }
    public int IdOS { get; set; }
    public decimal ValorMaoDeObra { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal ValorTotal { get; set; }
    public EStatusOrcamento Status { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
