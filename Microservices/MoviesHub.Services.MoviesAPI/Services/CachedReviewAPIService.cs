using Microsoft.Extensions.Caching.Memory;
using MoviesHub.Services.MoviesAPI.Services.IServices;

namespace MoviesHub.Services.MoviesAPI.Services
{
    public class CachedReviewAPIService : IReviewAPIService
    {
        private readonly IReviewAPIService _decoratedService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedReviewAPIService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15); // Caché de 15 minutos

        public CachedReviewAPIService(
            IReviewAPIService decoratedService,
            IMemoryCache cache,
            ILogger<CachedReviewAPIService> logger)
        {
            _decoratedService = decoratedService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<double> GetAverageRatingAsync(int movieId)
        {
            string cacheKey = $"movie_rating_{movieId}";

            // Intentar obtener de la caché
            if (_cache.TryGetValue(cacheKey, out double cachedRating))
            {
                _logger.LogInformation("Cache hit for movie rating {MovieId}: {Rating}", movieId, cachedRating);
                return cachedRating;
            }

            // Si no está en la caché, obtenerlo del servicio original
            _logger.LogInformation("Cache miss for movie rating {MovieId}, fetching from source", movieId);
            double rating = await _decoratedService.GetAverageRatingAsync(movieId);

            // Guardar en la caché
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheDuration)
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(cacheKey, rating, cacheOptions);
            _logger.LogInformation("Cached movie rating {MovieId}: {Rating} for {Duration} minutes",
                movieId, rating, _cacheDuration.TotalMinutes);

            return rating;
        }

        // Implementar los otros métodos de la interfaz
        public async Task<T> GetMovieRatingsAsync<T>(int movieId)
        {
            string cacheKey = $"movie_ratings_detail_{movieId}";

            // Intentar obtener de la caché
            if (_cache.TryGetValue(cacheKey, out T cachedRatings))
            {
                _logger.LogInformation("Cache hit for movie ratings {MovieId}", movieId);
                return cachedRatings;
            }

            // Si no está en la caché, obtenerlo del servicio original
            _logger.LogInformation("Cache miss for movie ratings {MovieId}, fetching from source", movieId);
            T ratings = await _decoratedService.GetMovieRatingsAsync<T>(movieId);

            if (ratings != null)
            {
                // Guardar en la caché
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration)
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(cacheKey, ratings, cacheOptions);
                _logger.LogInformation("Cached movie ratings {MovieId} for {Duration} minutes",
                    movieId, _cacheDuration.TotalMinutes);
            }

            return ratings;
        }

        public void InvalidateMovieRatingCache(int movieId)
        {
            string ratingKey = $"movie_rating_{movieId}";
            string detailKey = $"movie_ratings_detail_{movieId}";

            _cache.Remove(ratingKey);
            _cache.Remove(detailKey);

            _logger.LogInformation("Invalidated cache for movie {MovieId}", movieId);
        }
    }
}
