namespace RegisztracioTest.Dtos.Requests
{
    public class PostRequest
    {
        public string Content { get; set; } = string.Empty;
        public int? ParentPostId { get; set; }
    }
}