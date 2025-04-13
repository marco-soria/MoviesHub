using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.MoviesAPI.Models;
using System.Reflection.Emit;

namespace MoviesHub.Services.MoviesAPI.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índice para búsquedas por título 
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Title);

            // Índice para filtrar por año de lanzamiento
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.ReleaseYear);

            // Índice para búsquedas por rating (para mostrar películas mejor valoradas)
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.AverageRating);

            // Índice para optimizar búsquedas de géneros por nombre
            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique(); // Asegura que no hay

            modelBuilder.Entity<Movie>()
                .Property(m => m.AverageRating)
                .HasPrecision(4, 2) // Permite valores como 10.00
                .HasDefaultValue(0);

            // Soft Delete en Movies
            modelBuilder.Entity<Movie>().HasQueryFilter(m => !m.IsDeleted);

            // Soft Delete en Genre
            modelBuilder.Entity<Genre>().HasQueryFilter(g => !g.IsDeleted);

            modelBuilder.Entity<MovieGenre>().HasQueryFilter(mg =>
    !mg.Genre.IsDeleted && !mg.Movie.IsDeleted);

            // Relación muchos a muchos
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);

            // Seed data - se usa directamente el array sin ToList()
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Action", IsDeleted = false },
                new Genre { Id = 2, Name = "Comedy", IsDeleted = false },
                new Genre { Id = 3, Name = "Drama", IsDeleted = false },
                new Genre { Id = 4, Name = "Horror", IsDeleted = false },
                new Genre { Id = 5, Name = "SciFi", IsDeleted = false },
                new Genre { Id = 6, Name = "Cartoon", IsDeleted = false },
                new Genre { Id = 7, Name = "Anime", IsDeleted = false },
                new Genre { Id = 8, Name = "Romance", IsDeleted = false },
                new Genre { Id = 9, Name = "Thriller", IsDeleted = false },
                new Genre { Id = 10, Name = "Fantasy", IsDeleted = false },
                new Genre { Id = 11, Name = "Adventure", IsDeleted = false }
            );

            // Ejemplo de seed para películas (2 de 60)
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "The Dark Knight",
                    Description = "When the menace known as the Joker wreaks havoc...",
                    ReleaseYear = 2008,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_UX1280.jpg",
                    AverageRating = 9.0m,
                    CreatedAt = DateTime.Parse("2020-01-15"),
                    IsDeleted = false 
                },
                new Movie
                {
                    Id = 2,
                    Title = "Inception",
                    Description = "A thief who steals corporate secrets...",
                    ReleaseYear = 2010,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_UX1280.jpg",
                    AverageRating = 8.8m,
                    CreatedAt = DateTime.Parse("2020-02-20"),
                    IsDeleted = false
                },
                new Movie 
                { 
                    Id = 3, 
                    Title = "Mad Max: Fury Road", 
                    Description = "In a post-apocalyptic wasteland, a woman rebels against a tyrannical ruler...", 
                    ReleaseYear = 2015, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BN2EwM2I5OWMtMGQyMi00Zjg1LWJkNTctZTdjYTA4OGUwZjMyXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2020-03-10"),
                    IsDeleted = false
                },
                new Movie 
                { 
                    Id = 4, 
                    Title = "John Wick", 
                    Description = "An ex-hit-man comes out of retirement to track down the gangsters that killed his dog...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTU2NjA1ODgzMF5BMl5BanBnXkFtZTgwMTM2MTI4MjE@._V1_UX1280.jpg", 
                    AverageRating = 7.4m, 
                    CreatedAt = DateTime.Parse("2020-04-05"),
                    IsDeleted = false
                },
                new Movie 
                { 
                    Id = 5, 
                    Title = "The Avengers", 
                    Description = "Earth's mightiest heroes must come together and learn to fight as a team...", 
                    ReleaseYear = 2012, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDYxNjQyMjAtNTdiOS00NGYwLWFmNTAtNThmYjU5ZGI2YTI1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2020-05-12"), 
                    IsDeleted = false 
                },

                new Movie 
                { 
                    Id = 6, 
                    Title = "Black Panther", 
                    Description = "T'Challa, heir to the hidden but advanced kingdom of Wakanda...", 
                    ReleaseYear = 2018, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTg1MTY2MjYzNV5BMl5BanBnXkFtZTgwMTc4NTMwNDI@._V1_UX1280.jpg", 
                    AverageRating = 7.3m, 
                    CreatedAt = DateTime.Parse("2020-06-18"),
                    IsDeleted = false
                },

                // Comedy (6)
                new Movie 
                { 
                    Id = 7, 
                    Title = "Superbad", 
                    Description = "Two co-dependent high school seniors are forced to deal with separation anxiety...", 
                    ReleaseYear = 2007, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTc0NjIyMjA2OF5BMl5BanBnXkFtZTcwMzIxNDE1MQ@@._V1_UX1280.jpg", 
                    AverageRating = 7.6m, 
                    CreatedAt = DateTime.Parse("2020-07-22"),
                    IsDeleted = false
                },

                new Movie 
                { 
                    Id = 8, 
                    Title = "The Hangover", 
                    Description = "Three buddies wake up from a bachelor party in Las Vegas...", 
                    ReleaseYear = 2009, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNGQwZjg5YmYtY2VkNC00NzliLTljYTctNzI5NmU3MjE2ODQzXkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2020-08-30"),
                    IsDeleted = false
                },
                new Movie 
                { 
                    Id = 9, 
                    Title = "Bridesmaids", 
                    Description = "Competition between the maid of honor and a bridesmaid...", 
                    ReleaseYear = 2011, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjAyOTMyMzUxNl5BMl5BanBnXkFtZTcwODI4MzE0NA@@._V1_UX1280.jpg", 
                    AverageRating = 6.8m, 
                    CreatedAt = DateTime.Parse("2020-09-14"),
                    IsDeleted = false
                },

                new Movie 
                { 
                    Id = 10, Title = "Deadpool", 
                    Description = "A wisecracking mercenary gets experimented on and becomes immortal...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzE5MjY1ZDgtMTkyNC00MTMyLThhMjAtZGI5OTE1NzFlZGJjXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2020-10-05"),
                    IsDeleted = false
                },
                new Movie 
                { 
                    Id = 11, 
                    Title = "The Grand Budapest Hotel", 
                    Description = "The adventures of Gustave H, a legendary concierge at a famous hotel...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMzM5NjUxOTEyMl5BMl5BanBnXkFtZTgwNjEyMDM0MDE@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2020-11-11"),
                    IsDeleted = false
                },
                    
                new Movie 
                { 
                    Id = 12, 
                    Title = "Booksmart",
                    Description = "On the eve of their high school graduation, two academic superstars...", 
                    ReleaseYear = 2019, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzBhYmMzYWQtZTMzMS00YjNkLWE4ODItOGM5YWJlY2E1YTI1XkEyXkFqcGdeQXVyODQzNTE3ODc@._V1_UX1280.jpg", 
                    AverageRating = 7.1m, 
                    CreatedAt = DateTime.Parse("2020-12-03"),
                    IsDeleted = false
                },

                // Drama (6)
                new Movie 
                { 
                    Id = 13, 
                    Title = "The Social Network", 
                    Description = "Harvard student Mark Zuckerberg creates the social networking site...", 
                    ReleaseYear = 2010, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOGUyZDUxZjEtMmIzMC00MzlmLTg4MGItZWJmMzBhZjE0Mjc1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2021-01-07"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 14, 
                    Title = "12 Years a Slave", 
                    Description = "In the antebellum United States, Solomon Northup is kidnapped and sold into slavery...", 
                    ReleaseYear = 2013, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjExMTEzODkyN15BMl5BanBnXkFtZTcwNTU4NTc4OQ@@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2021-02-14"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 15, 
                    Title = "Parasite", 
                    Description = "Greed and class discrimination threaten the newly formed symbiotic relationship...", 
                    ReleaseYear = 2019, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYWZjMjk3ZTItODQ2ZC00NTY5LWE0ZDYtZTI3MjcwN2Q5NTVkXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg", 
                    AverageRating = 8.6m, 
                    CreatedAt = DateTime.Parse("2021-03-22") },
                
                new Movie 
                { 
                    Id = 16, 
                    Title = "La La Land", 
                    Description = "While navigating their careers in Los Angeles, a pianist and an actress...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMzUzNDM2NzM2MV5BMl5BanBnXkFtZTgwNTM3NTg4OTE@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2021-04-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 17, 
                    Title = "The King's Speech", 
                    Description = "King George VI struggles with a stammer and seeks help from an unorthodox speech therapist...", 
                    ReleaseYear = 2010, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMzU5MjEwMTg2Nl5BMl5BanBnXkFtZTcwNzM3MTYxNA@@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2021-05-18"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 18, 
                    Title = "Moonlight", 
                    Description = "A young African-American man grapples with his identity and sexuality...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzQxNTIyODAxMV5BMl5BanBnXkFtZTgwNzQyMDA3OTE@._V1_UX1280.jpg", 
                    AverageRating = 7.4m, 
                    CreatedAt = DateTime.Parse("2021-06-30"),
                    IsDeleted = false
                },

                // Horror (6)
                new Movie 
                { 
                    Id = 19, 
                    Title = "Get Out", 
                    Description = "A young African-American visits his white girlfriend's parents for the weekend...", 
                    ReleaseYear = 2017, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjUxMDQwNjcyNl5BMl5BanBnXkFtZTgwNzcwMzc0MTI@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2021-07-15"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 20, 
                    Title = "Hereditary", 
                    Description = "A grieving family is haunted by tragic and disturbing occurrences...", 
                    ReleaseYear = 2018, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTU5MDg3OGItZWQ1Ny00ZGVmLTg2YTUtMzBkYzQ1YWIwZjlhXkEyXkFqcGdeQXVyNTAzMTY4MDA@._V1_UX1280.jpg", 
                    AverageRating = 7.3m, 
                    CreatedAt = DateTime.Parse("2021-08-22"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 21, 
                    Title = "A Quiet Place", 
                    Description = "In a post-apocalyptic world, a family must live in silence to avoid creatures...", 
                    ReleaseYear = 2018, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjI0MDMzNTQ0M15BMl5BanBnXkFtZTgwMTM5NzM3NDM@._V1_UX1280.jpg", 
                    AverageRating = 7.5m, 
                    CreatedAt = DateTime.Parse("2021-09-10"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 22, 
                    Title = "The Conjuring", 
                    Description = "Paranormal investigators Ed and Lorraine Warren work to help a family terrorized...", 
                    ReleaseYear = 2013, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTM3NjA1NDMyMV5BMl5BanBnXkFtZTcwMDQzNDMzOQ@@._V1_UX1280.jpg", 
                    AverageRating = 7.5m, 
                    CreatedAt = DateTime.Parse("2021-10-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 23, 
                    Title = "It", 
                    Description = "In the summer of 1989, a group of bullied kids band together to destroy a shape-shifting monster...", 
                    ReleaseYear = 2017, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZDVkZmI0YzAtNzdjYi00ZjhhLWE1ODEtMWMzMWMzNDA0NmQ4XkEyXkFqcGdeQXVyNzYzODM3Mzg@._V1_UX1280.jpg", 
                    AverageRating = 7.3m, 
                    CreatedAt = DateTime.Parse("2021-11-18"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 24, 
                    Title = "The Babadook", 
                    Description = "A single mother and her child fall into a deep well of paranoia when an eerie children's book...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTk0NzMzODc2NF5BMl5BanBnXkFtZTgwNTY5NTM1NjE@._V1_UX1280.jpg", 
                    AverageRating = 6.8m, 
                    CreatedAt = DateTime.Parse("2021-12-25"),
                    IsDeleted = false
                },

                // SciFi (6)
                new Movie 
                { 
                    Id = 25, 
                    Title = "Interstellar", 
                    Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.6m, 
                    CreatedAt = DateTime.Parse("2022-01-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 26, 
                    Title = "Arrival", 
                    Description = "A linguist is recruited by the military to communicate with alien lifeforms...",
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTExMzU0ODcxNDheQTJeQWpwZ15BbWU4MDE1OTI4MzAy._V1_UX1280.jpg", 
                    AverageRating = 7.9m, 
                    CreatedAt = DateTime.Parse("2022-02-14"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 27, 
                    Title = "Blade Runner 2049", 
                    Description = "A young blade runner's discovery of a long-buried secret leads him to track down former blade runner...", 
                    ReleaseYear = 2017, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2022-03-08"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 28, 
                    Title = "The Martian", 
                    Description = "An astronaut becomes stranded on Mars after his team assume him dead, and must rely on his ingenuity...", 
                    ReleaseYear = 2015, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTc2MTQ3MDA1Nl5BMl5BanBnXkFtZTgwODA3OTI4NjE@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2022-04-01"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 29, 
                    Title = "Ex Machina", 
                    Description = "A young programmer is selected to participate in a ground-breaking experiment in synthetic intelligence...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTUxNzc0OTIxMV5BMl5BanBnXkFtZTgwNDI3NzU2NDE@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2022-05-15"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 30, 
                    Title = "District 9", 
                    Description = "An extraterrestrial race forced to live in slum-like conditions on Earth suddenly finds a kindred spirit...", 
                    ReleaseYear = 2009, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYmY5MzJiN2UtZDFmNi00YzhjLThjNmUtMTEwZDIzYjVlY2YxXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", 
                    AverageRating = 7.9m, 
                    CreatedAt = DateTime.Parse("2022-06-20"),
                    IsDeleted = false
                },

                // Cartoon (6)
                new Movie 
                { 
                    Id = 31, 
                    Title = "Spider-Man: Into the Spider-Verse", 
                    Description = "Teen Miles Morales becomes the Spider-Man of his universe and must join with five spider-powered individuals...", 
                    ReleaseYear = 2018, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjMwNDkxMTgzOF5BMl5BanBnXkFtZTgwNTkwNTQ3NjM@._V1_UX1280.jpg", 
                    AverageRating = 8.4m, 
                    CreatedAt = DateTime.Parse("2022-07-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 32, 
                    Title = "Coco", 
                    Description = "Aspiring musician Miguel, confronted with his family's ancestral ban on music, enters the Land of the Dead...", 
                    ReleaseYear = 2017, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYjQ5NjM0Y2YtNjZkNC00ZDhkLWJjMWItN2QyNzFkMDE3ZjAxXkEyXkFqcGdeQXVyODIxMzk5NjA@._V1_UX1280.jpg", 
                    AverageRating = 8.4m, 
                    CreatedAt = DateTime.Parse("2022-08-12"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 33, 
                    Title = "Zootopia", 
                    Description = "In a city of anthropomorphic animals, a rookie bunny cop and a cynical con artist fox must work together...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTMyMjEyNzIzMV5BMl5BanBnXkFtZTgwNzIyNjU0NzE@._V1_UX1280.jpg", 
                    AverageRating = 8.0m, 
                    CreatedAt = DateTime.Parse("2022-09-18"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 34, 
                    Title = "Frozen", 
                    Description = "When the newly crowned Queen Elsa accidentally uses her power to turn things into ice to curse her home...", 
                    ReleaseYear = 2013, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTQ1MjQwMTE5OF5BMl5BanBnXkFtZTgwNjk3MTcyMDE@._V1_UX1280.jpg", 
                    AverageRating = 7.4m, 
                    CreatedAt = DateTime.Parse("2022-10-22"),
                    IsDeleted = false

                },
                
                new Movie 
                { 
                    Id = 35, 
                    Title = "The Lego Movie", 
                    Description = "An ordinary Lego construction worker, thought to be the prophesied 'Special', is recruited to join a quest...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTg4MDk1ODExN15BMl5BanBnXkFtZTgwNzIyNjg3MDE@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2022-11-30"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 36, 
                    Title = "How to Train Your Dragon", 
                    Description = "A hapless young Viking who aspires to hunt dragons becomes the unlikely friend of a young dragon himself...", 
                    ReleaseYear = 2010, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjA5NDQyMjc2NF5BMl5BanBnXkFtZTcwMjg5ODcyMw@@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2022-12-15"),
                    IsDeleted = false
                },

                // Anime (6)
                new Movie 
                { 
                    Id = 37, 
                    Title = "Your Name", 
                    Description = "Two strangers find themselves linked in a bizarre way. When a connection forms, will distance be the only thing...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BODRmZDVmNzUtZDA4ZC00NjhkLWI2M2UtN2M0ZDIzNDcxYThjL2ltYWdlXkEyXkFqcGdeQXVyNTk0MzMzODA@._V1_UX1280.jpg", 
                    AverageRating = 8.4m, 
                    CreatedAt = DateTime.Parse("2023-01-10"),
                    IsDeleted = false

                },
                
                new Movie 
                { 
                    Id = 38, 
                    Title = "Spirited Away", 
                    Description = "During her family's move to the suburbs, a sullen 10-year-old girl wanders into a world ruled by gods...", 
                    ReleaseYear = 2001, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjlmZmI5MDctNDE2YS00YWE0LWE5ZWItZDBhYWQ0NTcxNWRhXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.6m, 
                    CreatedAt = DateTime.Parse("2023-02-14"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 39, 
                    Title = "A Silent Voice", 
                    Description = "A young man is ostracized by his classmates after he bullies a deaf girl to the point where she leaves...", 
                    ReleaseYear = 2016, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZGRkOGMxYTUtZTBhYS00NzI3LWEzMDQtOWRhMmNjNjJjMzM4XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2023-03-20"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 40, 
                    Title = "Weathering With You", 
                    Description = "A high-school boy who has run away to Tokyo befriends a girl who appears to be able to manipulate the weather...", 
                    ReleaseYear = 2019, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzBmMWE1ODYtY2Q5YS00Y2NiLWI3Y2QtYjI0NGE0OTBlY2E2XkEyXkFqcGdeQXVyNjAwNDUxODI@._V1_UX1280.jpg", 
                    AverageRating = 7.5m, 
                    CreatedAt = DateTime.Parse("2023-04-05"),
                    IsDeleted = false
                },
                
                new Movie             
                { 
                    Id = 41, 
                    Title = "Princess Mononoke", 
                    Description = "On a journey to find the cure for a Tatarigami's curse, Ashitaka finds himself in the middle of a war...", 
                    ReleaseYear = 1997, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNGIzY2IzODQtNThmMi00ZDE4LWI5YzAtNzNlZTM1ZjYyYjUyXkEyXkFqcGdeQXVyODEzNjM5OTQ@._V1_UX1280.jpg", 
                    AverageRating = 8.4m, 
                    CreatedAt = DateTime.Parse("2023-05-18"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 42, 
                    Title = "Demon Slayer: Mugen Train", 
                    Description = "After his family was brutally murdered and his sister turned into a demon, Tanjiro Kamado's journey...", 
                    ReleaseYear = 2020, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BODI2NjdlYWItMTE1ZC00YzI2LTlhZGQtNzE3NzYxMTc0ZmVkXkEyXkFqcGdeQXVyNjU1OTg4OTM@._V1_UX1280.jpg", 
                    AverageRating = 8.3m, 
                    CreatedAt = DateTime.Parse("2023-06-22"),
                    IsDeleted = false
                },

                // Romance (6)
                new Movie 
                { 
                    Id = 43, 
                    Title = "The Notebook", 
                    Description = "A poor yet passionate young man falls in love with a rich young woman, giving her a sense of freedom...", 
                    ReleaseYear = 2004, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTk3OTM5Njg5M15BMl5BanBnXkFtZTYwMzA0ODI3._V1_UX1280.jpg", 
                    AverageRating = 7.8m, 
                    CreatedAt = DateTime.Parse("2023-07-30"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 44, 
                    Title = "Eternal Sunshine of the Spotless Mind", 
                    Description = "When their relationship turns sour, a couple undergoes a procedure to have each other erased from their memories...", 
                    ReleaseYear = 2004, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTY4NzcwODg3Nl5BMl5BanBnXkFtZTcwNTEwOTMyMw@@._V1_UX1280.jpg", 
                    AverageRating = 8.3m, 
                    CreatedAt = DateTime.Parse("2023-08-15"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 45, 
                    Title = "Before Sunrise", 
                    Description = "A young man and woman meet on a train in Europe, and wind up spending one evening together in Vienna...", 
                    ReleaseYear = 1995, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BZDdiZTAwYzAtMDI3Ni00OTRjLTkzN2UtMGE3MDMyZmU4NTU4XkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2023-09-20"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 46, 
                    Title = "Crazy Rich Asians", 
                    Description = "This contemporary romantic comedy, based on a global bestseller, follows native New Yorker Rachel Chu...", 
                    ReleaseYear = 2018, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTYxNDMyOTAxN15BMl5BanBnXkFtZTgwMDg1ODYzNTM@._V1_UX1280.jpg", 
                    AverageRating = 6.9m, 
                    CreatedAt = DateTime.Parse("2023-10-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 47, 
                    Title = "Silver Linings Playbook", 
                    Description = "After a stint in a mental institution, former teacher Pat Solitano moves back in with his parents...", 
                    ReleaseYear = 2012, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTM2MTI5NzA3MF5BMl5BanBnXkFtZTcwODExNTc0OA@@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2023-11-11"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 48, 
                    Title = "500 Days of Summer", 
                    Description = "After being dumped by the girl he believes to be his soulmate, hopeless romantic Tom Hansen reflects...",
                    ReleaseYear = 2009, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTk5MjM4OTU1OV5BMl5BanBnXkFtZTcwODkzNDIzMw@@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2023-12-25"),
                    IsDeleted = false
                },

                // Thriller (6)
                new Movie 
                { 
                    Id = 49, 
                    Title = "Gone Girl", 
                    Description = "With his wife's disappearance having become the focus of an intense media circus, a man sees the spotlight...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTk0MDQ3MzAzOV5BMl5BanBnXkFtZTgwNzU1NzE3MjE@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2024-01-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 50, 
                    Title = "Prisoners", 
                    Description = "When Keller Dover's daughter and her friend go missing, he takes matters into his own hands...", 
                    ReleaseYear = 2013, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTg0NTIzMjQ1NV5BMl5BanBnXkFtZTcwNDc3MzM5OQ@@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2024-02-14"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 51, 
                    Title = "Nightcrawler", 
                    Description = "When Louis Bloom, a con man desperate for work, muscles into the world of L.A. crime journalism...", 
                    ReleaseYear = 2014, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BN2U1YzdhYWMtZWUzMi00OWI1LWFkM2ItNWVjM2YxMGQ2MmNhXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", 
                    AverageRating = 7.9m, 
                    CreatedAt = DateTime.Parse("2024-03-08"),
                    IsDeleted = false
                },
                
                new Movie                 
                { 
                    Id = 52, 
                    Title = "Zodiac", 
                    Description = "In the late 1960s/early 1970s, a San Francisco cartoonist becomes an amateur detective obsessed...", 
                    ReleaseYear = 2007, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BN2UwNDc5NmEtNjVjZS00OTI5LWE5YjctMWM3ZjBiZGYwMGI2XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_UX1280.jpg", 
                    AverageRating = 7.7m, 
                    CreatedAt = DateTime.Parse("2024-04-01"),
                    IsDeleted = false
                },

                new Movie 
                { 
                    Id = 53, 
                    Title = "The Girl with the Dragon Tattoo", 
                    Description = "Journalist Mikael Blomkvist is aided in his search for a woman who has been missing for forty years...", 
                    ReleaseYear = 2011, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTczNDk4NTQ0OV5BMl5BanBnXkFtZTcwNDAxMDgxNw@@._V1_UX1280.jpg", 
                    AverageRating = 7.8m, 
                    CreatedAt = DateTime.Parse("2024-05-15"),
                    IsDeleted = false
                },

                new Movie 
                { 
                    Id = 54, 
                    Title = "Shutter Island", 
                    Description = "In 1954, a U.S. Marshal investigates the disappearance of a murderer who escaped from a hospital...", 
                    ReleaseYear = 2010, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzhiNDkyNzktNTZmYS00ZTBkLTk2MDAtM2U0YjU1MzgxZjgzXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", 
                    AverageRating = 8.2m, 
                    CreatedAt = DateTime.Parse("2024-06-20"),
                    IsDeleted = false
                },

                // Fantasy (6)
                new Movie 
                { 
                    Id = 55, 
                    Title = "The Lord of the Rings: The Fellowship of the Ring", 
                    Description = "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring...", 
                    ReleaseYear = 2001, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BN2EyZjM3NzUtNWUzMi00MTgxLWI0NTctMzY4M2VlOTdjZWRiXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_UX1280.jpg", 
                    AverageRating = 8.8m, 
                    CreatedAt = DateTime.Parse("2024-07-05"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 56, 
                    Title = "Pan's Labyrinth", 
                    Description = "In the falangist Spain of 1944, the bookish young stepdaughter of a sadistic army officer escapes...", 
                    ReleaseYear = 2006, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTU3ODg2NjQ5NF5BMl5BanBnXkFtZTcwMDEwODgzMQ@@._V1_UX1280.jpg", 
                    AverageRating = 8.2m, 
                    CreatedAt = DateTime.Parse("2024-08-12"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 57, 
                    Title = "Stardust", 
                    Description = "In a countryside town bordering on a magical land, a young man makes a promise to his beloved...", 
                    ReleaseYear = 2007, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjkyMTE1OTYwNF5BMl5BanBnXkFtZTcwMDIxODYzMw@@._V1_UX1280.jpg", 
                    AverageRating = 7.6m, 
                    CreatedAt = DateTime.Parse("2024-09-18"),
                    IsDeleted = false
                },

                new Movie 
                { 
                    Id = 58, 
                    Title = "The Shape of Water", 
                    Description = "At a top secret research facility in the 1960s, a lonely janitor forms a unique relationship...", 
                    ReleaseYear = 2017, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNGNiNWQ5M2MtNGI0OC00MDA2LWI5NzEtMmZiYjVjMDEyOWYzXkEyXkFqcGdeQXVyMjM4NTM5NDY@._V1_UX1280.jpg", 
                    AverageRating = 7.3m, 
                    CreatedAt = DateTime.Parse("2024-09-18"),
                    IsDeleted = false
                },
                
                new Movie 
                {                  
                    Id = 59, 
                    Title = "Harry Potter and the Deathly Hallows: Part 2", 
                    Description = "Harry, Ron, and Hermione search for Voldemort's remaining Horcruxes in their effort to destroy...", 
                    ReleaseYear = 2011, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMGVmMWNiMDktYjQ0Mi00MWIxLTk0N2UtN2ZlYTdkN2IzNDNlXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_UX1280.jpg", 
                    AverageRating = 8.1m, 
                    CreatedAt = DateTime.Parse("2024-10-22"),
                    IsDeleted = false
                },
                
                new Movie 
                { 
                    Id = 60, 
                    Title = "The Chronicles of Narnia: The Lion, the Witch and the Wardrobe",
                    Description = "Four kids travel through a wardrobe to the land of Narnia and learn of their destiny to free it...", 
                    ReleaseYear = 2005, 
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMTc0NTUwMTU5OV5BMl5BanBnXkFtZTcwNjAwNzQzMw@@._V1_UX1280.jpg", 
                    AverageRating = 6.9m, 
                    CreatedAt = DateTime.Parse("2024-11-30"),
                    IsDeleted = false
                },

                new Movie
                {
                    Id = 61,
                    Title = "Indiana Jones and the Dial of Destiny",
                    Description = "Archaeologist Indiana Jones races against time to retrieve a legendary artifact that can change the course of history.",
                    ReleaseYear = 2023,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BYzhmODMzYzMtNTM5NS00NDQyLWEyMjYtYzBiYzExYjU0MTJlXkEyXkFqcGdeQXVyMTUzMTg2ODkz._V1_UX1280.jpg",
                    AverageRating = 6.7m,
                    CreatedAt = DateTime.Parse("2023-07-01"),
                    IsDeleted = false
                },

                new Movie
                {
                    Id = 62,
                    Title = "Jungle Cruise",
                    Description = "Based on Disneyland's theme park ride where a small riverboat takes a group of travelers through a jungle filled with dangerous animals and reptiles.",
                    ReleaseYear = 2021,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BNDE1MGRlNTQtZjc4ZC00MTI0LWEwY2MtODk1YTM2NmFmYTNmXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg",
                    AverageRating = 6.6m,
                    CreatedAt = DateTime.Parse("2021-07-30"),
                    IsDeleted = false
                },

                new Movie
                {
                    Id = 63,
                    Title = "The Lost City",
                    Description = "A reclusive romance novelist on a book tour with her cover model gets swept up in a kidnapping attempt that lands them both in a cutthroat jungle adventure.",
                    ReleaseYear = 2022,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMmIwYzFhODAtY2I1YS00ZDdmLTkyYWQtZjI5NDIwMDc2MjEyXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg",
                    AverageRating = 6.1m,
                    CreatedAt = DateTime.Parse("2022-03-25"),
                    IsDeleted = false
                },

                new Movie
                {
                    Id = 64,
                    Title = "Uncharted",
                    Description = "Street-smart Nathan Drake is recruited by seasoned treasure hunter Victor Sullivan to recover a fortune amassed by Ferdinand Magellan.",
                    ReleaseYear = 2022,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BMWEwNjhkYzYtNjgzYy00YTY2LThjYWYtYzViMGJkZTI4Y2MyXkEyXkFqcGdeQXVyNTM0OTY1OQ@@._V1_UX1280.jpg",
                    AverageRating = 6.3m,
                    CreatedAt = DateTime.Parse("2022-02-18"),
                    IsDeleted = false
                },

                new Movie
                {
                    Id = 65,
                    Title = "Dora and the Lost City of Gold",
                    Description = "Dora, a teenage explorer, leads her friends on an adventure to save her parents and solve the mystery behind a lost city of gold.",
                    ReleaseYear = 2019,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOTVhMzYxNjgtYzYwOC00MGIwLWJmZGEtMjgwMzgxMWUwNmRhXkEyXkFqcGdeQXVyNjg2NjQwMDQ@._V1_UX1280.jpg",
                    AverageRating = 6.1m,
                    CreatedAt = DateTime.Parse("2019-08-09"),
                    IsDeleted = false
                },
                new Movie
                {
                    Id = 66,
                    Title = "The Adam Project",
                    Description = "After accidentally crash-landing in 2022, time-traveling fighter pilot Adam Reed teams up with his 12-year-old self for a mission to save the future.",
                    ReleaseYear = 2022,
                    ImageUrl = "https://m.media-amazon.com/images/M/MV5BOWM0YWMwMDQtMjE5NS00ZTIwLWE1NWEtODViMWZjMWI2OTU3XkEyXkFqcGdeQXVyMTEyMjM2NDc2._V1_UX1280.jpg",
                    AverageRating = 6.7m,
                    CreatedAt = DateTime.Parse("2022-03-11"),
                    IsDeleted = false
                }



             );

            // Ejemplo de relaciones (2 de 60)
            modelBuilder.Entity<MovieGenre>().HasData(

                new MovieGenre { MovieId = 1, GenreId = 1 }, // The Dark Knight - Action
                new MovieGenre { MovieId = 1, GenreId = 9 }, // The Dark Knight - Thriller
                new MovieGenre { MovieId = 2, GenreId = 1 }, // Inception - Action
                new MovieGenre { MovieId = 2, GenreId = 5 }, // Inception - SciFi
                new MovieGenre { MovieId = 3, GenreId = 1 }, // Mad Max - Action
                new MovieGenre { MovieId = 4, GenreId = 1 }, // John Wick - Action
                new MovieGenre { MovieId = 5, GenreId = 1 }, // Avengers - Action
                new MovieGenre { MovieId = 5, GenreId = 5 }, // Avengers - SciFi
                new MovieGenre { MovieId = 6, GenreId = 1 }, // Black Panther - Action
                new MovieGenre { MovieId = 6, GenreId = 5 }, // Black Panther - SciFi

                // Comedy (7-12)
                new MovieGenre { MovieId = 7, GenreId = 2 }, // Superbad - Comedy
                new MovieGenre { MovieId = 8, GenreId = 2 }, // The Hangover - Comedy
                new MovieGenre { MovieId = 9, GenreId = 2 }, // Bridesmaids - Comedy
                new MovieGenre { MovieId = 9, GenreId = 8 }, // Bridesmaids - Romance
                new MovieGenre { MovieId = 10, GenreId = 2 }, // Deadpool - Comedy
                new MovieGenre { MovieId = 10, GenreId = 1 }, // Deadpool - Action
                new MovieGenre { MovieId = 11, GenreId = 2 }, // Grand Budapest - Comedy
                new MovieGenre { MovieId = 11, GenreId = 3 }, // Grand Budapest - Drama
                new MovieGenre { MovieId = 12, GenreId = 2 }, // Booksmart - Comedy
                new MovieGenre { MovieId = 12, GenreId = 3 }, // Booksmart - Drama

                // Drama (13-18)
                new MovieGenre { MovieId = 13, GenreId = 3 }, // Social Network - Drama
                new MovieGenre { MovieId = 14, GenreId = 3 }, // 12 Years a Slave - Drama
                new MovieGenre { MovieId = 15, GenreId = 3 }, // Parasite - Drama
                new MovieGenre { MovieId = 15, GenreId = 9 }, // Parasite - Thriller
                new MovieGenre { MovieId = 16, GenreId = 3 }, // La La Land - Drama
                new MovieGenre { MovieId = 16, GenreId = 8 }, // La La Land - Romance
                new MovieGenre { MovieId = 17, GenreId = 3 }, // King's Speech - Drama
                new MovieGenre { MovieId = 18, GenreId = 3 }, // Moonlight - Drama

                // Horror (19-24)
                new MovieGenre { MovieId = 19, GenreId = 4 }, // Get Out - Horror
                new MovieGenre { MovieId = 19, GenreId = 9 }, // Get Out - Thriller
                new MovieGenre { MovieId = 20, GenreId = 4 }, // Hereditary - Horror
                new MovieGenre { MovieId = 21, GenreId = 4 }, // A Quiet Place - Horror
                new MovieGenre { MovieId = 21, GenreId = 9 }, // A Quiet Place - Thriller
                new MovieGenre { MovieId = 22, GenreId = 4 }, // The Conjuring - Horror
                new MovieGenre { MovieId = 23, GenreId = 4 }, // It - Horror
                new MovieGenre { MovieId = 24, GenreId = 4 }, // The Babadook - Horror

                // SciFi (25-30)
                new MovieGenre { MovieId = 25, GenreId = 5 }, // Interstellar - SciFi
                new MovieGenre { MovieId = 25, GenreId = 3 }, // Interstellar - Drama
                new MovieGenre { MovieId = 26, GenreId = 5 }, // Arrival - SciFi
                new MovieGenre { MovieId = 26, GenreId = 3 }, // Arrival - Drama
                new MovieGenre { MovieId = 27, GenreId = 5 }, // Blade Runner 2049 - SciFi
                new MovieGenre { MovieId = 28, GenreId = 5 }, // The Martian - SciFi
                new MovieGenre { MovieId = 29, GenreId = 5 }, // Ex Machina - SciFi
                new MovieGenre { MovieId = 29, GenreId = 9 }, // Ex Machina - Thriller
                new MovieGenre { MovieId = 30, GenreId = 5 }, // District 9 - SciFi

                // Cartoon (31-36)
                new MovieGenre { MovieId = 31, GenreId = 6 }, // Spider-Verse - Cartoon
                new MovieGenre { MovieId = 31, GenreId = 1 }, // Spider-Verse - Action
                new MovieGenre { MovieId = 32, GenreId = 6 }, // Coco - Cartoon
                new MovieGenre { MovieId = 32, GenreId = 10 }, // Coco - Fantasy
                new MovieGenre { MovieId = 33, GenreId = 6 }, // Zootopia - Cartoon
                new MovieGenre { MovieId = 34, GenreId = 6 }, // Frozen - Cartoon
                new MovieGenre { MovieId = 34, GenreId = 10 }, // Frozen - Fantasy
                new MovieGenre { MovieId = 35, GenreId = 6 }, // Lego Movie - Cartoon
                new MovieGenre { MovieId = 36, GenreId = 6 }, // How to Train Your Dragon - Cartoon
                new MovieGenre { MovieId = 36, GenreId = 10 }, // How to Train Your Dragon - Fantasy

                // Anime (37-42)
                new MovieGenre { MovieId = 37, GenreId = 7 }, // Your Name - Anime
                new MovieGenre { MovieId = 37, GenreId = 8 }, // Your Name - Romance
                new MovieGenre { MovieId = 38, GenreId = 7 }, // Spirited Away - Anime
                new MovieGenre { MovieId = 38, GenreId = 10 }, // Spirited Away - Fantasy
                new MovieGenre { MovieId = 39, GenreId = 7 }, // A Silent Voice - Anime
                new MovieGenre { MovieId = 39, GenreId = 3 }, // A Silent Voice - Drama
                new MovieGenre { MovieId = 40, GenreId = 7 }, // Weathering With You - Anime
                new MovieGenre { MovieId = 41, GenreId = 7 }, // Princess Mononoke - Anime
                new MovieGenre { MovieId = 41, GenreId = 10 }, // Princess Mononoke - Fantasy
                new MovieGenre { MovieId = 42, GenreId = 7 }, // Demon Slayer - Anime
                new MovieGenre { MovieId = 42, GenreId = 1 }, // Demon Slayer - Action

                // Romance (43-48)
                new MovieGenre { MovieId = 43, GenreId = 8 }, // The Notebook - Romance
                new MovieGenre { MovieId = 43, GenreId = 3 }, // The Notebook - Drama
                new MovieGenre { MovieId = 44, GenreId = 8 }, // Eternal Sunshine - Romance
                new MovieGenre { MovieId = 44, GenreId = 5 }, // Eternal Sunshine - SciFi
                new MovieGenre { MovieId = 45, GenreId = 8 }, // Before Sunrise - Romance
                new MovieGenre { MovieId = 46, GenreId = 8 }, // Crazy Rich Asians - Romance
                new MovieGenre { MovieId = 46, GenreId = 2 }, // Crazy Rich Asians - Comedy
                new MovieGenre { MovieId = 47, GenreId = 8 }, // Silver Linings - Romance
                new MovieGenre { MovieId = 47, GenreId = 3 }, // Silver Linings - Drama
                new MovieGenre { MovieId = 48, GenreId = 8 }, // 500 Days of Summer - Romance
                new MovieGenre { MovieId = 48, GenreId = 2 }, // 500 Days of Summer - Comedy

                // Thriller (49-54)
                new MovieGenre { MovieId = 49, GenreId = 9 }, // Gone Girl - Thriller
                new MovieGenre { MovieId = 49, GenreId = 3 }, // Gone Girl - Drama
                new MovieGenre { MovieId = 50, GenreId = 9 }, // Prisoners - Thriller
                new MovieGenre { MovieId = 51, GenreId = 9 }, // Nightcrawler - Thriller
                new MovieGenre { MovieId = 52, GenreId = 9 }, // Zodiac - Thriller
                new MovieGenre { MovieId = 53, GenreId = 9 }, // Girl with Dragon Tattoo - Thriller
                new MovieGenre { MovieId = 54, GenreId = 9 }, // Shutter Island - Thriller

                // Fantasy (55-60)
                new MovieGenre { MovieId = 55, GenreId = 10 }, // LOTR - Fantasy
                new MovieGenre { MovieId = 55, GenreId = 1 }, // LOTR - Action
                new MovieGenre { MovieId = 56, GenreId = 10 }, // Pan's Labyrinth - Fantasy
                new MovieGenre { MovieId = 57, GenreId = 10 }, // Stardust - Fantasy
                new MovieGenre { MovieId = 57, GenreId = 8 }, // Stardust - Romance
                new MovieGenre { MovieId = 58, GenreId = 10 }, // Shape of Water - Fantasy
                new MovieGenre { MovieId = 58, GenreId = 8 }, // Shape of Water - Romance
                new MovieGenre { MovieId = 59, GenreId = 10 }, // Harry Potter - Fantasy
                new MovieGenre { MovieId = 60, GenreId = 10 }, // Narnia - Fantasy
                new MovieGenre { MovieId = 60, GenreId = 1 },  // Narnia - Action

                //Adventure(61-66)
                new MovieGenre { MovieId = 61, GenreId = 11 }, // Indiana Jones - Adventure
                new MovieGenre { MovieId = 61, GenreId = 1 },  // Indiana Jones - Action
                new MovieGenre { MovieId = 62, GenreId = 11 }, // Jungle Cruise - Adventure
                new MovieGenre { MovieId = 62, GenreId = 2 },  // Jungle Cruise - Comedy
                new MovieGenre { MovieId = 63, GenreId = 11 }, // The Lost City - Adventure
                new MovieGenre { MovieId = 63, GenreId = 2 },  // The Lost City - Comedy
                new MovieGenre { MovieId = 64, GenreId = 11 }, // Uncharted - Adventure
                new MovieGenre { MovieId = 64, GenreId = 1 },  // Uncharted - Action
                new MovieGenre { MovieId = 65, GenreId = 11 }, // Dora - Adventure
                new MovieGenre { MovieId = 65, GenreId = 6 },  // Dora - Cartoon
                new MovieGenre { MovieId = 66, GenreId = 11 }, // The Adam Project - Adventure
                new MovieGenre { MovieId = 66, GenreId = 5 }   // The Adam Project - SciFi

            );
        }
    }
}
