namespace PosTechChallenge.Dominio.Model;
public sealed class ItemOS
{
    public int Id { get; set; }
    public int IdOS { get; set; }
    public int TipoItem { get; set; }
    public int QuantidadeItem { get; set; }
    public int IdFuncionario { get; set; }
    public int IdPeca { get; set; }
}
