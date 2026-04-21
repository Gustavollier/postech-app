using PosTechChallenge.Aplicacao.Dto.Veiculo;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class VeiculoService : IVeiculoService
{
    private readonly IVeiculosRepositorio _veiculosRepositorio;

    public VeiculoService(IVeiculosRepositorio veiculosRepositorio)
    {
        _veiculosRepositorio = veiculosRepositorio;
    }

    public async Task<Resultado> CriarAsync(CriarVeiculoDto veiculoDto)
    {
        try
        {
            var veiculo = new Veiculo
            {
                ClienteId = veiculoDto.ClienteId,
                Marca = veiculoDto.Marca,
                Modelo = veiculoDto.Modelo,
                Placa = veiculoDto.Placa.ToUpperInvariant(),
                Cor = veiculoDto.Cor,
                AnoModelo = veiculoDto.AnoModelo,
                AnoFabricacao = veiculoDto.AnoFabricacao,
                KmEntrada = veiculoDto.KmEntrada,
                Ativo = true
            };

            await _veiculosRepositorio.CriarAsync(veiculo);
            return Resultado.Sucesso("Veículo cadastrado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterVeiculoDto>> ObterPorIdAsync(int id)
    {
        try
        {
            var veiculo = await _veiculosRepositorio.ObterPorIdAsync(id);

            if (veiculo == null)
                return Resultado<ObterVeiculoDto>.Falha($"Veículo com ID {id} não encontrado.");

            return Resultado<ObterVeiculoDto>.Sucesso(MapearParaDto(veiculo));
        }
        catch (Exception ex)
        {
            return Resultado<ObterVeiculoDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterVeiculoDto>> ObterPorPlacaAsync(string placa)
    {
        try
        {
            var veiculo = await _veiculosRepositorio.ObterPorPlacaAsync(placa.ToUpperInvariant());

            if (veiculo == null)
                return Resultado<ObterVeiculoDto>.Falha($"Veículo com placa {placa} não encontrado.");

            return Resultado<ObterVeiculoDto>.Sucesso(MapearParaDto(veiculo));
        }
        catch (Exception ex)
        {
            return Resultado<ObterVeiculoDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<IEnumerable<ObterVeiculoDto>>> ObterPorClienteIdAsync(int clienteId)
    {
        try
        {
            var veiculos = await _veiculosRepositorio.ObterPorClienteIdAsync(clienteId);

            if (veiculos == null || !veiculos.Any())
                return Resultado<IEnumerable<ObterVeiculoDto>>.Falha($"Nenhum veículo encontrado para o cliente {clienteId}.");

            var dtos = veiculos.Select(MapearParaDto).ToList();
            return Resultado<IEnumerable<ObterVeiculoDto>>.Sucesso(dtos);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterVeiculoDto>>.Falha(ex.Message);
        }
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarVeiculoDto veiculoDto)
    {
        try
        {
            var veiculoExistente = await _veiculosRepositorio.ObterPorIdAsync(id);

            if (veiculoExistente == null)
                return Resultado.Falha($"Veículo com ID {id} não encontrado.");

            veiculoExistente.ClienteId = veiculoDto.ClienteId;
            veiculoExistente.Marca = veiculoDto.Marca;
            veiculoExistente.Modelo = veiculoDto.Modelo;
            veiculoExistente.Placa = veiculoDto.Placa.ToUpperInvariant();
            veiculoExistente.Cor = veiculoDto.Cor;
            veiculoExistente.AnoModelo = veiculoDto.AnoModelo;
            veiculoExistente.AnoFabricacao = veiculoDto.AnoFabricacao;
            veiculoExistente.KmEntrada = veiculoDto.KmEntrada;
            veiculoExistente.Ativo = true;

            var atualizado = await _veiculosRepositorio.AtualizarAsync(veiculoExistente);

            return atualizado
                ? Resultado.Sucesso("Veículo atualizado com sucesso.")
                : Resultado.Falha("Não foi possível atualizar o veículo.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    private static ObterVeiculoDto MapearParaDto(Veiculo veiculo)
    {
        return new ObterVeiculoDto
        {
            Id = veiculo.Id,
            ClienteId = veiculo.ClienteId,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            Placa = veiculo.Placa,
            Cor = veiculo.Cor,
            AnoModelo = veiculo.AnoModelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            KmEntrada = veiculo.KmEntrada,
            Ativo = veiculo.Ativo
        };
    }
}