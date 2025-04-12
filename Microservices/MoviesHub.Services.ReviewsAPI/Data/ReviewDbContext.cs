using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.ReviewsAPI.Models;

namespace MoviesHub.Services.ReviewsAPI.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Índice para obtener todas las reviews de una película (consulta común)
            builder.Entity<Review>()
                .HasIndex(r => r.MovieId);

            // Índice para obtener todas las reviews de un usuario
            builder.Entity<Review>()
                .HasIndex(r => r.UserId);

            // Índice compuesto para verificar si un usuario ya ha valorado una película
            // y para consultas que filtran por usuario y película
            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.MovieId })
                .IsUnique(); // Opcional: si solo permites una review por usuario/película

            // Índice para búsquedas por rating (ej. para mostrar reviews con 5 estrellas)
            builder.Entity<Review>()
                .HasIndex(r => r.Rating);

            // Agregar filtro de soft delete
            builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);

            builder.Entity<Review>()
                .Property(r => r.Rating)
                .HasAnnotation("MinValue", 1)
                .HasAnnotation("MaxValue", 10);
            // Ejemplo de seed para reviews (2 de 60)
            builder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    MovieId = 1,
                    UserId = "3",
                    Comment = "The Dark Knight redefines what a superhero movie can be.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-02-01"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 2,
                    MovieId = 2,
                    UserId = "4",
                    Comment = "Inception bends your mind with its dream concept.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-02-02"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 3, 
                    MovieId = 3, 
                    UserId = "5", 
                    Comment = "Mad Max delivers non-stop adrenaline with its spectacular practical effects.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-03"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 4, 
                    MovieId = 4, 
                    UserId = "6", 
                    Comment = "John Wick revolutionized action choreography. Keanu Reeves at his best.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-04"),
                    IsDeleted = false
                },
                
                new Review 
                { 
                    Id = 5, 
                    MovieId = 5, 
                    UserId = "7", 
                    Comment = "The Avengers set the standard for team-up superhero movies.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-05"),
                    IsDeleted = false
                },
                
                new Review 
                { 
                    Id = 6, 
                    MovieId = 6, 
                    UserId = "8", 
                    Comment = "Black Panther's cultural significance matches its cinematic excellence.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-06") ,
                    IsDeleted = false
                },

                // Comedy Movies (7-12)
                new Review 
                { 
                    Id = 7, 
                    MovieId = 7, 
                    UserId = "9", 
                    Comment = "Superbad captures teenage awkwardness perfectly. Endlessly quotable.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-07"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 8, 
                    MovieId = 8, 
                    UserId = "10", 
                    Comment = "The Hangover created a new template for raunchy comedies.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-08"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 9, 
                    MovieId = 9, 
                    UserId = "3", 
                    Comment = "Bridesmaids proves women can headline hilarious R-rated comedies.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-09"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 10, 
                    MovieId = 10, 
                    UserId = "4", 
                    Comment = "Deadpool breaks the fourth wall with perfect comedic timing.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-10"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 11, 
                    MovieId = 11, 
                    UserId = "5", 
                    Comment = "Wes Anderson's signature style shines in Grand Budapest Hotel.",
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-11"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 12, 
                    MovieId = 12, 
                    UserId = "6", 
                    Comment = "Booksmart is the smart, heartfelt teen comedy we needed.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-12"),
                    IsDeleted = false
                },

                // Drama Movies (13-18)
                new Review 
                { 
                    Id = 13, 
                    MovieId = 13, 
                    UserId = "7", 
                    Comment = "The Social Network's razor-sharp dialogue makes tech history thrilling.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-13"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 14, 
                    MovieId = 14, 
                    UserId = "8", 
                    Comment = "12 Years a Slave is a brutal, essential American story.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-14"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 15,
                    MovieId = 15, 
                    UserId = "9", 
                    Comment = "Parasite masterfully blends genres while delivering social commentary.", 
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-15"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 16, 
                    MovieId = 16, 
                    UserId = "10", 
                    Comment = "La La Land's magical realism makes it an instant classic.",
                    Rating = 10, 
                    CreatedAt = DateTime.Parse("2023-02-16"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 17,
                    MovieId = 17, 
                    UserId = "3", 
                    Comment = "The King's Speech proves great acting can make any subject compelling.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-17"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 18, 
                    MovieId = 18, 
                    UserId = "4", 
                    Comment = "Moonlight's intimate storytelling is profoundly moving.", 
                    Rating = 7, 
                    CreatedAt = DateTime.Parse("2023-02-18"),
                    IsDeleted = false
                },

                // Horror Movies (19-24)
                new Review 
                { 
                    Id = 19, 
                    MovieId = 19, 
                    UserId = "5", 
                    Comment = "Get Out blends horror and social satire perfectly.", 
                    Rating = 9, 
                    CreatedAt = DateTime.Parse("2023-02-19"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 20,
                    MovieId = 20, 
                    UserId = "6",
                    Comment = "Hereditary builds dread like few modern horror films.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-20"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 21, 
                    MovieId = 21, 
                    UserId = "7", 
                    Comment = "A Quiet Place's sound design creates incredible tension.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-21"),
                    IsDeleted = false
                },

                new Review 
                { 
                    Id = 22, 
                    MovieId = 22, 
                    UserId = "8", 
                    Comment = "The Conjuring sets new standards for supernatural horror.", 
                    Rating = 8, 
                    CreatedAt = DateTime.Parse("2023-02-22"),
                    IsDeleted = false
                },

                new Review
                {
                    Id = 23,
                    MovieId = 23,
                    UserId = "9",
                    Comment = "It revitalizes Stephen King adaptations with great scares.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-02-23"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 24,
                    MovieId = 24,
                    UserId = "10",
                    Comment = "The Babadook uses horror to explore grief brilliantly.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-02-24"),
                    IsDeleted = false
                },

                // Sci-Fi Movies (25-30)
                new Review
                {
                    Id = 25,
                    MovieId = 25,
                    UserId = "3",
                    Comment = "Interstellar's cosmic ambition matches its emotional depth.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-02-25"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 26,
                    MovieId = 26,
                    UserId = "4",
                    Comment = "Arrival's linguistic sci-fi approach is refreshingly original.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-02-26"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 27,
                    MovieId = 27,
                    UserId = "5",
                    Comment = "Blade Runner 2049 lives up to the original's legacy.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-02-27"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 28,
                    MovieId = 28,
                    UserId = "6",
                    Comment = "The Martian makes science exciting and accessible.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-02-28"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 29,
                    MovieId = 29,
                    UserId = "7",
                    Comment = "Ex Machina offers a chilling look at AI.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-01"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 30,
                    MovieId = 30,
                    UserId = "8",
                    Comment = "Gravity is a visually stunning survival story.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-02"),
                    IsDeleted = false
                },

                // Action Movies (31-36)
                new Review
                {
                    Id = 31,
                    MovieId = 31,
                    UserId = "9",
                    Comment = "Mad Max: Fury Road redefines the action genre with relentless energy.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-03"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 32,
                    MovieId = 32,
                    UserId = "10",
                    Comment = "John Wick delivers stylish and intense action sequences.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-04"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 33,
                    MovieId = 33,
                    UserId = "3",
                    Comment = "The Dark Knight blends superhero action with crime drama brilliantly.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-05"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 34,
                    MovieId = 34,
                    UserId = "4",
                    Comment = "Inception is an action-packed dreamscape like no other.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-06"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 35,
                    MovieId = 35,
                    UserId = "5",
                    Comment = "Skyfall delivers Bond's emotional depth with thrilling spectacle.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-07"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 36,
                    MovieId = 36,
                    UserId = "6",
                    Comment = "The Raid redefines close-quarters action choreography.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-08"),
                    IsDeleted = false
                },

                // Animated Movies (37-42)
                new Review
                {
                    Id = 37,
                    MovieId = 37,
                    UserId = "7",
                    Comment = "Inside Out is a masterclass in emotional storytelling.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-09"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 38,
                    MovieId = 38,
                    UserId = "8",
                    Comment = "Spider-Man: Into the Spider-Verse innovates animation and superhero films.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-10"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 39,
                    MovieId = 39,
                    UserId = "9",
                    Comment = "Coco is a vibrant, heartfelt celebration of family and culture.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-11"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 40,
                    MovieId = 40,
                    UserId = "10",
                    Comment = "Zootopia blends clever social commentary with adorable animation.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-12"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 41,
                    MovieId = 41,
                    UserId = "3",
                    Comment = "Toy Story 3 delivers nostalgia and deep emotional impact.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-13"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 42,
                    MovieId = 42,
                    UserId = "4",
                    Comment = "Frozen charms with its memorable songs and strong characters.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-14"),
                    IsDeleted = false
                },

                // Comedy Movies (43-48)
                new Review
                {
                    Id = 43,
                    MovieId = 43,
                    UserId = "5",
                    Comment = "The Grand Budapest Hotel is a visually delightful comedic adventure.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-15"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 44,
                    MovieId = 44,
                    UserId = "6",
                    Comment = "21 Jump Street is a hilarious and smart reboot.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-16"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 45,
                    MovieId = 45,
                    UserId = "7",
                    Comment = "Bridesmaids brings heartfelt humor with a brilliant ensemble cast.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-17"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 46,
                    MovieId = 46,
                    UserId = "8",
                    Comment = "The Hangover delivers outrageous laughs and unforgettable moments.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-18"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 47,
                    MovieId = 47,
                    UserId = "9",
                    Comment = "Superbad captures teenage awkwardness with hilarious accuracy.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-19"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 48,
                    MovieId = 48,
                    UserId = "10",
                    Comment = "Borat offers biting satire through its outrageous humor.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-20"),
                    IsDeleted = false
                },

                // Drama Movies (49-54)
                new Review
                {
                    Id = 49,
                    MovieId = 49,
                    UserId = "3",
                    Comment = "Moonlight is a profound coming-of-age story with stunning performances.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-21"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 50,
                    MovieId = 50,
                    UserId = "4",
                    Comment = "The Social Network brilliantly captures the rise of Facebook.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-22"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 51,
                    MovieId = 51,
                    UserId = "5",
                    Comment = "La La Land dazzles with its nostalgic love story and music.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-23"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 52,
                    MovieId = 52,
                    UserId = "6",
                    Comment = "Whiplash is an electrifying portrayal of ambition and obsession.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-24"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 53,
                    MovieId = 53,
                    UserId = "7",
                    Comment = "The King's Speech is a stirring historical drama with powerful performances.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-25"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 54,
                    MovieId = 54,
                    UserId = "8",
                    Comment = "Slumdog Millionaire is an uplifting tale of love and destiny.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-26"),
                    IsDeleted = false
                },

                // Thriller Movies (55-60)
                new Review
                {
                    Id = 55,
                    MovieId = 55,
                    UserId = "9",
                    Comment = "Gone Girl is a chilling, twisty psychological thriller.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-03-27"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 56,
                    MovieId = 56,
                    UserId = "10",
                    Comment = "Prisoners is a dark and gripping morality tale.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-28"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 57,
                    MovieId = 57,
                    UserId = "3",
                    Comment = "Nightcrawler is a haunting dive into media ethics and ambition.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-29"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 58,
                    MovieId = 58,
                    UserId = "4",
                    Comment = "Shutter Island keeps audiences guessing until the end.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-30"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 59,
                    MovieId = 59,
                    UserId = "5",
                    Comment = "The Girl with the Dragon Tattoo is a chilling and complex mystery.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-03-31"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 60,
                    MovieId = 60,
                    UserId = "6",
                    Comment = "Sicario delivers unrelenting tension and stark moral ambiguity.",
                    Rating = 10,
                    CreatedAt = DateTime.Parse("2023-04-01"),
                    IsDeleted = false
                },

                new Review
                {
                    Id = 61,
                    MovieId = 61,
                    UserId = "3",
                    Comment = "A fun return for Indiana Jones with exciting action sequences and nostalgic charm.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2023-07-15"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 62,
                    MovieId = 62,
                    UserId = "4",
                    Comment = "The Rock and Emily Blunt have great chemistry in this entertaining adventure inspired by the Disney ride.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2021-08-05"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 63,
                    MovieId = 63,
                    UserId = "5",
                    Comment = "Sandra Bullock and Channing Tatum make a hilarious duo in this jungle romp that doesn't take itself too seriously.",
                    Rating = 6,
                    CreatedAt = DateTime.Parse("2022-04-01"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 64,
                    MovieId = 64,
                    UserId = "6",
                    Comment = "Tom Holland brings his charm to this video game adaptation with plenty of treasure-hunting action.",
                    Rating = 6,
                    CreatedAt = DateTime.Parse("2022-02-20"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 65,
                    MovieId = 65,
                    UserId = "7",
                    Comment = "A surprisingly fun live-action adaptation that works for both kids and adults.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2019-08-15"),
                    IsDeleted = false
                },
                new Review
                {
                    Id = 66,
                    MovieId = 66,
                    UserId = "8",
                    Comment = "Ryan Reynolds shines in this heartfelt time-travel adventure with great action and humor.",
                    Rating = 8,
                    CreatedAt = DateTime.Parse("2022-03-15"),
                    IsDeleted = false
                }




            );
        }
    }
}

   
