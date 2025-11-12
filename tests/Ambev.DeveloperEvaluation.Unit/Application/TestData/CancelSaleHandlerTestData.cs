using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CancelSaleHandlerTestData
{
    private static readonly Faker<CancelSaleCommand> CancelSaleCommandFaker = new Faker<CancelSaleCommand>()
        .CustomInstantiator(f => new CancelSaleCommand(Guid.NewGuid()));

    public static CancelSaleCommand GenerateValidCommand()
    {
        return CancelSaleCommandFaker.Generate();
    }

    public static CancelSaleCommand GenerateInvalidCommand()
    {
        return new CancelSaleCommand(Guid.Empty);
    }
}

