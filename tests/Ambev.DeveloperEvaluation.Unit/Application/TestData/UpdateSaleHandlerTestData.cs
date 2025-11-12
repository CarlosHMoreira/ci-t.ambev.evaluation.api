using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker<UpdateSaleItemDto> UpdateSaleItemFaker = new Faker<UpdateSaleItemDto>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.ProductTitle, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100))
        .RuleFor(i => i.IsCancelled, f => false);

    private static readonly Faker<UpdateSaleCommand> UpdateSaleCommandFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => UpdateSaleItemFaker.Generate(f.Random.Int(1, 5)));

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return UpdateSaleCommandFaker.Generate();
    }

    public static UpdateSaleCommand GenerateInvalidCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.Empty,
            CustomerId = Guid.Empty,
            CustomerName = string.Empty,
            BranchId = Guid.Empty,
            BranchName = string.Empty,
            Items = new List<UpdateSaleItemDto>()
        };
    }

    public static UpdateSaleCommand GenerateCommandWithSpecificQuantity(int quantity)
    {
        var command = UpdateSaleCommandFaker.Generate();
        command.Items = new List<UpdateSaleItemDto>
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                ProductTitle = "Test Product",
                Quantity = quantity,
                UnitPrice = 100.00m,
                IsCancelled = false
            }
        };
        return command;
    }
}

