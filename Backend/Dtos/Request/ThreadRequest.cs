namespace RegisztracioTest.Dtos.Requests
{
    public class ThreadRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
    }
}