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
        CreateMap<User, DriverDto>()
            .ForMember(dest => dest.AvgRate, opt => opt.MapFrom(src => Math.Round(src.AvgRate, 2)))
            .ReverseMap();
        CreateMap<User, BlockDriverDto>().ReverseMap();
        CreateMap<Rating, RatingDto>().ReverseMap();
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.PhotoUrl))
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.PhotoUrl))
            .ReverseMap();
        CreateMap<DateTime, DateTime>()
            .ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>()
            .ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
            
}
