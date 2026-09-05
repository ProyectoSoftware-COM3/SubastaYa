namespace SubastaYa.Domain.Entities
{
   
    public class Wallet
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal HeldBalance { get; set; }
        public decimal AvailableBalance { get; set; }

       
        public byte[] Version { get; set; } = default!;

        public User User { get; set; } = default!;
        public ICollection<LedgerTransaction> Movements { get; set; } = new List<LedgerTransaction>();
    }
}
