using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Infraestrutura.Querys
{
    internal class PecasQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, Nome, Marca, Codigo, Preco, UnidadeMedida, CriadoEm, AtualizadoEm FROM Pecas";

        public const string OBTER_POR_ID = "SELECT Id, Nome, Marca, Codigo, Preco, UnidadeMedida, CriadoEm, AtualizadoEm FROM Pecas WHERE Id = @Id";

        public const string CRIAR = @"
            INSERT INTO Pecas (Nome, Marca, Codigo, Preco, UnidadeMedida, CriadoEm, AtualizadoEm)
            VALUES (@Nome, @Marca, @Codigo, @Preco, @UnidadeMedida, @CriadoEm, @AtualizadoEm);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Pecas
            SET Nome = @Nome, Marca = @Marca, Codigo = @Codigo, Preco = @Preco, UnidadeMedida = @UnidadeMedida, CriadoEm = @CriadoEm, AtualizadoEm = @AtualizadoEm
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM Pecas WHERE Id = @Id";
    }
}
