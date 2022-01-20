using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Plants;
using WebApi.Models.Users;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();

            CreateMap<Plant, CreatePlantModel>();
            CreateMap<CreatePlantModel, Plant>();

            CreateMap<Plant, ListPlantModel>();
            CreateMap<ListPlantModel, Plant>();
        }
    }
}