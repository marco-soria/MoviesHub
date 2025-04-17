using AutoMapper;
using MoviesHub.Services.ReviewsAPI.Models;
using MoviesHub.Services.ReviewsAPI.Models.Dto;

namespace MoviesHub.Services.ReviewsAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
               
                config.CreateMap<Review, ReviewDto>().ReverseMap();
                config.CreateMap<Review, ReviewCreateDto>().ReverseMap();
                config.CreateMap<Review, ReviewUpdateDto>().ReverseMap();
                config.CreateMap<Review, ReviewResponseDto>().ReverseMap();

                
            });
            return mappingConfig;
        }
    }
}
