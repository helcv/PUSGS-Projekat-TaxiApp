﻿using AutoMapper;

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
        CreateMap<User, DriverDto>().ReverseMap();
        CreateMap<User, BlockDriverDto>().ReverseMap();
        CreateMap<Rating, RatingDto>().ReverseMap();
    }
            
}
