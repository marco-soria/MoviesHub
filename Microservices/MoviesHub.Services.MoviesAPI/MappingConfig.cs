using AutoMapper;
using MoviesHub.Services.MoviesAPI.Models;
using MoviesHub.Services.MoviesAPI.Models.Dto;

namespace MoviesHub.Services.MoviesAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Movie, MovieDto>().ReverseMap();
                config.CreateMap<Movie, MovieCreateDto>().ReverseMap();
                config.CreateMap<Movie, MovieUpdateDto>().ReverseMap();

                config.CreateMap<MovieGenre, MovieGenreCreateDto>().ReverseMap();
                config.CreateMap<MovieGenre, MovieGenreDto>().ReverseMap();
                config.CreateMap<MovieGenre, MovieGenreUpdateDto>().ReverseMap();

                config.CreateMap<Genre, GenreDto>().ReverseMap();
                config.CreateMap<Genre, GenreCreateDto>().ReverseMap();
                config.CreateMap<Genre, GenreUpdateDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
