using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class ClienteService : IClienteService
{
    private readonly IClienteRepositorio _clienteRepositorio;

    public ClienteService(IClienteRepositorio clienteRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<Resultado> CriarAsync(CriarClienteDto clienteDto)
    {
        try
        {
            var cliente = new Cliente
            {
                NomeCompleto = clienteDto.NomeCompleto,
                CPF = string.IsNullOrWhiteSpace(clienteDto.CPF) ? null : clienteDto.CPF,
                CNPJ = string.IsNullOrWhiteSpace(clienteDto.CNPJ) ? null : clienteDto.CNPJ,
                Telefone = clienteDto.Telefone,
                Email = clienteDto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Ativo = true
            };

            await _clienteRepositorio.CriarAsync(cliente);
            return Resultado.Sucesso("Cliente criado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterClienteDto>> ObterPorIdAsync(int id)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (cliente == null)
                return Resultado<ObterClienteDto>.Falha($"Cliente com ID {id} não encontrado.");

            return Resultado<ObterClienteDto>.Sucesso(MapearParaDto(cliente));
        }
        catch (Exception ex)
        {
            return Resultado<ObterClienteDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorCpfCnpjAsync(cpfCnpj);

            if (cliente == null)
                return Resultado<ObterClienteDto>.Falha($"Cliente com CPF/CNPJ {cpfCnpj} não encontrado.");

            return Resultado<ObterClienteDto>.Sucesso(MapearParaDto(cliente));
        }
        catch (Exception ex)
        {
            return Resultado<ObterClienteDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<IEnumerable<ObterClienteDto>>> ObterTodosAsync()
    {
        try
        {
            var clientes = await _clienteRepositorio.ObterTodosAsync();

            if (clientes == null || !clientes.Any())
                return Resultado<IEnumerable<ObterClienteDto>>.Falha("Nenhum cliente encontrado.");

            var dtos = clientes.Select(MapearParaDto).ToList();
            return Resultado<IEnumerable<ObterClienteDto>>.Sucesso(dtos);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterClienteDto>>.Falha(ex.Message);
        }
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto)
    {
        try
        {
            var clienteExistente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (clienteExistente == null)
                return Resultado.Falha($"Cliente com ID {id} não encontrado.");

            clienteExistente.NomeCompleto = clienteDto.NomeCompleto;
            clienteExistente.CPF = string.IsNullOrWhiteSpace(clienteDto.CPF) ? null : clienteDto.CPF;
            clienteExistente.CNPJ = string.IsNullOrWhiteSpace(clienteDto.CNPJ) ? null : clienteDto.CNPJ;
            clienteExistente.Telefone = clienteDto.Telefone;
            clienteExistente.Email = clienteDto.Email;
            clienteExistente.UpdatedAt = DateTime.UtcNow;
            clienteExistente.Ativo = true;

            var atualizado = await _clienteRepositorio.AtualizarAsync(clienteExistente);

            return atualizado
                ? Resultado.Sucesso("Cliente atualizado com sucesso.")
                : Resultado.Falha("Não foi possível atualizar o cliente.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    public async Task<Resultado> DesativarAsync(int id)
    {
        try
        {
            var clienteExistente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (clienteExistente == null)
                return Resultado.Falha($"Cliente com ID {id} não encontrado.");

            var desativado = await _clienteRepositorio.DesativarAsync(id);

            return desativado
                ? Resultado.Sucesso("Cliente desativado com sucesso.")
                : Resultado.Falha("Não foi possível desativar o cliente.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    private static ObterClienteDto MapearParaDto(Cliente cliente)
    {
        return new ObterClienteDto
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