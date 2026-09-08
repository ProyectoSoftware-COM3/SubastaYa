using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction
{
    public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAuctionCommandHandler(
            IAuctionRepository auctionRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _auctionRepository = auctionRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
                ?? throw new CategoryNotFoundException(request.CategoryId);

            var status = request.StartDate <= DateTime.UtcNow ? AuctionStatus.Active : AuctionStatus.Scheduled;

            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = request.SellerId,
                CategoryId = category.Id,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                BasePrice = request.BasePrice,
                MinIncrement = request.MinIncrement,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = status
            };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _auctionRepository.AddAsync(auction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return auction.Id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}