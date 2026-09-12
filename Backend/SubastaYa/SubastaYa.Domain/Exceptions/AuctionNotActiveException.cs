using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Domain.Exceptions
{
    public class AuctionNotActiveException : DomainException
    {
        public AuctionNotActiveException(Guid auctionId)
            : base($"La subasta {auctionId} no esta activa o ya cerro.") { }
    }
}
