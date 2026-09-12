using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Domain.Exceptions
{
    public class InvalidBidException : DomainException
    {
        public InvalidBidException(Guid auctionId, decimal offered, decimal minimumRequired)
            : base($"La oferta {offered} para la subasta {auctionId} no supera el minimo requerido de {minimumRequired}.") { }
    }
}
