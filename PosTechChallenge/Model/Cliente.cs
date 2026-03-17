namespace PosTechChallenge.Model
{
    public class Cliente
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? CPF { get; set; }        

        public string? CNPJ { get; set; }

        public string? NomeCompleto { get; set; }

        public string? Telefone { get; set; }

        public string? Email { get; set; }
    }
}
    