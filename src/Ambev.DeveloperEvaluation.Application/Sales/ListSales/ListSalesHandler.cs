using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler(SaleService service, IMapper mapper) : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    public async Task<ListSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListSaleValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        var (sales, total) = await service
            .ListAsync(request.Page, request.Size, request.Order, request.Filters, cancellationToken);
        
        var mapped = mapper.Map<IEnumerable<SaleResult>>(sales);
        
        return new ListSalesResult(mapped)
        {
            CurrentPage = request.Page,
            PageSize = request.Size,
            TotalCount = total
        };
    }
}

