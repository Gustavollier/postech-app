namespace PosTechChallenge.Dominio.Model
{
    public class SegurancaFuncionario
    {
        public int Id { get; set; }
        public int FuncionarioId { get; set; }
        public string SenhaHash { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
