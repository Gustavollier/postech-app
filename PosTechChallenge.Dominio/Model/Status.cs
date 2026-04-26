using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Model
{
    public class Status
    {
        public int Id { get; set; }
        public int IdOS { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int IdFuncionario { get; set; }
        public EStatusOrdemServico StatusAtual { get; set; }
    }   
}
