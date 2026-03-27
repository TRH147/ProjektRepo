namespace RegisztracioTest.Dtos.Responses
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int VoteScore { get; set; }
        public int? ParentPostId { get; set; }

        public bool IsPinned { get; set; }

        public UserResponse Author { get; set; } = new UserResponse();
        public List<PostResponse> Replies { get; set; } = new List<PostResponse>();
    }
}