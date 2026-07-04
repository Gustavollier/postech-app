using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.Helpers;

/// <summary>
/// Helper centralizado para validação de documentos (CPF/CNPJ).
/// Elimina duplicação de lógica entre Controllers.
/// </summary>
public static class DocumentValidationHelper
{
    /// <summary>
    /// Valida documento (CPF ou CNPJ).
    /// Regra: Deve ter CPF OU CNPJ, não ambos, não nenhum.
    /// </summary>
    /// <param name="cpf">CPF do cliente (opcional)</param>
    /// <param name="cnpj">CNPJ do cliente (opcional)</param>
    /// <returns>Mensagem de erro, ou null se válido</returns>
    public static string? ValidateDocument(string? cpf, string? cnpj)
    {
        var hasCpf = !string.IsNullOrWhiteSpace(cpf);
        var hasCnpj = !string.IsNullOrWhiteSpace(cnpj);

        // Regra de negócio: deve ter CPF ou CNPJ, não ambos, não nenhum
        if (!hasCpf && !hasCnpj)
            return "Informe CPF ou CNPJ.";

        if (hasCpf && hasCnpj)
            return "Informe apenas CPF ou CNPJ.";

        try
        {
            // Valida usando ValueObjects existentes
            if (hasCpf)
                _ = new CpfValueObject(cpf!);
            else
                _ = new CnpjValueObject(cnpj!);
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }

        return null;
    }

    /// <summary>
    /// Valida apenas CPF.
    /// </summary>
    /// <param name="cpf">CPF a validar</param>
    /// <returns>Mensagem de erro, ou null se válido</returns>
    public static string? ValidateCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return "CPF é obrigatório.";

        try
        {
            _ = new CpfValueObject(cpf);
            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }

    /// <summary>
    /// Valida apenas CNPJ.
    /// </summary>
    /// <param name="cnpj">CNPJ a validar</param>
    /// <returns>Mensagem de erro, ou null se válido</returns>
    public static string? ValidateCnpj(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return "CNPJ é obrigatório.";

        try
        {
            _ = new CnpjValueObject(cnpj);
            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}
