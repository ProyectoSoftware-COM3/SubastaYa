namespace SubastaYa.Domain.Exceptions
{
    public class AuctionNotEditableException : DomainException
    {
        public AuctionNotEditableException(Guid auctionId)
            : base($"La subasta {auctionId} ya no se puede modificar: solo se edita antes de recibir ofertas y dentro de los primeros minutos desde su inicio.") { }
    }
}
