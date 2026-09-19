using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.DTOs
{
    public record WalletMovementDto(Guid Id, LedgerTransactionType Type, decimal Amount, DateTime OccurredAt, Guid? AuctionId);
}
