using Microsoft.Extensions.Logging;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Helpers;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Mappers;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class ClienteService : IClienteService
{
    private const string MensagemErroInterno = "Erro interno ao processar cliente.";

    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IClienteRepositorio clienteRepositorio,
        ILogger<ClienteService> logger)
    {
        _clienteRepositorio = clienteRepositorio;
        _logger = logger;
    }

    public async Task<Resultado> CriarAsync(CriarClienteDto clienteDto)
    {
        try
        {
            // FASE 2.1: Remover validações de null/whitespace
            // Nota: Validação é responsabilidade do Controller
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
            _logger.LogError(ex, "Erro ao criar cliente.");
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ClienteDto>> ObterPorIdAsync(int id)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (cliente == null)
                return Resultado<ClienteDto>.Falha($"Cliente com ID {id} não encontrado.");

            // FASE 3.2: Usar ClienteMappingHelper ao invés de método privado
            return Resultado<ClienteDto>.Sucesso(ClienteMappingHelper.MapEntityToDto(cliente));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por ID {ClienteId}.", id);
            return Resultado<ClienteDto>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorCpfCnpjAsync(cpfCnpj);

            if (cliente == null)
                return Resultado<ClienteDto>.Falha($"Cliente com CPF/CNPJ {cpfCnpj} não encontrado.");

            // FASE 3.2: Usar ClienteMappingHelper ao invés de método privado
            return Resultado<ClienteDto>.Sucesso(ClienteMappingHelper.MapEntityToDto(cliente));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por CPF/CNPJ.");
            return Resultado<ClienteDto>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ObterClienteDto>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var clientes = await _clienteRepositorio.ObterTodosAsync(page, pageSize);

            int quantidadeClientes = await _clienteRepositorio.ObterQuantidadeClientesAsync();

            if (quantidadeClientes is 0 || clientes == null || clientes.Any() is false)
                return Resultado<ObterClienteDto>.Falha("Nenhum cliente encontrado.");

            // FASE 3.2: Usar ClienteMappingHelper ao invés de método privado
            IEnumerable<ClienteDto> dtos = clientes.Select(ClienteMappingHelper.MapEntityToDto).ToList();

            // FASE 2.2: CORRIGIDO - Bug crítico de paginação
            // ANTES: TotalPages = quantidadeClientes / 10 (hardcoded, sempre /10) ❌
            // DEPOIS: TotalPages = ceiling(quantidadeClientes / pageSize) ✅
            // Exemplo: 105 clientes, pageSize 10 = 11 páginas (não 10)
            var response = new ObterClienteDto
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = clientes.Count(),
                TotalPages = PaginationHelper.CalculateTotalPages(quantidadeClientes, pageSize)
            };

            return Resultado<ObterClienteDto>.Sucesso(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter clientes paginados. Page {Page}, PageSize {PageSize}.", page, pageSize);
            return Resultado<ObterClienteDto>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto)
    {
        try
        {
            var clienteExistente = await _clienteRepositorio.ObterPorIdAsync(id);

            if (clienteExistente == null)
                return Resultado.Falha($"Cliente com ID {id} não encontrado.");

            // FASE 2.1: Remover validações de null/whitespace
            // Nota: Validação é responsabilidade do Controller
            clienteExistente.NomeCompleto = clienteDto.NomeCompleto;
            clienteExistente.CPF = clienteDto.CPF;  // Já validado no Controller
            clienteExistente.CNPJ = clienteDto.CNPJ;  // Já validado no Controller
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
            _logger.LogError(ex, "Erro ao atualizar cliente {ClienteId}.", id);
            return Resultado.Falha(MensagemErroInterno);
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
            _logger.LogError(ex, "Erro ao desativar cliente {ClienteId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    private static ClienteDto MapearParaClienteDto(Cliente cliente)
    {
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
