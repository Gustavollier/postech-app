using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Model;
public sealed class ItemOrdemServico
{
    public int Id { get; set; }
    public int IdOrdemServico { get; set; }
    public EServico Tipo { get; set; }
    public int IdFuncionario { get; set; }
    public int IdPeca { get; set; }
}
