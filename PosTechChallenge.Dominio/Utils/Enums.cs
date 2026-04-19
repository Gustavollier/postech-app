namespace PosTechChallenge.Dominio.Utils;
public class Enums
{
    public enum ECargoFuncionario
    {
        Mecanico = 0,
        Recepcionista = 1,
        Gerente = 2,
        Estoquista = 3,
        Eletricista = 4,
        Lavador = 5,
        Supervisor = 6
    }

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
