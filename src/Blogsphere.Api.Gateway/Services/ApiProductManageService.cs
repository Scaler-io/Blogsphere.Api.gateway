using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Blogsphere.Api.Gateway.Services;

public class ApiProductManageService(
    ILogger logger,
    IUnitOfWork unitOfWork,
    ISubscriptionRepository<ApiProduct> apiProductRepository,
    IMapper mapper)  : IApiProductManageService
{
    private readonly ILogger _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISubscriptionRepository<ApiProduct> _apiProductRepository = apiProductRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<PaginatedResult<ApiProductDto>>> GetAllApiProductsAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} fetching all api products", nameof(GetAllApiProductsAsync));

        var result = await GetPaginatedResultAsync(request);

        if(result.Products.IsNullOrEmpty())
        {
            _logger.Here().Information("No api products found");
            return Result<PaginatedResult<ApiProductDto>>.Success(new PaginatedResult<ApiProductDto>()
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            });
        }

        // Map to DTO
        var apiProductDtos = _mapper.Map<List<ApiProductDto>>(result.Products);
        var paginatedResult = new PaginatedResult<ApiProductDto>
        {
            Items = apiProductDtos,
            TotalCount = result.Count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.Here().Information("{0} api products found", result.Count);
        _logger.Here().MethodExited();
        return Result<PaginatedResult<ApiProductDto>>.Success(paginatedResult);
    }

    public async Task<Result<ApiProductDto>> GetApiProductByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithApiProductId(id)
            .Information("Request - {0} fetching api product by id", nameof(GetApiProductByIdAsync));

        var apiProduct = await IncludeAllMemebers(_apiProductRepository.AsQueryable())
            .FirstOrDefaultAsync(x => x.Id == id);

        if(apiProduct is null)
        {
            _logger.Here().WithApiProductId(id).Warning("Api product with id {Id} not found", id);
            return Result<ApiProductDto>.Failure(ErrorCodes.NotFound);
        }

        var dto = _mapper.Map<ApiProductDto>(apiProduct);

        _logger.Here().WithApiProductId(id).Information("Api product found");
        _logger.Here().MethodExited();

        return Result<ApiProductDto>.Success(dto);
    }
    public async Task<Result<ApiProductDto>> CreateApiProductAsync(CreateApiProductRequest request, RequestInformation requestInformation)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} creating new api product", nameof(CreateApiProductAsync));

        if(await _apiProductRepository.IsApiProductNameUniqueAsync(request.ProductName))
        {
            _logger.Here().Information("Api product name {ProductName} is not unique", request.ProductName);
            return Result<ApiProductDto>.Failure(ErrorCodes.BadRequest, "Api product name is not unique");
        }

        var apiProduct = _mapper.Map<ApiProduct>(request);
        apiProduct.CreatedBy = requestInformation.CurrentUser.Id;
        
        await _apiProductRepository.AddAsync(apiProduct);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<ApiProductDto>(apiProduct);

        _logger.Here().WithApiProductId(apiProduct.Id).Information("Api product created");
        _logger.Here().MethodExited();

        return Result<ApiProductDto>.Success(dto);
    }
    public async Task<Result<bool>> DeleteApiProductAsync(Guid id, RequestInformation requestInformation)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithApiProductId(id)
            .Information("Request - {0} deleting api product", nameof(DeleteApiProductAsync));

        var apiProduct = await _apiProductRepository.GetByIdAsync(id);

        if(apiProduct is null)
        {
            _logger.Here().WithApiProductId(id).Warning("Api product with id {Id} not found", id);
            return Result<bool>.Failure(ErrorCodes.NotFound);
        }

        _apiProductRepository.Remove(apiProduct);
        await _unitOfWork.SaveChangesAsync();

        _logger.Here().WithApiProductId(id).Information("Api product deleted");
        _logger.Here().MethodExited();

        return Result<bool>.Success(true);
    }

    private async Task<(List<ApiProduct> Products, int Count)> GetPaginatedResultAsync(PaginationRequest request)
    {
        var query = IncludeAllMemebers(_apiProductRepository.AsQueryable());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    private IQueryable<ApiProduct> IncludeAllMemebers(IQueryable<ApiProduct> query)
    {
        return query.Include(x => x.Subscriptions)
        .Include(x => x.SubscribedApis);
    }
}
