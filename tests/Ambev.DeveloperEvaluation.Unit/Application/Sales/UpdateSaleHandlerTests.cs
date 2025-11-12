using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateSaleItemDto, SaleItem>();
            cfg.CreateMap<UpdateSaleCommand, Sale>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Number, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());
            cfg.CreateMap<SaleItem, SaleItemResult>();
            cfg.CreateMap<Sale, SaleResult>();
        });
        _mapper = cfg.CreateMapper();
        
        _saleService = new SaleService(_saleRepository, _productRepository, _userRepository, _dispatcher);
        _handler = new UpdateSaleHandler(_saleService, _mapper);
    }

    [Fact(DisplayName = "Given valid update command When updating sale Then returns updated sale")]
    public async Task Handle_ValidCommand_ReturnsUpdatedSale()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Old Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Old Branch",
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = i.ProductTitle,
            Price = i.UnitPrice,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = command.CustomerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var updatedSale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = existingSale.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                ProductTitle = i.ProductTitle,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountPercent = 0m,
                TotalGrossAmount = i.Quantity * i.UnitPrice,
                TotalNetAmount = i.Quantity * i.UnitPrice
            }).ToList(),
            TotalAmount = command.Items.Sum(i => i.Quantity * i.UnitPrice)
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CustomerName.Should().Be("John Doe");
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When updating Then throws exception")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Sale not found*");
    }

    [Fact(DisplayName = "Given invalid update command When updating Then throws validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = UpdateSaleHandlerTestData.GenerateInvalidCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given quantity above 20 When updating Then throws exception")]
    public async Task Handle_QuantityAbove20_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(25);
        var existingSale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = i.ProductTitle,
            Price = i.UnitPrice,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = command.CustomerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given valid update When updating Then dispatches domain event")]
    public async Task Handle_ValidUpdate_DispatchesDomainEvent()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Old Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Old Branch",
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = i.ProductTitle,
            Price = i.UnitPrice,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = command.CustomerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(args => args.Arg<Sale>());
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        await _dispatcher.Received(1).DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent product When updating Then throws exception")]
    public async Task Handle_NonExistentProduct_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            Number = 1,
            Date = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Test Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Test Branch",
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        var customer = new User
        {
            Id = command.CustomerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Product>());
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Product with id*not found*");
    }
}

