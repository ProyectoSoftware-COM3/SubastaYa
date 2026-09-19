using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Queries.MyActivity.GetMyBids
{
    public record GetMyBidsQuery(Guid UserId) : IRequest<List<MyBidDto>>;
}