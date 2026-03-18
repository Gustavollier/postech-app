namespace PosTechChallenge.Dominio.Model
{
    public class Status
    {
        public int Id { get; set; }
        public int IdOS { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int IdFuncionario { get; set; }
        public int StatusAtual { get; set; }
    }   
}
