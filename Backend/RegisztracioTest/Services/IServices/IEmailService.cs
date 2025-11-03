namespace IsmetlesWebAPI.Services.IServices;

public interface IEmailService
{
    public Task<bool> SendLoginCodeAsync(string email, string code);
}