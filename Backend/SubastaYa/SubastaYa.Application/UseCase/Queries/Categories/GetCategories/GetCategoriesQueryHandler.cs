using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Queries.Categories.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
            => _categoryRepository = categoryRepository;

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return categories
                .Select(c => new CategoryDto(c.Id, c.Name, c.IconUrl))
                .ToList();
        }
    }
}

