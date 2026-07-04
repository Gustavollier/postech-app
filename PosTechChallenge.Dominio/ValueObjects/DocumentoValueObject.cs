namespace PosTechChallenge.Dominio.ValueObjects;

public sealed class DocumentoValueObject
{
    public enum TipoDocumento { CPF, CNPJ }

    public string Valor { get; }
    public TipoDocumento Tipo { get; }
    public bool IsCpf => Tipo == TipoDocumento.CPF;
    public bool IsCnpj => Tipo == TipoDocumento.CNPJ;

    private DocumentoValueObject(string valor, TipoDocumento tipo)
    {
        Valor = valor;
        Tipo = tipo;
    }

    public static DocumentoValueObject CriarComCpf(string cpf)
    {
        var vo = new CpfValueObject(cpf);
        return new DocumentoValueObject(vo.Valor, TipoDocumento.CPF);
    }

    public static DocumentoValueObject CriarComCnpj(string cnpj)
    {
        var vo = new CnpjValueObject(cnpj);
        return new DocumentoValueObject(vo.Valor, TipoDocumento.CNPJ);
    }

    public static DocumentoValueObject Criar(string? cpf, string? cnpj)
    {
        var hasCpf = !string.IsNullOrWhiteSpace(cpf);
        var hasCnpj = !string.IsNullOrWhiteSpace(cnpj);

        if (!hasCpf && !hasCnpj)
            throw new ArgumentException("CPF ou CNPJ é obrigatório.");

        if (hasCpf && hasCnpj)
            throw new ArgumentException("Informe apenas CPF ou CNPJ.");

        return hasCpf ? CriarComCpf(cpf!) : CriarComCnpj(cnpj!);
    }

    public string? ObterCpf() => IsCpf ? Valor : null;
    public string? ObterCnpj() => IsCnpj ? Valor : null;

    public override bool Equals(object? obj) =>
        obj is DocumentoValueObject other && Valor == other.Valor && Tipo == other.Tipo;

    public override int GetHashCode() => HashCode.Combine(Valor, Tipo);

    public static bool operator ==(DocumentoValueObject? a, DocumentoValueObject? b) =>
        a?.Equals(b) ?? b is null;

    public static bool operator !=(DocumentoValueObject? a, DocumentoValueObject? b) => !(a == b);

    public override string ToString() => $"{Tipo}: {Valor}";
}
