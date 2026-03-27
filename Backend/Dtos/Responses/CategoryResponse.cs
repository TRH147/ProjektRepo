namespace RegisztracioTest.Dtos.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int Order { get; set; } = 0;
        public int ThreadCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}