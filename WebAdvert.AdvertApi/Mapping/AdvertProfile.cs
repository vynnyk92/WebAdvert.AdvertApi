using AutoMapper;
using WebAdvert.AdvertApi.Dtos;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Mapping
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDto>();
        }
    }
}
