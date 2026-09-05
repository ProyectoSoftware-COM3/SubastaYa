using SubastaYa.Domain.Enums;

namespace SubastaYa.Domain.Entities
{
    public class LedgerTransaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public LedgerTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid? AuctionId { get; set; }

        public Wallet Wallet { get; set; } = default!;
    }
}
