using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.UseCase.Queries.MyActivity.GetMyAuctions
{
    public class GetMyAuctionsQueryHandler : IRequestHandler<GetMyAuctionsQuery, List<MyAuctionDto>>
    {
        private readonly IAuctionRepository _auctionRepository;

        public GetMyAuctionsQueryHandler(IAuctionRepository auctionRepository)
            => _auctionRepository = auctionRepository;

        public async Task<List<MyAuctionDto>> Handle(GetMyAuctionsQuery request, CancellationToken cancellationToken)
        {
            var myAuctions = await _auctionRepository.GetBySellerIdAsync(request.UserId, cancellationToken);

            return myAuctions

                .OrderBy(a => GetStatusPriority(a.Status))
                .ThenByDescending(a => a.StartDate)
                .Select(a =>
                {
                    var currentPrice = a.Bids.Count > 0 ? a.Bids.Max(b => b.Amount) : a.BasePrice;
                    var revenue = a.Status == AuctionStatus.Finished ? currentPrice : 0;
                    return new MyAuctionDto(a.Id, a.Title, a.Status, revenue, a.Bids.Count);
                })
                .ToList();
        }
        private static int GetStatusPriority(AuctionStatus status) => status switch
        {
            AuctionStatus.Active => 0,
            AuctionStatus.Scheduled => 1,
            AuctionStatus.Finished => 2,
            _ => 3
        };

    }
}

