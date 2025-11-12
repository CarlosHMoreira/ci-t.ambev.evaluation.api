using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
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

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
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
        _handler = new ListSalesHandler(_saleService, _mapper);
    }

    [Fact(DisplayName = "Given valid list command When listing sales Then returns paginated results")]
    public async Task Handle_ValidCommand_ReturnsPaginatedResults()
    {
        var command = ListSalesHandlerTestData.GenerateValidCommand();
        var sales = new List<Sale>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Date = DateTime.UtcNow.AddDays(-1),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer 1",
                BranchId = Guid.NewGuid(),
                BranchName = "Branch 1",
                Items = new List<SaleItem>(),
                TotalAmount = 100.00m
            },
            new()
            {
                Id = Guid.NewGuid(),
                Number = 2,
                Date = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer 2",
                BranchId = Guid.NewGuid(),
                BranchName = "Branch 2",
                Items = new List<SaleItem>(),
                TotalAmount = 200.00m
            }
        };

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((sales, 2));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }

    [Fact(DisplayName = "Given empty results When listing sales Then returns empty list")]
    public async Task Handle_NoSales_ReturnsEmptyList()
    {
        var command = ListSalesHandlerTestData.GenerateValidCommand();

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 0));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact(DisplayName = "Given invalid command When listing sales Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = ListSalesHandlerTestData.GenerateInvalidCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given filters When listing sales Then passes filters to repository")]
    public async Task Handle_WithFilters_PassesFiltersToRepository()
    {
        var filters = new Dictionary<string, string>
        {
            { "IsCancelled", "false" },
            { "CustomerId", Guid.NewGuid().ToString() }
        };
        var command = ListSalesHandlerTestData.GenerateCommandWithFilters(filters);

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 0));

        await _handler.Handle(command, CancellationToken.None);

        await _saleRepository.Received(1).ListAsync(
            command.Page, 
            command.Size, 
            command.Order, 
            Arg.Is<Dictionary<string, string>>(d => d.ContainsKey("IsCancelled") && d.ContainsKey("CustomerId")), 
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given page and size When listing sales Then returns correct pagination")]
    public async Task Handle_WithPagination_ReturnsCorrectPagination()
    {
        var command = new ListSalesCommand
        {
            Page = 2,
            Size = 10,
            Order = "Date desc",
            Filters = new Dictionary<string, string>()
        };

        var sales = Enumerable.Range(1, 10).Select(i => new Sale
        {
            Id = Guid.NewGuid(),
            Number = i,
            Date = DateTime.UtcNow.AddDays(-i),
            CustomerId = Guid.NewGuid(),
            CustomerName = $"Customer {i}",
            BranchId = Guid.NewGuid(),
            BranchName = $"Branch {i}",
            Items = new List<SaleItem>(),
            TotalAmount = i * 100.00m
        }).ToList();

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((sales, 50));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(50);
        result.Should().HaveCount(10);
    }

    [Fact(DisplayName = "Given cancelled sales filter When listing sales Then returns only cancelled sales")]
    public async Task Handle_CancelledFilter_ReturnsOnlyCancelledSales()
    {
        var filters = new Dictionary<string, string> { { "IsCancelled", "true" } };
        var command = ListSalesHandlerTestData.GenerateCommandWithFilters(filters);

        var cancelledSales = new List<Sale>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Date = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Customer 1",
                BranchId = Guid.NewGuid(),
                BranchName = "Branch 1",
                IsCancelled = true,
                Items = new List<SaleItem>(),
                TotalAmount = 100.00m
            }
        };

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((cancelledSales, 1));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given order parameter When listing sales Then repository receives order")]
    public async Task Handle_WithOrder_PassesOrderToRepository()
    {
        var command = new ListSalesCommand
        {
            Page = 1,
            Size = 10,
            Order = "TotalAmount desc",
            Filters = new Dictionary<string, string>()
        };

        _saleRepository.ListAsync(command.Page, command.Size, command.Order, command.Filters, Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 0));

        await _handler.Handle(command, CancellationToken.None);

        await _saleRepository.Received(1).ListAsync(
            command.Page,
            command.Size,
            "TotalAmount desc",
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>());
    }
}

