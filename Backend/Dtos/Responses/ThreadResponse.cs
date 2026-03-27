namespace RegisztracioTest.Dtos.Responses
{
    public class ThreadResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPinned { get; set; }
        public int ViewCount { get; set; }
        public int PostCount { get; set; }
        public UserResponse Author { get; set; } = new UserResponse();
        public CategoryResponse Category { get; set; } = new CategoryResponse();
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
        public PostResponse? LastPost { get; set; }
    }
}