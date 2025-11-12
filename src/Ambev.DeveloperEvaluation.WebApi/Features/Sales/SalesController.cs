using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController(IMediator mediator, IMapper mapper) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var command = mapper.Map<CreateSaleCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<SaleResponse>(result);

        return Created(string.Empty, new ApiResponseWithData<SaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSaleCommand(id), cancellationToken);
        if (result is null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found"
            });
        }
        var response = mapper.Map<SaleResponse>(result);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> ListSales([FromQuery] ListSalesRequest request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return BadRequest(validation.Errors);
        var command = mapper.Map<Ambev.DeveloperEvaluation.Application.Sales.ListSales.ListSalesCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var responseItems = mapper.Map<IEnumerable<SaleResponse>>(result.ToList());
        var paginated = new PaginatedList<SaleResponse>(responseItems.ToList(), result.TotalCount, result.CurrentPage, result.PageSize);
        return OkPaginated(paginated);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSale.UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new UpdateSaleRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var command = mapper.Map<UpdateSaleCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<SaleResponse>(result);
        
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CancelSaleCommand(id), cancellationToken);
        return Ok(result);
    }
}
