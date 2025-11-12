using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SaleItem, SaleItemResult>();
            cfg.CreateMap<Sale, SaleResult>();
        });
        _mapper = cfg.CreateMapper();
        
        _saleService = new SaleService(_saleRepository, _productRepository, _userRepository, _dispatcher);
        _handler = new GetSaleHandler(_saleService, _mapper);
    }

    [Fact(DisplayName = "Given valid sale id When getting sale Then returns sale")]
    public async Task Handle_ValidId_ReturnsSale()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            IsCancelled = false,
            Items = new List<SaleItem>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    ProductTitle = "Test Product",
                    Quantity = 5,
                    UnitPrice = 100.00m,
                    DiscountPercent = 10m,
                    DiscountValue = 50.00m,
                    TotalGrossAmount = 500.00m,
                    TotalNetAmount = 450.00m
                }
            },
            TotalAmount = 450.00m
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.Number.Should().Be(sale.Number);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.Items.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Given non-existent sale id When getting sale Then returns null")]
    public async Task Handle_NonExistentId_ReturnsNull()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact(DisplayName = "Given empty guid When getting sale Then throws validation exception")]
    public async Task Handle_EmptyGuid_ThrowsValidationException()
    {
        var command = GetSaleHandlerTestData.GenerateInvalidCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("*Invalid id*");
    }

    [Fact(DisplayName = "Given valid id When getting sale Then repository is called once")]
    public async Task Handle_ValidId_CallsRepositoryOnce()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        await _handler.Handle(command, CancellationToken.None);

        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cancelled sale When getting sale Then returns cancelled sale")]
    public async Task Handle_CancelledSale_ReturnsCancelledSale()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            IsCancelled = true,
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.IsCancelled.Should().BeTrue();
    }
}

