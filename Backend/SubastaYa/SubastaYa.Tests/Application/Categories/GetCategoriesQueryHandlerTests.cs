using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.Categories.GetCategories;
using SubastaYa.Domain.Entities;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetCategoriesQueryHandlerTests
    {
        private class FakeCategoryRepository : ICategoryRepository
        {
            public List<Category> Categories = new();
            public Task<List<Category>> GetAllAsync(CancellationToken ct = default) => Task.FromResult(Categories);
            public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
        }

        
        [Fact]
        public async Task Handle_ReturnsCategoriesMappedToDto()
        {
            var repository = new FakeCategoryRepository
            {
                Categories = { new Category { Id = Guid.NewGuid(), Name = "Electronics", IconUrl = "icon-electro.svg" } }
            };
            var handler = new GetCategoriesQueryHandler(repository);

            var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Electronics", result[0].Name);
            Assert.Equal("icon-electro.svg", result[0].IconUrl);
        }

        
        [Fact]
        public async Task Handle_NoCategoriesLoaded_ReturnsEmptyList()
        {
            var handler = new GetCategoriesQueryHandler(new FakeCategoryRepository());

            var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            Assert.Empty(result);
        }
    }
}
