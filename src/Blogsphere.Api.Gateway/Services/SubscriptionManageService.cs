using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Blogsphere.Api.Gateway.Services;

public class SubscriptionManageService(
    ILogger logger,
    ISubscriptionRepository<Subscription> subscriptionRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IApiProductManageService apiProductManageService) : ISubscriptionManageService
{
    private readonly ILogger _logger = logger;
    private readonly ISubscriptionRepository<Subscription> _subscriptionRepository = subscriptionRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IApiProductManageService _apiProductManageService = apiProductManageService;

    public async Task<Result<PaginatedResult<SubscriptionDto>>> GetAllSubscriptionsAsync(PaginationRequest request)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} fetching all subscriptions", nameof(GetAllSubscriptionsAsync));

        var (subscriptions, count) = await GetPaginatedResultAsync(request);

        if(subscriptions.IsNullOrEmpty())
        {
            _logger.Here().Information("No api products found");
            return Result<PaginatedResult<SubscriptionDto>>.Success(new PaginatedResult<SubscriptionDto>()
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            });
        }

        // Map to DTO
        var apiProductDtos = _mapper.Map<List<SubscriptionDto>>(subscriptions);
        var paginatedResult = new PaginatedResult<SubscriptionDto>
        {
            Items = apiProductDtos,
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _logger.Here().Information("{0} subscriptions found", count);
        _logger.Here().MethodExited();
        return Result<PaginatedResult<SubscriptionDto>>.Success(paginatedResult);
    }

    public async Task<Result<SubscriptionDto>> GetSubscriptionByIdAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithSubscriptionId(id)
            .Information("Request - {0} fetching subscription by id", nameof(GetSubscriptionByIdAsync));

        var subscription = await IncludeAllMemebers(_subscriptionRepository.AsQueryable())
            .FirstOrDefaultAsync(x => x.Id == id);

        if(subscription is null)
        {
            _logger.Here().WithSubscriptionId(id).Warning("Subscription with id {Id} not found", id);
            return Result<SubscriptionDto>.Failure(ErrorCodes.NotFound);
        }

        var dto = _mapper.Map<SubscriptionDto>(subscription);

        _logger.Here().WithSubscriptionId(id).Information("Subscription found");
        _logger.Here().MethodExited();

        return Result<SubscriptionDto>.Success(dto);
    }

    public async Task<Result<SubscriptionDto>> CreateSubscriptionAsync(CreateSubscriptionRequest request, RequestInformation requestInformation)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - {0} creating subscription", nameof(CreateSubscriptionAsync));


        var apiProduct = await _apiProductManageService.GetApiProductByIdAsync(request.ProductId);
        if(!apiProduct.IsSuccess || apiProduct.Value is null)
        {
            _logger.Here().WithApiProductId(request.ProductId).Warning("Api product with id {Id} not found", request.ProductId);
            return Result<SubscriptionDto>.Failure(ErrorCodes.NotFound);
        }

        var subscription = _mapper.Map<Subscription>(request);
        subscription.CreatedBy = requestInformation.CurrentUser.Id;

        await _subscriptionRepository.AddAsync(subscription);
        await _unitOfWork.SaveChangesAsync();

        _logger.Here().Information("Subscription created");
        _logger.Here().MethodExited();

        return Result<SubscriptionDto>.Success(_mapper.Map<SubscriptionDto>(subscription));
    }

    public async Task<Result<bool>> DeleteSubscriptionAsync(Guid id)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithSubscriptionId(id)
            .Information("Request - {0} deleting subscription", nameof(DeleteSubscriptionAsync));

        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if(subscription is null)
        {
            _logger.Here().WithSubscriptionId(id).Warning("Subscription with id {Id} not found", id);
            return Result<bool>.Failure(ErrorCodes.NotFound);
        }

        _subscriptionRepository.Remove(subscription);
        await _unitOfWork.SaveChangesAsync();

        _logger.Here().WithSubscriptionId(id).Information("Subscription deleted");
        _logger.Here().MethodExited();

        return Result<bool>.Success(true);
    }

    private async Task<(List<Subscription> Subscriptions, int Count)> GetPaginatedResultAsync(PaginationRequest request)
    {
        var query = IncludeAllMemebers(_subscriptionRepository.AsQueryable());
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    private static IQueryable<Subscription> IncludeAllMemebers(IQueryable<Subscription> query)
    {
        return query.Include(x => x.ApiProduct);
    }

    public async Task<Result<SubscriptionDto>> GetSubscriptionByKeyAsync(string subscriptionKey)
    {
        var subscription = await _subscriptionRepository.AsQueryable()
            .Include(x => x.ApiProduct)
            .FirstOrDefaultAsync(x => x.SubscriptionKey == subscriptionKey);

        if(subscription is null)
        {
            return Result<SubscriptionDto>.Failure(ErrorCodes.NotFound);
        }

        return Result<SubscriptionDto>.Success(_mapper.Map<SubscriptionDto>(subscription));
    }
}
