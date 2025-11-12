using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListSalesHandlerTestData
{
    private static readonly Faker<ListSalesCommand> ListSalesCommandFaker = new Faker<ListSalesCommand>()
        .RuleFor(s => s.Page, f => f.Random.Int(1, 10))
        .RuleFor(s => s.Size, f => f.Random.Int(10, 50))
        .RuleFor(s => s.Order, f => "Date desc")
        .RuleFor(s => s.Filters, f => new Dictionary<string, string>());

    public static ListSalesCommand GenerateValidCommand()
    {
        return ListSalesCommandFaker.Generate();
    }

    public static ListSalesCommand GenerateInvalidCommand()
    {
        return new ListSalesCommand
        {
            Page = 0,
            Size = 0,
            Order = string.Empty,
            Filters = new Dictionary<string, string>()
        };
    }

    public static ListSalesCommand GenerateCommandWithFilters(Dictionary<string, string> filters)
    {
        var command = ListSalesCommandFaker.Generate();
        command.Filters = filters;
        return command;
    }
}

