namespace OriginIAM.Application.Interfaces
{
    public interface IFileDownloader
    {
        Task DownloadFileAsync(string url, Func<Stream, Task> processStream, string fileContentType);
    }
}