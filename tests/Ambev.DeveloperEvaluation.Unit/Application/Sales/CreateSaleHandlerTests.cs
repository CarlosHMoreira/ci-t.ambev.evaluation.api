using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateSaleItemDto, SaleItem>();
            cfg.CreateMap<CreateSaleCommand, Sale>()
                .ForMember(dest => dest.Number, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());
            cfg.CreateMap<SaleItem, SaleItemResult>();
            cfg.CreateMap<Sale, SaleResult>();
        });
        _mapper = cfg.CreateMapper();
        
        _saleService = new SaleService(_saleRepository, _productRepository, _userRepository, _dispatcher);
        _handler = new CreateSaleHandler(_saleService, _mapper);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customerId = command.CustomerId;

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = "Test Product",
            Price = 10.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                ProductTitle = "Test Product",
                Quantity = i.Quantity,
                UnitPrice = 10.00m,
                DiscountPercent = i.Quantity >= 4 && i.Quantity < 10 ? 10m : 0m,
                TotalGrossAmount = i.Quantity * 10.00m,
                TotalNetAmount = i.Quantity * 10.00m
            }).ToList(),
            TotalAmount = command.Items.Sum(i => i.Quantity * 10.00m)
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(createdSale.Id);
        result.Number.Should().Be(createdSale.Number);
        result.CustomerName.Should().Be("John Doe");
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = CreateSaleHandlerTestData.GenerateInvalidCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given sale with quantity between 4-9 When creating sale Then applies 10% discount")]
    public async Task Handle_QuantityBetween4And9_Applies10PercentDiscount()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(5);
        var productId = command.Items[0].ProductId;
        var customerId = command.CustomerId;

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new()
                {
                    ProductId = productId,
                    ProductTitle = product.Title,
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

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(new[] { product });
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.First().DiscountPercent.Should().Be(10m);
        result.Items.First().TotalNetAmount.Should().Be(450.00m);
    }

    [Fact(DisplayName = "Given sale with quantity between 10-20 When creating sale Then applies 20% discount")]
    public async Task Handle_QuantityBetween10And20_Applies20PercentDiscount()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(15);
        var productId = command.Items[0].ProductId;
        var customerId = command.CustomerId;

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new()
                {
                    ProductId = productId,
                    ProductTitle = product.Title,
                    Quantity = 15,
                    UnitPrice = 100.00m,
                    DiscountPercent = 20m,
                    DiscountValue = 300.00m,
                    TotalGrossAmount = 1500.00m,
                    TotalNetAmount = 1200.00m
                }
            },
            TotalAmount = 1200.00m
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(new[] { product });
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.First().DiscountPercent.Should().Be(20m);
        result.Items.First().TotalNetAmount.Should().Be(1200.00m);
    }

    [Fact(DisplayName = "Given sale with quantity above 20 When creating sale Then throws exception")]
    public async Task Handle_QuantityAbove20_ThrowsException()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(25);
        var productId = command.Items[0].ProductId;
        var customerId = command.CustomerId;

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(new[] { product });
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given sale with non-existent product When creating sale Then throws exception")]
    public async Task Handle_NonExistentProduct_ThrowsException()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customerId = command.CustomerId;

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Product>());
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Product with id*not found*");
    }

    [Fact(DisplayName = "Given sale with non-existent customer When creating sale Then throws exception")]
    public async Task Handle_NonExistentCustomer_ThrowsException()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        
        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User with id*not found*");
    }

    [Fact(DisplayName = "Given valid sale When creating Then dispatches domain event")]
    public async Task Handle_ValidSale_DispatchesDomainEvent()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customerId = command.CustomerId;

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        await _handler.Handle(command, CancellationToken.None);

        await _dispatcher.Received(1).DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale with quantity less than 4 When creating sale Then applies no discount")]
    public async Task Handle_QuantityLessThan4_AppliesNoDiscount()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(3);
        var productId = command.Items[0].ProductId;
        var customerId = command.CustomerId;

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new()
                {
                    ProductId = productId,
                    ProductTitle = product.Title,
                    Quantity = 3,
                    UnitPrice = 100.00m,
                    DiscountPercent = 0m,
                    DiscountValue = 0m,
                    TotalGrossAmount = 300.00m,
                    TotalNetAmount = 300.00m
                }
            },
            TotalAmount = 300.00m
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(new[] { product });
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.First().DiscountPercent.Should().Be(0m);
        result.Items.First().TotalNetAmount.Should().Be(300.00m);
        result.Items.First().TotalGrossAmount.Should().Be(300.00m);
    }

    [Fact(DisplayName = "Given sale with multiple items When creating sale Then calculates total correctly")]
    public async Task Handle_MultipleItems_CalculatesTotalCorrectly()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithMultipleItems(3);
        var customerId = command.CustomerId;

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = $"Product {i.ProductId}",
            Price = 50.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        }).ToArray();

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var expectedTotal = command.Items.Sum(i => i.Quantity * 50.00m);
        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = command.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                ProductTitle = $"Product {i.ProductId}",
                Quantity = i.Quantity,
                UnitPrice = 50.00m,
                DiscountPercent = 0m,
                TotalGrossAmount = i.Quantity * 50.00m,
                TotalNetAmount = i.Quantity * 50.00m
            }).ToList(),
            TotalAmount = expectedTotal
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(products);
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Given sale with empty items list When creating sale Then throws validation exception")]
    public async Task Handle_EmptyItemsList_ThrowsValidationException()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items = new List<CreateSaleItemDto>();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given sale with quantity exactly 20 When creating sale Then applies 20% discount")]
    public async Task Handle_QuantityExactly20_Applies20PercentDiscount()
    {
        var command = CreateSaleHandlerTestData.GenerateCommandWithSpecificQuantity(20);
        var productId = command.Items[0].ProductId;
        var customerId = command.CustomerId;

        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 100.00m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        var customer = new User
        {
            Id = customerId,
            Name = new FullName { FirstName = "John", LastName = "Doe" },
            Email = "john.doe@test.com"
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = 1,
            Date = command.Date,
            CustomerId = command.CustomerId,
            CustomerName = "John Doe",
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>
            {
                new()
                {
                    ProductId = productId,
                    ProductTitle = product.Title,
                    Quantity = 20,
                    UnitPrice = 100.00m,
                    DiscountPercent = 20m,
                    DiscountValue = 400.00m,
                    TotalGrossAmount = 2000.00m,
                    TotalNetAmount = 1600.00m
                }
            },
            TotalAmount = 1600.00m
        };

        _productRepository.GetProductsByIdsAsync(Arg.Any<Guid[]>(), Arg.Any<CancellationToken>())
            .Returns(new[] { product });
        _userRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);
        _dispatcher.DispatchAsync(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.First().DiscountPercent.Should().Be(20m);
        result.Items.First().TotalNetAmount.Should().Be(1600.00m);
    }
}
