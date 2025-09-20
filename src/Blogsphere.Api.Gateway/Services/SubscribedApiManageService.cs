using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Blogsphere.Api.Gateway.Services;

public class SubscribedApiManageService(
    ILogger logger,
    IUnitOfWork unitOfWork,
    ISubscriptionRepository<SubscribedApi> subscribedApiRepository,
    IMapper mapper,
    IApiProductManageService apiProductManageService) : ISubscribedApiManageService
{
    private readonly ILogger _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IApiProductManageService _apiProductManageService = apiProductManageService;
    private readonly ISubscriptionRepository<SubscribedApi> _subscribedApiRepository = subscribedApiRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<SubscribedApiDto>> CreateSubscribedApiAsync(CreateSubscribedApiRequest request, RequestInformation requestInformation)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} creating new subscribed api", nameof(CreateSubscribedApiAsync));

        if(await _subscribedApiRepository.IsSubscribedApiNameUniqueAsync(request.ApiName))
        {
            _logger.Here().Information("Subscribed api name {0} is not unique", request.ApiName);
            return Result<SubscribedApiDto>.Failure(ErrorCodes.BadRequest, "Subscribed api name is not unique");
        }
        if(await _subscribedApiRepository.IsSubscribedApiPathUniqueAsync(request.ApiPath))
        {
            _logger.Here().Information("Subscribed api path {0} is not unique", request.ApiPath);
            return Result<SubscribedApiDto>.Failure(ErrorCodes.BadRequest, "Subscribed api path is not unique");
        }

        var apiProduct = await _apiProductManageService.GetApiProductByIdAsync(request.ProductId);
        if(!apiProduct.IsSuccess || apiProduct.Value == null)
        {
            _logger.Here().WithApiProductId(request.ProductId).Information("Api subscription create request for invalid api product");
            return Result<SubscribedApiDto>.Failure(ErrorCodes.NotFound, "Api subscription create request for invalid api product");
        }

        var newSubscribedApi = _mapper.Map<SubscribedApi>(request);
        newSubscribedApi.CreatedBy = requestInformation.CurrentUser.Id;

        await _subscribedApiRepository.AddAsync(newSubscribedApi);
        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<SubscribedApiDto>(newSubscribedApi);

        _logger.Here().WithSubscribedApiId(newSubscribedApi.Id).Information("Subscribed api created");
        _logger.Here().MethodExited();

        return Result<SubscribedApiDto>.Success(dto);
    }

    public async Task<Result<bool>> DeleteSubscribedApiAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithSubscribedApiId(id).Information("Request - {0} deleting subscribed api", nameof(DeleteSubscribedApiAsync));

        var subscribedApi = await _subscribedApiRepository.GetByIdAsync(id);
        if(subscribedApi == null)
        {
            _logger.Here().Information("Subscribed api not found");
            return Result<bool>.Failure(ErrorCodes.NotFound, "Subscribed api not found");
        }

        _subscribedApiRepository.Remove(subscribedApi);
        await _unitOfWork.SaveChangesAsync();

        _logger.Here().WithSubscribedApiId(id).Information("Subscribed api deleted");
        _logger.Here().MethodExited();

        return Result<bool>.Success(true);  
    }

    public async Task<Result<PaginatedResult<SubscribedApiDto>>> GetAllSubscribedApisAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} fetching all subscribed apis", nameof(GetAllSubscribedApisAsync));

        var (subscribedApis, count) = await GetPaginatedResultAsync(request);

        if(subscribedApis.IsNullOrEmpty())
        {
            _logger.Here().Information("No subscribed apis found");
            return Result<PaginatedResult<SubscribedApiDto>>.Success(new PaginatedResult<SubscribedApiDto>()
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            });
        }

        // Map to DTO
        var subscribedApiDtos = _mapper.Map<List<SubscribedApiDto>>(subscribedApis);
        var paginatedResult = new PaginatedResult<SubscribedApiDto>
        {
            Items = subscribedApiDtos,
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.Here().Information("{0} subscribed apis found", count);
        _logger.Here().MethodExited();
        return Result<PaginatedResult<SubscribedApiDto>>.Success(paginatedResult);
    }

    public async Task<Result<SubscribedApiDto>> GetSubscribedApiByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithSubscribedApiId(id)
            .Information("Request - {0} fetching subscribed api by id", nameof(GetSubscribedApiByIdAsync));

        var subscribedApi = await IncludeAllMemebers(_subscribedApiRepository.AsQueryable())
            .FirstOrDefaultAsync(x => x.Id == id);

        if(subscribedApi is null)
        {
            _logger.Here().WithSubscribedApiId(id).Warning("Subscribed api with id {Id} not found", id);
            return Result<SubscribedApiDto>.Failure(ErrorCodes.NotFound);
        }

        var dto = _mapper.Map<SubscribedApiDto>(subscribedApi);

        _logger.Here().WithSubscribedApiId(id).Information("Subscribed api found");
        _logger.Here().MethodExited();

        return Result<SubscribedApiDto>.Success(dto);
    }

    public async Task<Result<List<SubscribedApiDto>>> GetSubscribedApisByProductIdAsync(Guid productId)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithApiProductId(productId)
            .Information("Request - {0} fetching subscribed apis by product id", nameof(GetSubscribedApisByProductIdAsync));

        var subscribedApis = await IncludeAllMemebers(_subscribedApiRepository.AsQueryable())
            .Where(x => x.ProductId == productId)
            .Select(x => _mapper.Map<SubscribedApiDto>(x))
            .ToListAsync();

        if(subscribedApis.IsNullOrEmpty())
        {
            _logger.Here().WithApiProductId(productId).Information("No subscribed apis found");
            return Result<List<SubscribedApiDto>>.Success([]);
        }

        _logger.Here().WithApiProductId(productId).Information("{0} subscribed apis found", subscribedApis.Count);
        _logger.Here().MethodExited();

        return Result<List<SubscribedApiDto>>.Success(subscribedApis);
    }

    public async Task<Result<bool>> UpdateSubscribedApiAsync(UpdateSubscribedApiRequest request, RequestInformation requestInformation)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} updating subscribed api", nameof(UpdateSubscribedApiAsync));

        var subscribedApi = await _subscribedApiRepository.GetByIdAsync(request.Id);
        if(subscribedApi == null)
        {
            _logger.Here().Information("Subscribed api not found");
            return Result<bool>.Failure(ErrorCodes.NotFound, "Subscribed api not found");
        }

        var apiProduct = await _apiProductManageService.GetApiProductByIdAsync(request.ProductId);
        if(apiProduct.IsSuccess == false || apiProduct.Value == null)
        {
            _logger.Here().WithApiProductId(request.ProductId).Information("Api subscription update request for invalid api product");
            return Result<bool>.Failure(ErrorCodes.NotFound, "Api subscription request for invalid api product");
        }

        subscribedApi.ProductId = request.ProductId;
        subscribedApi.UpdatedBy = requestInformation.CurrentUser.Id;

        _subscribedApiRepository.Update(subscribedApi);
        await _unitOfWork.SaveChangesAsync();

        _logger.Here().WithSubscribedApiId(request.Id).Information("Subscribed api updated");
        _logger.Here().MethodExited();

        return Result<bool>.Success(true);
    }

    private async Task<(List<SubscribedApi> SubscribedApis, int Count)> GetPaginatedResultAsync(PaginationRequest request)
    {
        var query = IncludeAllMemebers(_subscribedApiRepository.AsQueryable());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    private IQueryable<SubscribedApi> IncludeAllMemebers(IQueryable<SubscribedApi> query)
    {
        return query.Include(x => x.ApiProduct);
    }
}
