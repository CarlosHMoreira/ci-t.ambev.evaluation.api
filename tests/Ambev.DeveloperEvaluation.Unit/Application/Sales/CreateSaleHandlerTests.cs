using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateSaleItemDto, SaleItem>();
            cfg.CreateMap<CreateSaleCommand, Sale>();
            cfg.CreateMap<SaleItem, SaleItemResult>();
            cfg.CreateMap<Sale, SaleResult>();
        });
        _mapper = cfg.CreateMapper();
        _saleService = new SaleService(_saleRepository, _dispatcher);
        _handler = new CreateSaleHandler(_saleService, _mapper);
    }

    [Fact(DisplayName = "Given valid sale When creating Then calculates discounts and publishes event")]
    public async Task Handle_CreatesSale_WithDiscounts()
    {
        var command = new CreateSaleCommand
        {
            Number = "S0001",
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemDto>
            {
                new() { ProductId = Guid.NewGuid(), ProductTitle = "Item A", Quantity = 5, UnitPrice = 10 }, // 10% discount
                new() { ProductId = Guid.NewGuid(), ProductTitle = "Item B", Quantity = 2, UnitPrice = 20 }  // no discount
            }
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Sale>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(5 * 10 * 0.9m + 2 * 20); // 45 + 40 = 85
        result.Items.Should().HaveCount(2);
        _dispatcher.Received(1).DispatchAsync(Arg.Any<Ambev.DeveloperEvaluation.Domain.Events.Sale.SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}
