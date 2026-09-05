using SubastaYa.Domain.Enums;

namespace SubastaYa.Domain.Entities
{
    public class Auction
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public decimal BasePrice { get; set; }
        public decimal MinIncrement { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AuctionStatus Status { get; set; }

       
        public byte[] Version { get; set; } = default!;

        public User Seller { get; set; } = default!;
        public Category Category { get; set; } = default!;
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
