using AutoMapper;
using Blogsphere.Api.Gateway.Entity;
using Contracts.Events;

namespace Blogsphere.Api.Gateway.Models.Mappings;

public class EventMappingProfile : Profile
{
    public EventMappingProfile()
    {
        CreateMap<ProxyCluster, ApiClusterCreated>()
        .ForMember(d => d.LoadBalancerName, opt => opt.MapFrom(s => s.LoadBalancingPolicy))
        .ForMember(d => d.RouteCount, opt => opt.MapFrom(s => s.Routes.Count))
        .ForMember(d => d.DestinationCount, opt => opt.MapFrom(s => s.Destinations.Count))
        .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsActive ? "Active" : "Inactive"));

        CreateMap<ProxyCluster, ApiClusterUpdated>()
        .ForMember(d => d.LoadBalancerName, opt => opt.MapFrom(s => s.LoadBalancingPolicy))
        .ForMember(d => d.RouteCount, opt => opt.MapFrom(s => s.Routes.Count))
        .ForMember(d => d.DestinationCount, opt => opt.MapFrom(s => s.Destinations.Count))
        .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsActive ? "Active" : "Inactive"));

        CreateMap<ProxyCluster, ApiClusterDeleted>();

        CreateMap<ProxyRoute, ApiRouteCreated>()
        .ForMember(d => d.TransformCount, opt => opt.MapFrom(s => s.Transforms.Count))
        .ForMember(d => d.RateLimitterPolicy, opt => opt.MapFrom(s => s.RateLimiterPolicy))
        .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsActive ? "Active" : "Inactive"));
        
        CreateMap<ProxyRoute, ApiRouteUpdated>()
        .ForMember(d => d.TransformCount, opt => opt.MapFrom(s => s.Transforms.Count))
        .ForMember(d => d.RateLimitterPolicy, opt => opt.MapFrom(s => s.RateLimiterPolicy))
        .ForMember(d => d.Status, opt => opt.MapFrom(s => s.IsActive ? "Active" : "Inactive"));

        CreateMap<ProxyRoute, ApiRouteDeleted>();
    }
}
