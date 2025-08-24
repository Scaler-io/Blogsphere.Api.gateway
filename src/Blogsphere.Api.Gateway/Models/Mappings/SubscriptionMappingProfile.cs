using AutoMapper;

namespace Blogsphere.Api.Gateway.Models.Mappings;

public class SubscriptionMappingProfile : Profile
{
    public SubscriptionMappingProfile()
    {
        CreateMap<CreateApiProductRequest, ApiProduct>();
        CreateMap<CreateSubscribedApiRequest, SubscribedApi>();
        CreateMap<CreateSubscriptionRequest, Subscription>();
        CreateMap<UpdateSubscribedApiRequest, SubscribedApi>();

        CreateMap<ApiProduct, ApiProductDto>()
        .ForMember(s => s.ProductId, opt => opt.MapFrom(d => d.Id))
        .ForMember(s => s.IsActive, opt => opt.MapFrom(d => d.IsActive))
        .ForMember(s => s.SubscriptionCount, opt => opt.MapFrom(d => d.Subscriptions.Count))
        .ForMember(s => s.SubscribedApiCount, opt => opt.MapFrom(d => d.SubscribedApis.Count))
        .ForMember(s => s.CreatedAt, opt => opt.MapFrom(d => d.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss tt")))
        .ForMember(s => s.UpdatedAt, opt => opt.MapFrom(d => d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss tt") : null));


        CreateMap<SubscribedApi, SubscribedApiDto>()
        .ForMember(s => s.ApiPath, opt => opt.MapFrom(d => d.ApiPath))
        .ForMember(s => s.ApiName, opt => opt.MapFrom(d => d.ApiName))
        .ForMember(s => s.ApiDescription, opt => opt.MapFrom(d => d.ApiDescription))
        .ForMember(s => s.ApiProductDetails, opt => opt.MapFrom(d => new ApiProductSummary
        {
            Id = d.ApiProduct.Id,
            ProductName = d.ApiProduct.ProductName
        }))
        .ForMember(s => s.CreatedAt, opt => opt.MapFrom(d => d.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss tt")))
        .ForMember(s => s.UpdatedAt, opt => opt.MapFrom(d => d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss tt") : null));

        CreateMap<Subscription, SubscriptionDto>()
        .ForMember(s => s.SubscriptionName, opt => opt.MapFrom(d => d.SubscriptionName))
        .ForMember(s => s.SubscriptionDescription, opt => opt.MapFrom(d => d.SubscriptionDescription))
        .ForMember(s => s.ApiProductDetails, opt => opt.MapFrom(d => new ApiProductSummary
        {
            Id = d.ApiProduct.Id,
            ProductName = d.ApiProduct.ProductName
        }))
        .ForMember(s => s.CreatedAt, opt => opt.MapFrom(d => d.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss tt")))
        .ForMember(s => s.UpdatedAt, opt => opt.MapFrom(d => d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss tt") : null));


        // Removed duplicate mappings as they were overriding the detailed configurations above
    }
}
