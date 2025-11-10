using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsHandler(ICartRepository repository, IMapper mapper)
    : IRequestHandler<ListCartsCommand, ListCartsResult>
{
    public async Task<ListCartsResult> Handle(ListCartsCommand command, CancellationToken cancellationToken)
    {
        var (carts, total) = await repository.ListAsync(command.Page, command.Size, command.Order, cancellationToken);
        return new ListCartsResult
        {
            Items = mapper.Map<IEnumerable<CartResult>>(carts),
            TotalCount = total,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
