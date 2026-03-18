using static PosTechChallenge.Domain.Utils.Enums;

namespace PosTechChallenge.Domain.Model;
public sealed class OrdemServico
{
    public int Id { get; set; }
    public int IdCliente { get; set; }
    public int IdVeiculo { get; set; }
    public int IdFuncionario { get; set; }
    public StatusServico Status { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
