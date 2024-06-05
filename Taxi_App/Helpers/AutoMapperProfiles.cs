using AutoMapper;

namespace Taxi_App;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, User>().ReverseMap();
        CreateMap<VerificationDto, User>().ReverseMap();
        CreateMap<UserUpdateDto, User>().ReverseMap();
        CreateMap<GoogleRegisterDto, User>().ReverseMap();
        CreateMap<AddressDto, Ride>().ReverseMap();
        CreateMap<Ride, RideDto>().ReverseMap();
        CreateMap<Ride, DetailedRideDto>()
            .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.DriverUsername, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.UserName : null))
            .ReverseMap();
        CreateMap<User, DriverDto>().ReverseMap();
        CreateMap<User, BlockDriverDto>().ReverseMap();
        CreateMap<Rating, RatingDto>().ReverseMap();
    }
            
}
