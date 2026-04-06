namespace PosTechChallenge.Dominio.Model;
public sealed class Pecas
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Marca { get; set; }
    public string Codigo { get; set; }
    public string Preco { get; set; }
    public int UnidadeMedida { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
