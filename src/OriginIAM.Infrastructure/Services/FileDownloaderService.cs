using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using Microsoft.Extensions.Logging;
using OriginIAM.Application.Interfaces;

public sealed class FileDownloaderService : IFileDownloader
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FileDownloaderService> _logger;

    public FileDownloaderService(ILogger<FileDownloaderService> logger, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DownloadFileAsync(string blobUrl, Func<Stream, Task> processStream, string fileContentType)
    {
        try
        {   
            var _httpClient = _httpClientFactory.CreateClient();

            _logger.LogInformation($"Starting file download: {blobUrl}");

            using (var response = await _httpClient.GetAsync(blobUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType?.MediaType;

                if (!contentType.Equals(fileContentType, StringComparison.OrdinalIgnoreCase))
                {
                    var errorMessage = $"File {blobUrl} is not a valid csv.";

                    _logger.LogError(errorMessage);

                    throw new InvalidOperationException(errorMessage);
                }

                await using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    _logger.LogInformation("Download complete. Starting file processing.");
                    await processStream(stream);
                }
            }

            _logger.LogInformation("File processing completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing file: {blobUrl}");
            throw;
        }
    }
}
