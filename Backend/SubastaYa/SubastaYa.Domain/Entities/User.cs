namespace SubastaYa.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public DateTime RegisteredAt { get; set; }

        public Wallet Wallet { get; set; } = default!;
        public ICollection<Auction> AuctionsCreated { get; set; } = new List<Auction>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
