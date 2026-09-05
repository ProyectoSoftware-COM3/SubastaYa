namespace SubastaYa.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string IconUrl { get; set; } = default!;

        public ICollection<Auction> Auctions { get; set; } = new List<Auction>();
    }
}
