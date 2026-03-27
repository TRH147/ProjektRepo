namespace RegisztracioTest.Model
{
    public class ThreadTag
    {
        public int ThreadId { get; set; }
        public int TagId { get; set; }

        public ForumThread? Thread { get; set; }
        public Tag? Tag { get; set; }
    }
}