using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoviesHub.Services.MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false, defaultValue: 0m),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "DeletedAt", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, null, false, "Action" },
                    { 2, null, false, "Comedy" },
                    { 3, null, false, "Drama" },
                    { 4, null, false, "Horror" },
                    { 5, null, false, "SciFi" },
                    { 6, null, false, "Cartoon" },
                    { 7, null, false, "Anime" },
                    { 8, null, false, "Romance" },
                    { 9, null, false, "Thriller" },
                    { 10, null, false, "Fantasy" },
                    { 11, null, false, "Adventure" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AverageRating", "CreatedAt", "DeletedAt", "Description", "ImageUrl", "IsDeleted", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, 9.0m, new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "When the menace known as the Joker wreaks havoc...", "https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_UX1280.jpg", false, 2008, "The Dark Knight" },
                    { 2, 8.8m, new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A thief who steals corporate secrets...", "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_UX1280.jpg", false, 2010, "Inception" },
                    { 3, 8.1m, new DateTime(2020, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In a post-apocalyptic wasteland, a woman rebels against a tyrannical ruler...", "https://m.media-amazon.com/images/M/MV5BN2EwM2I5OWMtMGQyMi00Zjg1LWJkNTctZTdjYTA4OGUwZjMyXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2015, "Mad Max: Fury Road" },
                    { 4, 7.4m, new DateTime(2020, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "An ex-hit-man comes out of retirement to track down the gangsters that killed his dog...", "https://m.media-amazon.com/images/M/MV5BMTU2NjA1ODgzMF5BMl5BanBnXkFtZTgwMTM2MTI4MjE@._V1_UX1280.jpg", false, 2014, "John Wick" },
                    { 5, 8.0m, new DateTime(2020, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Earth's mightiest heroes must come together and learn to fight as a team...", "https://m.media-amazon.com/images/M/MV5BNDYxNjQyMjAtNTdiOS00NGYwLWFmNTAtNThmYjU5ZGI2YTI1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2012, "The Avengers" },
                    { 6, 7.3m, new DateTime(2020, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "T'Challa, heir to the hidden but advanced kingdom of Wakanda...", "https://m.media-amazon.com/images/M/MV5BMTg1MTY2MjYzNV5BMl5BanBnXkFtZTgwMTc4NTMwNDI@._V1_UX1280.jpg", false, 2018, "Black Panther" },
                    { 7, 7.6m, new DateTime(2020, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Two co-dependent high school seniors are forced to deal with separation anxiety...", "https://m.media-amazon.com/images/M/MV5BMTc0NjIyMjA2OF5BMl5BanBnXkFtZTcwMzIxNDE1MQ@@._V1_UX1280.jpg", false, 2007, "Superbad" },
                    { 8, 7.7m, new DateTime(2020, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Three buddies wake up from a bachelor party in Las Vegas...", "https://m.media-amazon.com/images/M/MV5BNGQwZjg5YmYtY2VkNC00NzliLTljYTctNzI5NmU3MjE2ODQzXkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_UX1280.jpg", false, 2009, "The Hangover" },
                    { 9, 6.8m, new DateTime(2020, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Competition between the maid of honor and a bridesmaid...", "https://m.media-amazon.com/images/M/MV5BMjAyOTMyMzUxNl5BMl5BanBnXkFtZTcwODI4MzE0NA@@._V1_UX1280.jpg", false, 2011, "Bridesmaids" },
                    { 10, 8.0m, new DateTime(2020, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A wisecracking mercenary gets experimented on and becomes immortal...", "https://m.media-amazon.com/images/M/MV5BYzE5MjY1ZDgtMTkyNC00MTMyLThhMjAtZGI5OTE1NzFlZGJjXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", false, 2016, "Deadpool" },
                    { 11, 8.1m, new DateTime(2020, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "The adventures of Gustave H, a legendary concierge at a famous hotel...", "https://m.media-amazon.com/images/M/MV5BMzM5NjUxOTEyMl5BMl5BanBnXkFtZTgwNjEyMDM0MDE@._V1_UX1280.jpg", false, 2014, "The Grand Budapest Hotel" },
                    { 12, 7.1m, new DateTime(2020, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "On the eve of their high school graduation, two academic superstars...", "https://m.media-amazon.com/images/M/MV5BYzBhYmMzYWQtZTMzMS00YjNkLWE4ODItOGM5YWJlY2E1YTI1XkEyXkFqcGdeQXVyODQzNTE3ODc@._V1_UX1280.jpg", false, 2019, "Booksmart" },
                    { 13, 7.7m, new DateTime(2021, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Harvard student Mark Zuckerberg creates the social networking site...", "https://m.media-amazon.com/images/M/MV5BOGUyZDUxZjEtMmIzMC00MzlmLTg4MGItZWJmMzBhZjE0Mjc1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2010, "The Social Network" },
                    { 14, 8.1m, new DateTime(2021, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In the antebellum United States, Solomon Northup is kidnapped and sold into slavery...", "https://m.media-amazon.com/images/M/MV5BMjExMTEzODkyN15BMl5BanBnXkFtZTcwNTU4NTc4OQ@@._V1_UX1280.jpg", false, 2013, "12 Years a Slave" },
                    { 15, 8.6m, new DateTime(2021, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Greed and class discrimination threaten the newly formed symbiotic relationship...", "https://m.media-amazon.com/images/M/MV5BYWZjMjk3ZTItODQ2ZC00NTY5LWE0ZDYtZTI3MjcwN2Q5NTVkXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg", false, 2019, "Parasite" },
                    { 16, 8.0m, new DateTime(2021, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "While navigating their careers in Los Angeles, a pianist and an actress...", "https://m.media-amazon.com/images/M/MV5BMzUzNDM2NzM2MV5BMl5BanBnXkFtZTgwNTM3NTg4OTE@._V1_UX1280.jpg", false, 2016, "La La Land" },
                    { 17, 8.0m, new DateTime(2021, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "King George VI struggles with a stammer and seeks help from an unorthodox speech therapist...", "https://m.media-amazon.com/images/M/MV5BMzU5MjEwMTg2Nl5BMl5BanBnXkFtZTcwNzM3MTYxNA@@._V1_UX1280.jpg", false, 2010, "The King's Speech" },
                    { 18, 7.4m, new DateTime(2021, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young African-American man grapples with his identity and sexuality...", "https://m.media-amazon.com/images/M/MV5BNzQxNTIyODAxMV5BMl5BanBnXkFtZTgwNzQyMDA3OTE@._V1_UX1280.jpg", false, 2016, "Moonlight" },
                    { 19, 7.7m, new DateTime(2021, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young African-American visits his white girlfriend's parents for the weekend...", "https://m.media-amazon.com/images/M/MV5BMjUxMDQwNjcyNl5BMl5BanBnXkFtZTgwNzcwMzc0MTI@._V1_UX1280.jpg", false, 2017, "Get Out" },
                    { 20, 7.3m, new DateTime(2021, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A grieving family is haunted by tragic and disturbing occurrences...", "https://m.media-amazon.com/images/M/MV5BOTU5MDg3OGItZWQ1Ny00ZGVmLTg2YTUtMzBkYzQ1YWIwZjlhXkEyXkFqcGdeQXVyNTAzMTY4MDA@._V1_UX1280.jpg", false, 2018, "Hereditary" },
                    { 21, 7.5m, new DateTime(2021, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In a post-apocalyptic world, a family must live in silence to avoid creatures...", "https://m.media-amazon.com/images/M/MV5BMjI0MDMzNTQ0M15BMl5BanBnXkFtZTgwMTM5NzM3NDM@._V1_UX1280.jpg", false, 2018, "A Quiet Place" },
                    { 22, 7.5m, new DateTime(2021, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Paranormal investigators Ed and Lorraine Warren work to help a family terrorized...", "https://m.media-amazon.com/images/M/MV5BMTM3NjA1NDMyMV5BMl5BanBnXkFtZTcwMDQzNDMzOQ@@._V1_UX1280.jpg", false, 2013, "The Conjuring" },
                    { 23, 7.3m, new DateTime(2021, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In the summer of 1989, a group of bullied kids band together to destroy a shape-shifting monster...", "https://m.media-amazon.com/images/M/MV5BZDVkZmI0YzAtNzdjYi00ZjhhLWE1ODEtMWMzMWMzNDA0NmQ4XkEyXkFqcGdeQXVyNzYzODM3Mzg@._V1_UX1280.jpg", false, 2017, "It" },
                    { 24, 6.8m, new DateTime(2021, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A single mother and her child fall into a deep well of paranoia when an eerie children's book...", "https://m.media-amazon.com/images/M/MV5BMTk0NzMzODc2NF5BMl5BanBnXkFtZTgwNTY5NTM1NjE@._V1_UX1280.jpg", false, 2014, "The Babadook" },
                    { 25, 8.6m, new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival...", "https://m.media-amazon.com/images/M/MV5BZjdkOTU3MDktN2IxOS00OGEyLWFmMjktY2FiMmZkNWIyODZiXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2014, "Interstellar" },
                    { 26, 7.9m, new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A linguist is recruited by the military to communicate with alien lifeforms...", "https://m.media-amazon.com/images/M/MV5BMTExMzU0ODcxNDheQTJeQWpwZ15BbWU4MDE1OTI4MzAy._V1_UX1280.jpg", false, 2016, "Arrival" },
                    { 27, 8.0m, new DateTime(2022, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young blade runner's discovery of a long-buried secret leads him to track down former blade runner...", "https://m.media-amazon.com/images/M/MV5BNzA1Njg4NzYxOV5BMl5BanBnXkFtZTgwODk5NjU3MzI@._V1_UX1280.jpg", false, 2017, "Blade Runner 2049" },
                    { 28, 8.0m, new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "An astronaut becomes stranded on Mars after his team assume him dead, and must rely on his ingenuity...", "https://m.media-amazon.com/images/M/MV5BMTc2MTQ3MDA1Nl5BMl5BanBnXkFtZTgwODA3OTI4NjE@._V1_UX1280.jpg", false, 2015, "The Martian" },
                    { 29, 7.7m, new DateTime(2022, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young programmer is selected to participate in a ground-breaking experiment in synthetic intelligence...", "https://m.media-amazon.com/images/M/MV5BMTUxNzc0OTIxMV5BMl5BanBnXkFtZTgwNDI3NzU2NDE@._V1_UX1280.jpg", false, 2014, "Ex Machina" },
                    { 30, 7.9m, new DateTime(2022, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "An extraterrestrial race forced to live in slum-like conditions on Earth suddenly finds a kindred spirit...", "https://m.media-amazon.com/images/M/MV5BYmY5MzJiN2UtZDFmNi00YzhjLThjNmUtMTEwZDIzYjVlY2YxXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", false, 2009, "District 9" },
                    { 31, 8.4m, new DateTime(2022, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Teen Miles Morales becomes the Spider-Man of his universe and must join with five spider-powered individuals...", "https://m.media-amazon.com/images/M/MV5BMjMwNDkxMTgzOF5BMl5BanBnXkFtZTgwNTkwNTQ3NjM@._V1_UX1280.jpg", false, 2018, "Spider-Man: Into the Spider-Verse" },
                    { 32, 8.4m, new DateTime(2022, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Aspiring musician Miguel, confronted with his family's ancestral ban on music, enters the Land of the Dead...", "https://m.media-amazon.com/images/M/MV5BYjQ5NjM0Y2YtNjZkNC00ZDhkLWJjMWItN2QyNzFkMDE3ZjAxXkEyXkFqcGdeQXVyODIxMzk5NjA@._V1_UX1280.jpg", false, 2017, "Coco" },
                    { 33, 8.0m, new DateTime(2022, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In a city of anthropomorphic animals, a rookie bunny cop and a cynical con artist fox must work together...", "https://m.media-amazon.com/images/M/MV5BOTMyMjEyNzIzMV5BMl5BanBnXkFtZTgwNzIyNjU0NzE@._V1_UX1280.jpg", false, 2016, "Zootopia" },
                    { 34, 7.4m, new DateTime(2022, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "When the newly crowned Queen Elsa accidentally uses her power to turn things into ice to curse her home...", "https://m.media-amazon.com/images/M/MV5BMTQ1MjQwMTE5OF5BMl5BanBnXkFtZTgwNjk3MTcyMDE@._V1_UX1280.jpg", false, 2013, "Frozen" },
                    { 35, 7.7m, new DateTime(2022, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "An ordinary Lego construction worker, thought to be the prophesied 'Special', is recruited to join a quest...", "https://m.media-amazon.com/images/M/MV5BMTg4MDk1ODExN15BMl5BanBnXkFtZTgwNzIyNjg3MDE@._V1_UX1280.jpg", false, 2014, "The Lego Movie" },
                    { 36, 8.1m, new DateTime(2022, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A hapless young Viking who aspires to hunt dragons becomes the unlikely friend of a young dragon himself...", "https://m.media-amazon.com/images/M/MV5BMjA5NDQyMjc2NF5BMl5BanBnXkFtZTcwMjg5ODcyMw@@._V1_UX1280.jpg", false, 2010, "How to Train Your Dragon" },
                    { 37, 8.4m, new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Two strangers find themselves linked in a bizarre way. When a connection forms, will distance be the only thing...", "https://m.media-amazon.com/images/M/MV5BODRmZDVmNzUtZDA4ZC00NjhkLWI2M2UtN2M0ZDIzNDcxYThjL2ltYWdlXkEyXkFqcGdeQXVyNTk0MzMzODA@._V1_UX1280.jpg", false, 2016, "Your Name" },
                    { 38, 8.6m, new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "During her family's move to the suburbs, a sullen 10-year-old girl wanders into a world ruled by gods...", "https://m.media-amazon.com/images/M/MV5BMjlmZmI5MDctNDE2YS00YWE0LWE5ZWItZDBhYWQ0NTcxNWRhXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2001, "Spirited Away" },
                    { 39, 8.1m, new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young man is ostracized by his classmates after he bullies a deaf girl to the point where she leaves...", "https://m.media-amazon.com/images/M/MV5BZGRkOGMxYTUtZTBhYS00NzI3LWEzMDQtOWRhMmNjNjJjMzM4XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2016, "A Silent Voice" },
                    { 40, 7.5m, new DateTime(2023, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A high-school boy who has run away to Tokyo befriends a girl who appears to be able to manipulate the weather...", "https://m.media-amazon.com/images/M/MV5BNzBmMWE1ODYtY2Q5YS00Y2NiLWI3Y2QtYjI0NGE0OTBlY2E2XkEyXkFqcGdeQXVyNjAwNDUxODI@._V1_UX1280.jpg", false, 2019, "Weathering With You" },
                    { 41, 8.4m, new DateTime(2023, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "On a journey to find the cure for a Tatarigami's curse, Ashitaka finds himself in the middle of a war...", "https://m.media-amazon.com/images/M/MV5BNGIzY2IzODQtNThmMi00ZDE4LWI5YzAtNzNlZTM1ZjYyYjUyXkEyXkFqcGdeQXVyODEzNjM5OTQ@._V1_UX1280.jpg", false, 1997, "Princess Mononoke" },
                    { 42, 8.3m, new DateTime(2023, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "After his family was brutally murdered and his sister turned into a demon, Tanjiro Kamado's journey...", "https://m.media-amazon.com/images/M/MV5BODI2NjdlYWItMTE1ZC00YzI2LTlhZGQtNzE3NzYxMTc0ZmVkXkEyXkFqcGdeQXVyNjU1OTg4OTM@._V1_UX1280.jpg", false, 2020, "Demon Slayer: Mugen Train" },
                    { 43, 7.8m, new DateTime(2023, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A poor yet passionate young man falls in love with a rich young woman, giving her a sense of freedom...", "https://m.media-amazon.com/images/M/MV5BMTk3OTM5Njg5M15BMl5BanBnXkFtZTYwMzA0ODI3._V1_UX1280.jpg", false, 2004, "The Notebook" },
                    { 44, 8.3m, new DateTime(2023, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "When their relationship turns sour, a couple undergoes a procedure to have each other erased from their memories...", "https://m.media-amazon.com/images/M/MV5BMTY4NzcwODg3Nl5BMl5BanBnXkFtZTcwNTEwOTMyMw@@._V1_UX1280.jpg", false, 2004, "Eternal Sunshine of the Spotless Mind" },
                    { 45, 8.1m, new DateTime(2023, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A young man and woman meet on a train in Europe, and wind up spending one evening together in Vienna...", "https://m.media-amazon.com/images/M/MV5BZDdiZTAwYzAtMDI3Ni00OTRjLTkzN2UtMGE3MDMyZmU4NTU4XkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", false, 1995, "Before Sunrise" },
                    { 46, 6.9m, new DateTime(2023, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "This contemporary romantic comedy, based on a global bestseller, follows native New Yorker Rachel Chu...", "https://m.media-amazon.com/images/M/MV5BMTYxNDMyOTAxN15BMl5BanBnXkFtZTgwMDg1ODYzNTM@._V1_UX1280.jpg", false, 2018, "Crazy Rich Asians" },
                    { 47, 7.7m, new DateTime(2023, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "After a stint in a mental institution, former teacher Pat Solitano moves back in with his parents...", "https://m.media-amazon.com/images/M/MV5BMTM2MTI5NzA3MF5BMl5BanBnXkFtZTcwODExNTc0OA@@._V1_UX1280.jpg", false, 2012, "Silver Linings Playbook" },
                    { 48, 7.7m, new DateTime(2023, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "After being dumped by the girl he believes to be his soulmate, hopeless romantic Tom Hansen reflects...", "https://m.media-amazon.com/images/M/MV5BMTk5MjM4OTU1OV5BMl5BanBnXkFtZTcwODkzNDIzMw@@._V1_UX1280.jpg", false, 2009, "500 Days of Summer" },
                    { 49, 8.1m, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "With his wife's disappearance having become the focus of an intense media circus, a man sees the spotlight...", "https://m.media-amazon.com/images/M/MV5BMTk0MDQ3MzAzOV5BMl5BanBnXkFtZTgwNzU1NzE3MjE@._V1_UX1280.jpg", false, 2014, "Gone Girl" },
                    { 50, 8.1m, new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "When Keller Dover's daughter and her friend go missing, he takes matters into his own hands...", "https://m.media-amazon.com/images/M/MV5BMTg0NTIzMjQ1NV5BMl5BanBnXkFtZTcwNDc3MzM5OQ@@._V1_UX1280.jpg", false, 2013, "Prisoners" },
                    { 51, 7.9m, new DateTime(2024, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "When Louis Bloom, a con man desperate for work, muscles into the world of L.A. crime journalism...", "https://m.media-amazon.com/images/M/MV5BN2U1YzdhYWMtZWUzMi00OWI1LWFkM2ItNWVjM2YxMGQ2MmNhXkEyXkFqcGdeQXVyNjU0OTQ0OTY@._V1_UX1280.jpg", false, 2014, "Nightcrawler" },
                    { 52, 7.7m, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In the late 1960s/early 1970s, a San Francisco cartoonist becomes an amateur detective obsessed...", "https://m.media-amazon.com/images/M/MV5BN2UwNDc5NmEtNjVjZS00OTI5LWE5YjctMWM3ZjBiZGYwMGI2XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_UX1280.jpg", false, 2007, "Zodiac" },
                    { 53, 7.8m, new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Journalist Mikael Blomkvist is aided in his search for a woman who has been missing for forty years...", "https://m.media-amazon.com/images/M/MV5BMTczNDk4NTQ0OV5BMl5BanBnXkFtZTcwNDAxMDgxNw@@._V1_UX1280.jpg", false, 2011, "The Girl with the Dragon Tattoo" },
                    { 54, 8.2m, new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In 1954, a U.S. Marshal investigates the disappearance of a murderer who escaped from a hospital...", "https://m.media-amazon.com/images/M/MV5BYzhiNDkyNzktNTZmYS00ZTBkLTk2MDAtM2U0YjU1MzgxZjgzXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_UX1280.jpg", false, 2010, "Shutter Island" },
                    { 55, 8.8m, new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring...", "https://m.media-amazon.com/images/M/MV5BN2EyZjM3NzUtNWUzMi00MTgxLWI0NTctMzY4M2VlOTdjZWRiXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_UX1280.jpg", false, 2001, "The Lord of the Rings: The Fellowship of the Ring" },
                    { 56, 8.2m, new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In the falangist Spain of 1944, the bookish young stepdaughter of a sadistic army officer escapes...", "https://m.media-amazon.com/images/M/MV5BMTU3ODg2NjQ5NF5BMl5BanBnXkFtZTcwMDEwODgzMQ@@._V1_UX1280.jpg", false, 2006, "Pan's Labyrinth" },
                    { 57, 7.6m, new DateTime(2024, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "In a countryside town bordering on a magical land, a young man makes a promise to his beloved...", "https://m.media-amazon.com/images/M/MV5BMjkyMTE1OTYwNF5BMl5BanBnXkFtZTcwMDIxODYzMw@@._V1_UX1280.jpg", false, 2007, "Stardust" },
                    { 58, 7.3m, new DateTime(2024, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "At a top secret research facility in the 1960s, a lonely janitor forms a unique relationship...", "https://m.media-amazon.com/images/M/MV5BNGNiNWQ5M2MtNGI0OC00MDA2LWI5NzEtMmZiYjVjMDEyOWYzXkEyXkFqcGdeQXVyMjM4NTM5NDY@._V1_UX1280.jpg", false, 2017, "The Shape of Water" },
                    { 59, 8.1m, new DateTime(2024, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Harry, Ron, and Hermione search for Voldemort's remaining Horcruxes in their effort to destroy...", "https://m.media-amazon.com/images/M/MV5BMGVmMWNiMDktYjQ0Mi00MWIxLTk0N2UtN2ZlYTdkN2IzNDNlXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_UX1280.jpg", false, 2011, "Harry Potter and the Deathly Hallows: Part 2" },
                    { 60, 6.9m, new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Four kids travel through a wardrobe to the land of Narnia and learn of their destiny to free it...", "https://m.media-amazon.com/images/M/MV5BMTc0NTUwMTU5OV5BMl5BanBnXkFtZTcwNjAwNzQzMw@@._V1_UX1280.jpg", false, 2005, "The Chronicles of Narnia: The Lion, the Witch and the Wardrobe" },
                    { 61, 6.7m, new DateTime(2023, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Archaeologist Indiana Jones races against time to retrieve a legendary artifact that can change the course of history.", "https://m.media-amazon.com/images/M/MV5BYzhmODMzYzMtNTM5NS00NDQyLWEyMjYtYzBiYzExYjU0MTJlXkEyXkFqcGdeQXVyMTUzMTg2ODkz._V1_UX1280.jpg", false, 2023, "Indiana Jones and the Dial of Destiny" },
                    { 62, 6.6m, new DateTime(2021, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Based on Disneyland's theme park ride where a small riverboat takes a group of travelers through a jungle filled with dangerous animals and reptiles.", "https://m.media-amazon.com/images/M/MV5BNDE1MGRlNTQtZjc4ZC00MTI0LWEwY2MtODk1YTM2NmFmYTNmXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg", false, 2021, "Jungle Cruise" },
                    { 63, 6.1m, new DateTime(2022, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "A reclusive romance novelist on a book tour with her cover model gets swept up in a kidnapping attempt that lands them both in a cutthroat jungle adventure.", "https://m.media-amazon.com/images/M/MV5BMmIwYzFhODAtY2I1YS00ZDdmLTkyYWQtZjI5NDIwMDc2MjEyXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_UX1280.jpg", false, 2022, "The Lost City" },
                    { 64, 6.3m, new DateTime(2022, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Street-smart Nathan Drake is recruited by seasoned treasure hunter Victor Sullivan to recover a fortune amassed by Ferdinand Magellan.", "https://m.media-amazon.com/images/M/MV5BMWEwNjhkYzYtNjgzYy00YTY2LThjYWYtYzViMGJkZTI4Y2MyXkEyXkFqcGdeQXVyNTM0OTY1OQ@@._V1_UX1280.jpg", false, 2022, "Uncharted" },
                    { 65, 6.1m, new DateTime(2019, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Dora, a teenage explorer, leads her friends on an adventure to save her parents and solve the mystery behind a lost city of gold.", "https://m.media-amazon.com/images/M/MV5BOTVhMzYxNjgtYzYwOC00MGIwLWJmZGEtMjgwMzgxMWUwNmRhXkEyXkFqcGdeQXVyNjg2NjQwMDQ@._V1_UX1280.jpg", false, 2019, "Dora and the Lost City of Gold" },
                    { 66, 6.7m, new DateTime(2022, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "After accidentally crash-landing in 2022, time-traveling fighter pilot Adam Reed teams up with his 12-year-old self for a mission to save the future.", "https://m.media-amazon.com/images/M/MV5BOWM0YWMwMDQtMjE5NS00ZTIwLWE1NWEtODViMWZjMWI2OTU3XkEyXkFqcGdeQXVyMTEyMjM2NDc2._V1_UX1280.jpg", false, 2022, "The Adam Project" }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 9, 1 },
                    { 1, 2 },
                    { 5, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 5, 5 },
                    { 1, 6 },
                    { 5, 6 },
                    { 2, 7 },
                    { 2, 8 },
                    { 2, 9 },
                    { 8, 9 },
                    { 1, 10 },
                    { 2, 10 },
                    { 2, 11 },
                    { 3, 11 },
                    { 2, 12 },
                    { 3, 12 },
                    { 3, 13 },
                    { 3, 14 },
                    { 3, 15 },
                    { 9, 15 },
                    { 3, 16 },
                    { 8, 16 },
                    { 3, 17 },
                    { 3, 18 },
                    { 4, 19 },
                    { 9, 19 },
                    { 4, 20 },
                    { 4, 21 },
                    { 9, 21 },
                    { 4, 22 },
                    { 4, 23 },
                    { 4, 24 },
                    { 3, 25 },
                    { 5, 25 },
                    { 3, 26 },
                    { 5, 26 },
                    { 5, 27 },
                    { 5, 28 },
                    { 5, 29 },
                    { 9, 29 },
                    { 5, 30 },
                    { 1, 31 },
                    { 6, 31 },
                    { 6, 32 },
                    { 10, 32 },
                    { 6, 33 },
                    { 6, 34 },
                    { 10, 34 },
                    { 6, 35 },
                    { 6, 36 },
                    { 10, 36 },
                    { 7, 37 },
                    { 8, 37 },
                    { 7, 38 },
                    { 10, 38 },
                    { 3, 39 },
                    { 7, 39 },
                    { 7, 40 },
                    { 7, 41 },
                    { 10, 41 },
                    { 1, 42 },
                    { 7, 42 },
                    { 3, 43 },
                    { 8, 43 },
                    { 5, 44 },
                    { 8, 44 },
                    { 8, 45 },
                    { 2, 46 },
                    { 8, 46 },
                    { 3, 47 },
                    { 8, 47 },
                    { 2, 48 },
                    { 8, 48 },
                    { 3, 49 },
                    { 9, 49 },
                    { 9, 50 },
                    { 9, 51 },
                    { 9, 52 },
                    { 9, 53 },
                    { 9, 54 },
                    { 1, 55 },
                    { 10, 55 },
                    { 10, 56 },
                    { 8, 57 },
                    { 10, 57 },
                    { 8, 58 },
                    { 10, 58 },
                    { 10, 59 },
                    { 1, 60 },
                    { 10, 60 },
                    { 1, 61 },
                    { 11, 61 },
                    { 2, 62 },
                    { 11, 62 },
                    { 2, 63 },
                    { 11, 63 },
                    { 1, 64 },
                    { 11, 64 },
                    { 6, 65 },
                    { 11, 65 },
                    { 5, 66 },
                    { 11, 66 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_AverageRating",
                table: "Movies",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ReleaseYear",
                table: "Movies",
                column: "ReleaseYear");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
