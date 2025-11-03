namespace RegisztracioTest.Services.IServices
{
    public interface ICodeService
    {
        public string GenerateCode();
        public Task<bool> StoreCodeAsync(string email, string code);
        public Task<bool> ValidateCodeAsync(string email, string code);
    }
}
