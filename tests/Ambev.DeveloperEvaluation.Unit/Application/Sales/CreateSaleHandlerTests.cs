using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Events;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IMapper _mapper;
    private readonly SaleService _saleService;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _dispatcher = Substitute.For<IDomainEventDispatcher>();
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateSaleItemDto, SaleItem>();
            cfg.CreateMap<CreateSaleCommand, Sale>();
            cfg.CreateMap<SaleItem, SaleItemResult>();
            cfg.CreateMap<Sale, SaleResult>();
        });
        _mapper = cfg.CreateMapper();
        _saleService = new SaleService(_saleRepository, _dispatcher);
        _handler = new CreateSaleHandler(_saleService, _mapper);
    }
}
