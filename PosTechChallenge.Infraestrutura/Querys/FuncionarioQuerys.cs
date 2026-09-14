namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class FuncionarioQuerys
    {
        // Toda leitura filtra Ativo = 1: um funcionario desativado some das
        // listagens, das buscas e do login, do mesmo jeito que um cliente
        // desativado. O registro continua no banco, com o historico intacto.
        public const string OBTER_TODOS = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE Ativo = 1";

        public const string OBTER_POR_NOME = @"SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE Ativo = 1 AND Nome LIKE '%' + @Nome + '%'";

        public const string OBTER_POR_ID = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE Id = @Id AND Ativo = 1";

        public const string OBTER_POR_CPF = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionario WHERE CPF = @CPF AND Ativo = 1";

        public const string CRIAR = @"
            INSERT INTO Funcionario (Nome, Contato, CPF, Cargo, ValorHora, Ativo)
            VALUES (@Nome, @Contato, @CPF, @Cargo, @ValorHora, 1);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Funcionario
            SET Nome = @Nome, Contato = @Contato, CPF = @CPF, Cargo = @Cargo, ValorHora = @ValorHora
            WHERE Id = @Id";

        // Desativacao, nao DELETE: Funcionario e referenciado por Seguranca,
        // OrdemServico, Itens e Status, nenhuma com cascade. Um DELETE de
        // verdade sempre esbarrava na FK do login, que nasce junto com o
        // cadastro — ou seja, a rota nunca chegava a funcionar.
        public const string DESATIVAR = "UPDATE Funcionario SET Ativo = 0 WHERE Id = @Id AND Ativo = 1";
    }
}
