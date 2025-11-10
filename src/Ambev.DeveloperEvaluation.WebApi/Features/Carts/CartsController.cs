using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

/// <summary>
/// Controller for managing cart operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController(IMediator mediator, IMapper mapper) : BaseController
{
    /// <summary>
    /// Creates a new cart
    /// </summary>
    /// <param name="request">The cart creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created cart</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CartResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCart(
        [FromBody] CreateCartRequest request, 
        CancellationToken cancellationToken)
    {
        var validator = new CreateCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = mapper.Map<CreateCartCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<CartResponse>(result);

        return Created(string.Empty, new ApiResponseWithData<CartResponse>
        {
            Success = true,
            Message = "Cart created successfully",
            Data = response
        });
    }

    /// <summary>
    /// Lists the carts
    /// </summary>
    /// <param name="request">The request parameters for listing carts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of carts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCarts([FromQuery] ListCartsRequest request, CancellationToken cancellationToken)
    {
        var validator = new ListCartsRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }
            
        var command = mapper.Map<ListCartsCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<PaginatedList<CartResponse>>(result);
        return OkPaginated(response);
    }

    /// <summary>
    /// Gets a specific cart by its ID
    /// </summary>
    /// <param name="id">The ID of the cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested cart</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new GetCartCommand { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<CartResponse>(result);
        return Ok(response);
    }

    /// <summary>
    /// Updates a specific cart
    /// </summary>
    /// <param name="id">The ID of the cart</param>
    /// <param name="request">The cart update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated cart</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<CartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCart([FromRoute] Guid id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var validator = new UpdateCartRequestValidator();
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }
        
        var command = mapper.Map<UpdateCartCommand>(request);
        var result = await mediator.Send(command, cancellationToken);
        var response = mapper.Map<CartResponse>(result);
        return Ok(new ApiResponseWithData<CartResponse>{ Success = true, Data = response });
    }

    /// <summary>
    /// Deletes a specific cart
    /// </summary>
    /// <param name="id">The ID of the cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCartCommand { Id = id };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
