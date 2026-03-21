using Dapper;
using PosTechChallenge.Dominio.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PosTechChallenge.Infraestrutura.Mapeamentos
{
    public class PlacaDapper : SqlMapper.TypeHandler<Placa>
    {
        public override void SetValue(IDbDataParameter parameter, Placa value)
        {
            parameter.Value = value?.Valor;
        }

        public override Placa Parse(object value)
        {
            if (value == null || value is DBNull) return null!;

            return new Placa(value.ToString()!);
        }
    }
}
