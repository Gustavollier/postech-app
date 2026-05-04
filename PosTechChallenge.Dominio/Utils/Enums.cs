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

    public enum EStatusOrdemServico
    {
        Recebida = 0,
        EmDiagnostico = 1,
        AguardandoAprovacao = 2,
        EmExecucao = 3,
        Finalizada = 4,
        Entregue = 5,
        Cancelada = 6
    }

    public enum ETipoItemOrdemServico
    {
        MaoDeObra = 0,
        Peca = 1
    }

    public enum EStatusOrcamento
    {
        Pendente = 0,
        Aprovado = 1,
        Rejeitado = 2
    }

    public enum EStatusEmailOutbox
    {
        Pendente = 0,
        Processado = 1,
        Falha = 2
    }
}
