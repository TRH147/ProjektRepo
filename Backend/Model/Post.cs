namespace RegisztracioTest.Model
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int VoteScore { get; set; }
        public int? ParentPostId { get; set; }
        public int ThreadId { get; set; }
        public int AuthorId { get; set; }

        public bool IsPinned { get; set; } = false;

        public virtual ForumThread Thread { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
        public virtual Post? ParentPost { get; set; }
        public virtual ICollection<Post> Replies { get; set; } = new List<Post>();
    }
}