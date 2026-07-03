using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Aplicacao.Mappers;

public static class ClienteMappingHelper
{
    public static ClienteDto MapEntityToDto(Cliente cliente)
    {
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));

        return new ClienteDto
        {
            Id = cliente.Id,
            CreatedAt = cliente.CreatedAt,
            UpdatedAt = cliente.UpdatedAt,
            CPF = cliente.CPF,
            CNPJ = cliente.CNPJ,
            NomeCompleto = cliente.NomeCompleto,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            Ativo = cliente.Ativo
        };
    }
}
