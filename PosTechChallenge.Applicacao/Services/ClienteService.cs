using Microsoft.Extensions.Logging;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Helpers;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Mappers;
using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class ClienteService : IClienteService
{
    private const string MensagemErroInterno = "Erro interno ao processar cliente.";

    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IClienteRepositorio clienteRepositorio,
        IUnitOfWork unitOfWork,
        ILogger<ClienteService> logger)
    {
        _clienteRepositorio = clienteRepositorio;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Resultado> CriarAsync(CriarClienteDto clienteDto, CancellationToken cancellationToken = default)
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

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _clienteRepositorio.CriarAsync(cliente, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Resultado.Sucesso("Cliente criado com sucesso.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erro ao criar cliente.");
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ClienteDto>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id, cancellationToken);

            if (cliente == null)
                return Resultado<ClienteDto>.Falha($"Cliente com ID {id} não encontrado.");

            return Resultado<ClienteDto>.Sucesso(ClienteMappingHelper.MapEntityToDto(cliente));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por ID {ClienteId}.", id);
            return Resultado<ClienteDto>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ClienteDto>> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        try
        {
            var cliente = await _clienteRepositorio.ObterPorCpfCnpjAsync(cpfCnpj, cancellationToken);

            if (cliente == null)
                return Resultado<ClienteDto>.Falha($"Cliente com CPF/CNPJ {cpfCnpj} não encontrado.");

            return Resultado<ClienteDto>.Sucesso(ClienteMappingHelper.MapEntityToDto(cliente));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por CPF/CNPJ.");
            return Resultado<ClienteDto>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<ObterClienteDto>> ObterTodosAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            // Materializa uma única vez para evitar múltiplas enumerações da sequência.
            var clientes = (await _clienteRepositorio.ObterTodosAsync(page, pageSize, cancellationToken)).ToList();

            int quantidadeClientes = await _clienteRepositorio.ObterQuantidadeClientesAsync(cancellationToken);

            if (quantidadeClientes is 0 || clientes.Count == 0)
                return Resultado<ObterClienteDto>.Falha("Nenhum cliente encontrado.");

            IEnumerable<ClienteDto> dtos = clientes.Select(ClienteMappingHelper.MapEntityToDto).ToList();

            var response = new ObterClienteDto
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = clientes.Count,
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

    public async Task<Resultado> AtualizarAsync(int id, AtualizarClienteDto clienteDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var clienteExistente = await _clienteRepositorio.ObterPorIdAsync(id, cancellationToken);

            if (clienteExistente == null)
                return Resultado.Falha($"Cliente com ID {id} não encontrado.");

            clienteExistente.NomeCompleto = clienteDto.NomeCompleto;
            clienteExistente.CPF = clienteDto.CPF;
            clienteExistente.CNPJ = clienteDto.CNPJ;
            clienteExistente.Telefone = clienteDto.Telefone;
            clienteExistente.Email = clienteDto.Email;
            clienteExistente.UpdatedAt = DateTime.UtcNow;
            clienteExistente.Ativo = true;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var atualizado = await _clienteRepositorio.AtualizarAsync(clienteExistente, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return atualizado
                ? Resultado.Sucesso("Cliente atualizado com sucesso.")
                : Resultado.Falha("Não foi possível atualizar o cliente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erro ao atualizar cliente {ClienteId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado> DesativarAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var clienteExistente = await _clienteRepositorio.ObterPorIdAsync(id, cancellationToken);

            if (clienteExistente == null)
                return Resultado.Falha($"Cliente com ID {id} não encontrado.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var desativado = await _clienteRepositorio.DesativarAsync(id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return desativado
                ? Resultado.Sucesso("Cliente desativado com sucesso.")
                : Resultado.Falha("Não foi possível desativar o cliente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(CancellationToken.None);
            _logger.LogError(ex, "Erro ao desativar cliente {ClienteId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }
}
