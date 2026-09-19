using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Domain.Exceptions
{
    public class InsufficientBalanceException : DomainException
    {
        public InsufficientBalanceException(Guid userId, decimal requested, decimal available)
            : base($"No tenes saldo disponible suficiente para esta oferta. Solicitado: {requested}, disponible: {available}.") { }
    }
}
