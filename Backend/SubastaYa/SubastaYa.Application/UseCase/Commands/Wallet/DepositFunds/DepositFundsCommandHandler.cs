using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds
{
    public class DepositFundsCommandHandler : IRequestHandler<DepositFundsCommand, WalletBalanceDto>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepositFundsCommandHandler(
            IWalletRepository walletRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WalletBalanceDto> Handle(DepositFundsCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                    ?? throw new WalletNotFoundException(request.UserId);

                wallet.TotalBalance += request.Amount;
                wallet.AvailableBalance = wallet.TotalBalance - wallet.HeldBalance;

                await _walletRepository.AddMovementAsync(new LedgerTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Type = LedgerTransactionType.Deposit,
                    Amount = request.Amount,
                    OccurredAt = DateTime.UtcNow
                }, cancellationToken);

                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityType = AuditEntityType.Wallet,
                    EntityId = wallet.Id.ToString(),
                    Action = "AcreditacionManual",
                    UserId = request.UserId,
                    DetailsJson = $"{{\"monto\":{request.Amount}}}",
                    OccurredAt = DateTime.UtcNow
                }, cancellationToken);

                return new WalletBalanceDto(wallet.TotalBalance, wallet.HeldBalance, wallet.AvailableBalance);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

