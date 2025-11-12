using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly SaleService _saleService;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        
        _saleService = new SaleService(_saleRepository, _productRepository, _userRepository, _dispatcher);
        _handler = new CancelSaleHandler(_saleService);
    }

    [Fact(DisplayName = "Given valid sale id When cancelling sale Then returns success")]
    public async Task Handle_ValidId_ReturnsSuccess()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
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
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(args => args.Arg<Sale>());
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Is<Sale>(s => s.IsCancelled), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale id When cancelling sale Then throws exception")]
    public async Task Handle_NonExistentId_ThrowsException()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Sale not found*");
    }

    [Fact(DisplayName = "Given empty guid When cancelling sale Then throws validation exception")]
    public async Task Handle_EmptyGuid_ThrowsValidationException()
    {
        var command = CancelSaleHandlerTestData.GenerateInvalidCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("*Invalid id*");
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling sale Then returns success")]
    public async Task Handle_AlreadyCancelled_ReturnsSuccess()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
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
        result.Id.Should().Be(command.Id);
        result.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(0).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid sale When cancelling Then dispatches domain event")]
    public async Task Handle_ValidSale_DispatchesDomainEvent()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
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
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(args => args.Arg<Sale>());
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        await _dispatcher.Received(1).DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>());
    }
}

