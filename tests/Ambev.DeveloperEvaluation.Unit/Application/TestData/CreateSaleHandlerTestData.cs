using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleItemDto> CreateSaleItemFaker = new Faker<CreateSaleItemDto>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10));

    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.Date, f => f.Date.Recent(30))
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => CreateSaleItemFaker.Generate(f.Random.Int(1, 5)));

    public static CreateSaleCommand GenerateValidCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }

    public static CreateSaleCommand GenerateCommandWithSpecificQuantity(int quantity)
    {
        var command = CreateSaleCommandFaker.Generate();
        command.Items = new List<CreateSaleItemDto>
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                Quantity = quantity
            }
        };
        return command;
    }

    public static CreateSaleCommand GenerateCommandWithMultipleItems(int itemCount)
    {
        var command = CreateSaleCommandFaker.Generate();
        command.Items = CreateSaleItemFaker.Generate(itemCount);
        return command;
    }

    public static CreateSaleCommand GenerateCommandWithDuplicateProducts()
    {
        var command = CreateSaleCommandFaker.Generate();
        var productId = Guid.NewGuid();
        command.Items = new List<CreateSaleItemDto>
        {
            new() { ProductId = productId, Quantity = 5 },
            new() { ProductId = productId, Quantity = 3 }
        };
        return command;
    }

    public static CreateSaleCommand GenerateInvalidCommand()
    {
        return new CreateSaleCommand
        {
            CustomerId = Guid.Empty,
            BranchId = Guid.Empty,
            BranchName = string.Empty,
            Items = new List<CreateSaleItemDto>()
        };
    }
}

