namespace RegisztracioTest.Model
{
    public class ForumThread
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsLocked { get; set; } = false;
        public bool IsPinned { get; set; } = false;
        public int ViewCount { get; set; } = 0;

        public int CategoryId { get; set; }
        public int AuthorId { get; set; }

        public Category? Category { get; set; }
        public User? Author { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<ThreadTag> ThreadTags { get; set; } = new List<ThreadTag>();
    }
}