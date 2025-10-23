namespace NetBuild.App.Core.Services.Interface
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(string to, string subject, string body, bool htmlContent = false);
        Task<bool> SendMailAsync(string to, string subject, object[] subjectArgs, string body, object[] bodyArgs, bool isHtmlContent = false);
    }
}
