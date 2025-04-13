namespace MoviesHub.Services.MoviesAPI.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
