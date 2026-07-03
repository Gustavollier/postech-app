namespace PosTechChallenge.Aplicacao.Helpers;

/// <summary>
/// Helper centralizado para lógica de paginação.
/// Elimina duplicação e corrige bug crítico: TotalPages = hardcoded /10
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Valida parâmetros de paginação.
    /// </summary>
    /// <param name="page">Número da página (deve ser > 0)</param>
    /// <param name="pageSize">Tamanho da página (deve ser > 0)</param>
    /// <returns>Mensagem de erro, ou null se válido</returns>
    public static string? ValidatePaginationParams(int page, int pageSize)
    {
        if (page <= 0)
            return "Página deve ser maior que zero.";

        if (pageSize <= 0)
            return "Tamanho da página deve ser maior que zero.";

        return null;
    }

    /// <summary>
    /// Calcula o número total de páginas.
    ///
    /// ⚠️ BUG CORRIGIDO: Antes estava hardcoded como "totalItems / 10"
    /// Agora usa pageSize dinamicamente.
    ///
    /// Exemplo:
    /// - 105 itens, pageSize 10 = 11 páginas (antes retornava 10)
    /// - 100 itens, pageSize 10 = 10 páginas
    /// - 1 item, pageSize 10 = 1 página
    /// </summary>
    /// <param name="totalItems">Total de itens</param>
    /// <param name="pageSize">Itens por página</param>
    /// <returns>Número total de páginas</returns>
    public static int CalculateTotalPages(int totalItems, int pageSize)
    {
        if (totalItems == 0 || pageSize <= 0)
            return 0;

        // Fórmula correta: ceiling(totalItems / pageSize)
        // Exemplo: ceiling(105 / 10) = ceiling(10.5) = 11 ✅
        return (totalItems + pageSize - 1) / pageSize;
    }

    /// <summary>
    /// Calcula o OFFSET para query SQL (usada em OFFSET/FETCH).
    /// </summary>
    /// <param name="page">Número da página (1-based)</param>
    /// <param name="pageSize">Itens por página</param>
    /// <returns>Número de linhas a pular (0-based)</returns>
    public static int CalculateOffset(int page, int pageSize)
    {
        return (page - 1) * pageSize;
    }

    /// <summary>
    /// Valida se um número de página é válido para um conjunto paginado.
    /// </summary>
    /// <param name="page">Número da página requisitada</param>
    /// <param name="totalPages">Total de páginas disponíveis</param>
    /// <returns>True se página existe, false caso contrário</returns>
    public static bool IsValidPage(int page, int totalPages)
    {
        return page > 0 && page <= totalPages;
    }
}
