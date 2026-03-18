namespace PosTechChallenge.Domain.Model
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Placa { get; set; }
        public string? Cor { get; set; }
        public string? AnoModelo { get; set; }
        public string? AnoFabricacao { get; set; }
        public string? KmEntrada { get; set; }
    }
}
