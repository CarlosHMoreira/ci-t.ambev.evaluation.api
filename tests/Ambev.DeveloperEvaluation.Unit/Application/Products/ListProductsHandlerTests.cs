using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Products.Common;
namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class ListProductsHandlerTests
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ListProductsHandler _handler;

    public ListProductsHandlerTests()
    {
        _repository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListProductsHandler(_repository, _mapper);
    }

    [Fact(DisplayName = "Given paging and order When handling Then passes values to repository and returns mapped result")]
    public async Task Handle_PassesPagingOrderAndFilters()
    {
        var command = new ListProductsCommand
        {
            Page = 2,
            Size = 5,
            Order = "price desc",
            Filters = new Dictionary<string, string> { ["title"] = "phone" }
        };

        var productId = Guid.NewGuid();
        var products = new List<Product>
        {
            new() { Id = productId, Title = "Phone", Price = 10, Category = "Tech" }
        } as IEnumerable<Product>;

        _repository.ListAsync(2, 5, "price desc", Arg.Is<Dictionary<string,string>>(d => d.ContainsKey("title") && d["title"] == "phone"), Arg.Any<CancellationToken>())
            .Returns(callInfo => (products, 1));

        var mapped = new [] { new ProductResult { Id = productId, Title = "Phone" } };
        _mapper.Map<IEnumerable<ProductResult>>(products).Returns(mapped);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(5);
    }
}
