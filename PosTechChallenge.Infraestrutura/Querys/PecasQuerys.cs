using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Infraestrutura.Querys
{
    internal class PecasQuerys
    {
        public const string OBTER_TODOS = @"
            SELECT Id, Nome, Marca, Codigo, Preco, UnidadeMedida, QuantidadeEstoque, CriadoEm, AtualizadoEm, Ativo
            FROM Pecas
            WHERE Ativo = 1";

        public const string OBTER_POR_ID = @"
            SELECT Id, Nome, Marca, Codigo, Preco, UnidadeMedida, QuantidadeEstoque, CriadoEm, AtualizadoEm, Ativo
            FROM Pecas
            WHERE Id = @Id AND Ativo = 1";

        public const string CRIAR = @"
            INSERT INTO Pecas (Nome, Marca, Codigo, Preco, UnidadeMedida, QuantidadeEstoque, CriadoEm, AtualizadoEm, Ativo)
            VALUES (@Nome, @Marca, @Codigo, @Preco, @UnidadeMedida, @QuantidadeEstoque, @CriadoEm, @AtualizadoEm, @Ativo);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Pecas
            SET Nome = @Nome,
                Marca = @Marca,
                Codigo = @Codigo,
                Preco = @Preco,
                UnidadeMedida = @UnidadeMedida,
                QuantidadeEstoque = @QuantidadeEstoque,
                AtualizadoEm = @AtualizadoEm,
                Ativo = @Ativo
            WHERE Id = @Id";

        public const string AJUSTAR_ESTOQUE = @"
            UPDATE Pecas
            SET QuantidadeEstoque = @QuantidadeEstoque,
                AtualizadoEm = @AtualizadoEm
            WHERE Id = @Id AND Ativo = 1";

        public const string DELETAR = @"
            UPDATE Pecas
            SET Ativo = 0,
                AtualizadoEm = @AtualizadoEm
            WHERE Id = @Id";
    }
}
