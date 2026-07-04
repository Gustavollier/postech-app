namespace PosTechChallenge.Aplicacao.Dto.Common;

/// <summary>
/// DTO genérico para respostas paginadas.
/// Reutilizável para Cliente, Funcionário, Veículo, etc.
///
/// Elimina duplicação de DTOs específicos como ObterClienteDto,
/// mantendo uma estrutura consistente em toda a aplicação.
///
/// Exemplo de uso:
/// var response = new PaginatedResponseDto{T}
/// {
///     Items = clientes,
///     Page = 1,
///     PageSize = 10,
///     TotalItems = 105,
///     TotalPages = 11
/// };
/// </summary>
/// <typeparam name="T">Tipo de item paginado (Cliente, Funcionário, etc.)</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>
    /// Itens da página atual.
    /// </summary>
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// Número da página atual (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Quantidade de itens por página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens em todas as páginas.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total de páginas disponíveis.
    /// 📌 CRÍTICO: Calculado com PaginationHelper.CalculateTotalPages()
    /// Antes: hardcoded = TotalItems / 10 ❌
    /// Depois: TotalPages = ceiling(TotalItems / PageSize) ✅
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// DTO genérico não-genérico para casos onde tipo é desconhecido.
/// Use quando T não pode ser especificado em tempo de compilação.
/// </summary>
public class PaginatedResponseDto
{
    public IEnumerable<object>? Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
