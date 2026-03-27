namespace RegisztracioTest.Model
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#6c757d";

        public ICollection<ThreadTag> ThreadTags { get; set; } = new List<ThreadTag>();
    }
}