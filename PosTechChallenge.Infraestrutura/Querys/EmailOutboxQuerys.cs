namespace PosTechChallenge.Infraestrutura.Querys;

public static class EmailOutboxQuerys
{
    public const string CRIAR = @"
        INSERT INTO EmailOutbox (Destinatario, Assunto, Corpo, Status, Tentativas, CriadoEm, ProcessadoEm)
        VALUES (@Destinatario, @Assunto, @Corpo, @Status, @Tentativas, @CriadoEm, @ProcessadoEm);
        SELECT CAST(SCOPE_IDENTITY() as int);";
}
