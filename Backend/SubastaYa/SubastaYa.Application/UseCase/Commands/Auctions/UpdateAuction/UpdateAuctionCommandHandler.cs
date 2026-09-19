using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Domain.Enums;
using SubastaYa.Domain.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Auctions.UpdateAuction
{
    public class UpdateAuctionCommandHandler : IRequestHandler<UpdateAuctionCommand, Unit>
    {
        
        private static readonly TimeSpan EditWindowAfterStart = TimeSpan.FromMinutes(5);

        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAuctionCommandHandler(
            IAuctionRepository auctionRepository,
            IBidRepository bidRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateAuctionCommand request, CancellationToken cancellationToken)
        {
            var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken)
                ?? throw new AuctionNotFoundException(request.AuctionId);

           
            if (auction.SellerId != request.SellerId)
                throw new AuctionNotFoundException(request.AuctionId);

            await ValidateAuctionIsEditableAsync(auction, cancellationToken);

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new CategoryNotFoundException(request.CategoryId);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                auction.Title = request.Title;
                auction.Description = request.Description;
                auction.ImageUrl = request.ImageUrl;
                auction.CategoryId = category.Id;
                auction.BasePrice = request.BasePrice;
                auction.MinIncrement = request.MinIncrement;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task ValidateAuctionIsEditableAsync(Domain.Entities.Auction auction, CancellationToken ct)
        {
            
            var highestBid = await _bidRepository.GetHighestBidAsync(auction.Id, ct);
            if (highestBid is not null)
                throw new AuctionNotEditableException(auction.Id);

            var isScheduled = auction.Status == AuctionStatus.Scheduled;
            var isWithinEditWindow = auction.Status == AuctionStatus.Active
                && DateTime.UtcNow <= auction.StartDate.Add(EditWindowAfterStart);

            if (!isScheduled && !isWithinEditWindow)
                throw new AuctionNotEditableException(auction.Id);
        }
    }
}