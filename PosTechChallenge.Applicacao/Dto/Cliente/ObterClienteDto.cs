using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Aplicacao.Dto.Cliente
{
    public class ObterClienteDto
    {
        public IEnumerable<ClienteDto> Items { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
    }
}
