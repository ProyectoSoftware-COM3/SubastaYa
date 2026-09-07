using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Exceptions
{
    public class AuctionNotFoundException : AppException
    {
        public AuctionNotFoundException(Guid auctionId)
            : base($"No se encontro la subasta {auctionId}.") { }
    }
}

