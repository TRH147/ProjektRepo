namespace RegisztracioTest.Dtos.CategoryDto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public int ThreadCount { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
