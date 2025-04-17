using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoviesHub.Services.ReviewsAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReviewAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "DeletedAt", "IsDeleted", "MovieId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "The Dark Knight redefines what a superhero movie can be.", new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 1, 10, "3" },
                    { 2, "Inception bends your mind with its dream concept.", new DateTime(2023, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 2, 10, "4" },
                    { 3, "Mad Max delivers non-stop adrenaline with its spectacular practical effects.", new DateTime(2023, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 3, 8, "5" },
                    { 4, "John Wick revolutionized action choreography. Keanu Reeves at his best.", new DateTime(2023, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 4, 10, "6" },
                    { 5, "The Avengers set the standard for team-up superhero movies.", new DateTime(2023, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 5, 8, "7" },
                    { 6, "Black Panther's cultural significance matches its cinematic excellence.", new DateTime(2023, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 6, 10, "8" },
                    { 7, "Superbad captures teenage awkwardness perfectly. Endlessly quotable.", new DateTime(2023, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 7, 8, "9" },
                    { 8, "The Hangover created a new template for raunchy comedies.", new DateTime(2023, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 8, 8, "10" },
                    { 9, "Bridesmaids proves women can headline hilarious R-rated comedies.", new DateTime(2023, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 9, 8, "3" },
                    { 10, "Deadpool breaks the fourth wall with perfect comedic timing.", new DateTime(2023, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 10, 10, "4" },
                    { 11, "Wes Anderson's signature style shines in Grand Budapest Hotel.", new DateTime(2023, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 11, 10, "5" },
                    { 12, "Booksmart is the smart, heartfelt teen comedy we needed.", new DateTime(2023, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 12, 8, "6" },
                    { 13, "The Social Network's razor-sharp dialogue makes tech history thrilling.", new DateTime(2023, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 13, 10, "7" },
                    { 14, "12 Years a Slave is a brutal, essential American story.", new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 14, 10, "8" },
                    { 15, "Parasite masterfully blends genres while delivering social commentary.", new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 15, 10, "9" },
                    { 16, "La La Land's magical realism makes it an instant classic.", new DateTime(2023, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 16, 10, "10" },
                    { 17, "The King's Speech proves great acting can make any subject compelling.", new DateTime(2023, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 17, 8, "3" },
                    { 18, "Moonlight's intimate storytelling is profoundly moving.", new DateTime(2023, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 18, 7, "4" },
                    { 19, "Get Out blends horror and social satire perfectly.", new DateTime(2023, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 19, 9, "5" },
                    { 20, "Hereditary builds dread like few modern horror films.", new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 20, 8, "6" },
                    { 21, "A Quiet Place's sound design creates incredible tension.", new DateTime(2023, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 21, 8, "7" },
                    { 22, "The Conjuring sets new standards for supernatural horror.", new DateTime(2023, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 22, 8, "8" },
                    { 23, "It revitalizes Stephen King adaptations with great scares.", new DateTime(2023, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 23, 8, "9" },
                    { 24, "The Babadook uses horror to explore grief brilliantly.", new DateTime(2023, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 24, 8, "10" },
                    { 25, "Interstellar's cosmic ambition matches its emotional depth.", new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 25, 10, "3" },
                    { 26, "Arrival's linguistic sci-fi approach is refreshingly original.", new DateTime(2023, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 26, 10, "4" },
                    { 27, "Blade Runner 2049 lives up to the original's legacy.", new DateTime(2023, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 27, 10, "5" },
                    { 28, "The Martian makes science exciting and accessible.", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 28, 8, "6" },
                    { 29, "Ex Machina offers a chilling look at AI.", new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 29, 10, "7" },
                    { 30, "Gravity is a visually stunning survival story.", new DateTime(2023, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 30, 8, "8" },
                    { 31, "Mad Max: Fury Road redefines the action genre with relentless energy.", new DateTime(2023, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 31, 10, "9" },
                    { 32, "John Wick delivers stylish and intense action sequences.", new DateTime(2023, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 32, 8, "10" },
                    { 33, "The Dark Knight blends superhero action with crime drama brilliantly.", new DateTime(2023, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 33, 10, "3" },
                    { 34, "Inception is an action-packed dreamscape like no other.", new DateTime(2023, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 34, 10, "4" },
                    { 35, "Skyfall delivers Bond's emotional depth with thrilling spectacle.", new DateTime(2023, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 35, 8, "5" },
                    { 36, "The Raid redefines close-quarters action choreography.", new DateTime(2023, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 36, 8, "6" },
                    { 37, "Inside Out is a masterclass in emotional storytelling.", new DateTime(2023, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 37, 10, "7" },
                    { 38, "Spider-Man: Into the Spider-Verse innovates animation and superhero films.", new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 38, 10, "8" },
                    { 39, "Coco is a vibrant, heartfelt celebration of family and culture.", new DateTime(2023, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 39, 10, "9" },
                    { 40, "Zootopia blends clever social commentary with adorable animation.", new DateTime(2023, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 40, 8, "10" },
                    { 41, "Toy Story 3 delivers nostalgia and deep emotional impact.", new DateTime(2023, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 41, 10, "3" },
                    { 42, "Frozen charms with its memorable songs and strong characters.", new DateTime(2023, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 42, 8, "4" },
                    { 43, "The Grand Budapest Hotel is a visually delightful comedic adventure.", new DateTime(2023, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 43, 8, "5" },
                    { 44, "21 Jump Street is a hilarious and smart reboot.", new DateTime(2023, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 44, 8, "6" },
                    { 45, "Bridesmaids brings heartfelt humor with a brilliant ensemble cast.", new DateTime(2023, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 45, 8, "7" },
                    { 46, "The Hangover delivers outrageous laughs and unforgettable moments.", new DateTime(2023, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 46, 8, "8" },
                    { 47, "Superbad captures teenage awkwardness with hilarious accuracy.", new DateTime(2023, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 47, 8, "9" },
                    { 48, "Borat offers biting satire through its outrageous humor.", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 48, 8, "10" },
                    { 49, "Moonlight is a profound coming-of-age story with stunning performances.", new DateTime(2023, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 49, 10, "3" },
                    { 50, "The Social Network brilliantly captures the rise of Facebook.", new DateTime(2023, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 50, 10, "4" },
                    { 51, "La La Land dazzles with its nostalgic love story and music.", new DateTime(2023, 3, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 51, 8, "5" },
                    { 52, "Whiplash is an electrifying portrayal of ambition and obsession.", new DateTime(2023, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 52, 10, "6" },
                    { 53, "The King's Speech is a stirring historical drama with powerful performances.", new DateTime(2023, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 53, 8, "7" },
                    { 54, "Slumdog Millionaire is an uplifting tale of love and destiny.", new DateTime(2023, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 54, 8, "8" },
                    { 55, "Gone Girl is a chilling, twisty psychological thriller.", new DateTime(2023, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 55, 8, "9" },
                    { 56, "Prisoners is a dark and gripping morality tale.", new DateTime(2023, 3, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 56, 10, "10" },
                    { 57, "Nightcrawler is a haunting dive into media ethics and ambition.", new DateTime(2023, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 57, 10, "3" },
                    { 58, "Shutter Island keeps audiences guessing until the end.", new DateTime(2023, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 58, 10, "4" },
                    { 59, "The Girl with the Dragon Tattoo is a chilling and complex mystery.", new DateTime(2023, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 59, 10, "5" },
                    { 60, "Sicario delivers unrelenting tension and stark moral ambiguity.", new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 60, 10, "6" },
                    { 61, "A fun return for Indiana Jones with exciting action sequences and nostalgic charm.", new DateTime(2023, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 61, 8, "3" },
                    { 62, "The Rock and Emily Blunt have great chemistry in this entertaining adventure inspired by the Disney ride.", new DateTime(2021, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 62, 8, "4" },
                    { 63, "Sandra Bullock and Channing Tatum make a hilarious duo in this jungle romp that doesn't take itself too seriously.", new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 63, 6, "5" },
                    { 64, "Tom Holland brings his charm to this video game adaptation with plenty of treasure-hunting action.", new DateTime(2022, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 64, 6, "6" },
                    { 65, "A surprisingly fun live-action adaptation that works for both kids and adults.", new DateTime(2019, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 65, 8, "7" },
                    { 66, "Ryan Reynolds shines in this heartfelt time-travel adventure with great action and humor.", new DateTime(2022, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, false, 66, 8, "8" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Rating",
                table: "Reviews",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_MovieId",
                table: "Reviews",
                columns: new[] { "UserId", "MovieId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
