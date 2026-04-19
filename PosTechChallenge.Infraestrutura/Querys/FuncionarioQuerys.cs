namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class FuncionarioQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionarios";

        public const string OBTER_POR_ID = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionarios WHERE Id = @Id";

        public const string OBTER_POR_CPF = "SELECT Id, Nome, Contato, CPF, Cargo, ValorHora FROM Funcionarios WHERE CPF = @CPF";

        public const string CRIAR = @"
            INSERT INTO Funcionarios (Nome, Contato, CPF, Cargo, ValorHora)
            VALUES (@Nome, @Contato, @CPF, @Cargo, @ValorHora);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Funcionarios
            SET Nome = @Nome, Contato = @Contato, CPF = @CPF, Cargo = @Cargo, ValorHora = @ValorHora
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM Funcionarios WHERE Id = @Id";
    }
}
