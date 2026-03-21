using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class VeiculoQuerys
    {
        public const string OBTER_TODOS = "SELECT Id, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada FROM Veiculos";

        public const string OBTER_POR_ID = "SELECT Id, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada FROM Veiculos where Id = @Id";

        public const string CRIAR = @"
            INSERT INTO Veiculos (Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada)
            VALUES (@Marca, @Modelo, @Placa, @Cor, @AnoModelo, @AnoFabricacao, @KmEntrada);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Veiculos
            SET Marca = @Marca, Modelo = @Modelo, Placa = @Placa, Cor = @Cor, AnoModelo = @AnoModelo, AnoFabricacao = @AnoFabricacao, KmEntrada = @KmEntrada
            WHERE Id = @Id""";

        public const string DELETAR = "DELETE FROM Veiculos WHERE Id = @Id";
    }
}
