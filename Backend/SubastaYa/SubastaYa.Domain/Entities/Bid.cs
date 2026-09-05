namespace SubastaYa.Domain.Entities
{
    public class Bid
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PlacedAt { get; set; }

        public Auction Auction { get; set; } = default!;
        public User Bidder { get; set; } = default!;
    }
}
