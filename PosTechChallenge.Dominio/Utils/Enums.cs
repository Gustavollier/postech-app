namespace PosTechChallenge.Dominio.Utils;
public class Enums
{
    public enum StatusServico
    {
        Recebida = 0,
        EmDiagnostico = 1,
        AguardandoAprovacao = 2,
        EmExecucao = 3,
        Finalizada = 4,
        Entregue = 5
    }

    public enum EServico
    {
        MaoDeObra,
        Peca
    }
}
