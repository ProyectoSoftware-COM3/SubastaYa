using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SubastaYa.Application.Exceptions
{
    public class WalletNotFoundException : AppException
    {
        public WalletNotFoundException(Guid userId)
            : base($"No se encontro la billetera del usuario {userId}.") { }
    }
}
