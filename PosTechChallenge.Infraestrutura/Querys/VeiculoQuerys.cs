using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Infraestrutura.Querys
{
    public static class VeiculoQuerys
    {
        public const string OBTER_TODOS = @"
            SELECT Id, ClienteId, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada, Ativo
            FROM Veiculo
            WHERE Ativo = 1";

        public const string OBTER_POR_ID = @"
            SELECT Id, ClienteId, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada, Ativo
            FROM Veiculo
            WHERE Id = @Id AND Ativo = 1";

        public const string OBTER_POR_PLACA = @"
            SELECT Id, ClienteId, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada, Ativo
            FROM Veiculo
            WHERE Placa = @Placa AND Ativo = 1";

        public const string OBTER_POR_CLIENTE_ID = @"
            SELECT Id, ClienteId, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada, Ativo
            FROM Veiculo
            WHERE ClienteId = @ClienteId AND Ativo = 1";

        public const string CRIAR = @"
            INSERT INTO Veiculo (ClienteId, Marca, Modelo, Placa, Cor, AnoModelo, AnoFabricacao, KmEntrada, Ativo)
            VALUES (@ClienteId, @Marca, @Modelo, @Placa, @Cor, @AnoModelo, @AnoFabricacao, @KmEntrada, @Ativo);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string ATUALIZAR = @"
            UPDATE Veiculo
            SET ClienteId = @ClienteId,
                Marca = @Marca,
                Modelo = @Modelo,
                Placa = @Placa,
                Cor = @Cor,
                AnoModelo = @AnoModelo,
                AnoFabricacao = @AnoFabricacao,
                KmEntrada = @KmEntrada,
                Ativo = @Ativo
            WHERE Id = @Id";

        public const string DELETAR = "DELETE FROM Veiculo WHERE Id = @Id";
    }
}
