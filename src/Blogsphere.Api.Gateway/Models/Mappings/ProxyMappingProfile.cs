using AutoMapper;

namespace Blogsphere.Api.Gateway.Models.Mappings;

public class ProxyMappingProfile : Profile
{
    public ProxyMappingProfile()
    {
        // Entity to DTO mappings (include all fields including audit fields)
        CreateMap<ProxyCluster, ProxyClusterDto>()
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
        .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => new MetaDataDto
        {
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy
        }));
        CreateMap<ProxyRoute, ProxyRouteDto>()
        .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => new MetaDataDto
        {
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy
        }));
        CreateMap<ProxyDestination, ProxyDestinationDto>()
        .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => new MetaDataDto
        {
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy
        }));

        CreateMap<ProxyHeader, ProxyHeaderDto>()
         .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => new MetaDataDto
        {
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy
        }));

        CreateMap<ProxyTransform, ProxyTransformDto>()
         .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => new MetaDataDto
        {
            CreatedAt = src.CreatedAt,
            UpdatedAt = src.UpdatedAt,
            CreatedBy = src.CreatedBy,
            UpdatedBy = src.UpdatedBy
        })).ReverseMap();

        // DTO to Entity mappings (include all fields including audit fields)
        CreateMap<ProxyClusterDto, ProxyCluster>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.HealthCheckInterval, opt => opt.MapFrom(src => src.HealthCheckInterval ?? 0))
            .ForMember(dest => dest.HealthCheckTimeout, opt => opt.MapFrom(src => src.HealthCheckTimeout ?? 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Metadata.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Metadata.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Metadata.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Metadata.UpdatedBy));

        CreateMap<ProxyDestinationDto, ProxyDestination>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Metadata.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Metadata.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Metadata.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Metadata.UpdatedBy));
        
        CreateMap<ProxyRouteDto, ProxyRoute>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Metadata.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Metadata.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Metadata.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Metadata.UpdatedBy));
            
        CreateMap<ProxyHeaderDto, ProxyHeader>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Metadata.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Metadata.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Metadata.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Metadata.UpdatedBy));

        CreateMap<ProxyTransformDto, ProxyTransform>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Metadata.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Metadata.UpdatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Metadata.CreatedBy))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.Metadata.UpdatedBy));

        // Request to DTO mappings for clean service layer
        CreateMap<CreateProxyClusterRequest, ProxyClusterDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Metadata, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore());

        CreateMap<CreateProxyDestinationRequest, ProxyDestinationDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClusterId, opt => opt.Ignore());

        CreateMap<CreateProxyRouteRequest, ProxyRouteDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Metadata, opt => opt.Ignore());

        CreateMap<ProxyHeaderRequest, ProxyHeaderDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ProxyTransformRequest, ProxyTransformDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Update request to DTO mappings
        CreateMap<UpdateProxyClusterRequest, ProxyClusterDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Metadata, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore());

        CreateMap<UpdateProxyDestinationRequest, ProxyDestinationDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClusterId, opt => opt.Ignore());

        CreateMap<UpdateProxyRouteRequest, ProxyRouteDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Request to Entity mappings (for backwards compatibility)
        CreateMap<CreateProxyClusterRequest, ProxyCluster>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Destinations, opt => opt.Ignore())
            .ForMember(dest => dest.Routes, opt => opt.Ignore());

        CreateMap<CreateProxyDestinationRequest, ProxyDestination>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClusterId, opt => opt.Ignore())
            .ForMember(dest => dest.Cluster, opt => opt.Ignore());

        CreateMap<CreateProxyRouteRequest, ProxyRoute>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.Headers, opt => opt.Ignore())
            .ForMember(dest => dest.Transforms, opt => opt.Ignore())
            .ForMember(dest => dest.Cluster, opt => opt.Ignore());

        CreateMap<ProxyHeaderRequest, ProxyHeader>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.RouteId, opt => opt.Ignore())
            .ForMember(dest => dest.Route, opt => opt.Ignore());

        CreateMap<ProxyTransformRequest, ProxyTransform>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.RouteId, opt => opt.Ignore())
            .ForMember(dest => dest.Route, opt => opt.Ignore());

        CreateMap<ProxyCluster, ProxyClusterSearchableDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.LoadBalancerName, opt => opt.MapFrom(src => src.LoadBalancingPolicy))
            .ForMember(dest => dest.DestinationCount, opt => opt.MapFrom(src => src.Destinations.Count))
            .ForMember(dest => dest.RouteCount, opt => opt.MapFrom(src => src.Routes.Count))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));
        
        CreateMap<ProxyRoute, ProxyRouteSearchableDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.RouteId, opt => opt.MapFrom(src => src.RouteId))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Cluster, opt => opt.MapFrom(src => src.Cluster.ClusterId))
            .ForMember(dest => dest.RateLimitterPolicy, opt => opt.MapFrom(src => src.RateLimiterPolicy))
            .ForMember(dest => dest.TransformCount, opt => opt.MapFrom(src => src.Transforms.Count))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));
    }
} 