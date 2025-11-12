using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetSaleHandlerTestData
{
    private static readonly Faker<GetSaleCommand> GetSaleCommandFaker = new Faker<GetSaleCommand>()
        .CustomInstantiator(f => new GetSaleCommand(Guid.NewGuid()));

    public static GetSaleCommand GenerateValidCommand()
    {
        return GetSaleCommandFaker.Generate();
    }

    public static GetSaleCommand GenerateInvalidCommand()
    {
        return new GetSaleCommand(Guid.Empty);
    }
}

