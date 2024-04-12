using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OriginIAM.Application.Interfaces;
using Xunit;

namespace OriginIAM.Application.Tests.Services
{
    public class FileDownloaderServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly HttpClient _mockHttpClient;
        private readonly FileDownloaderService _fileDownloaderService;
        private readonly Mock<ILogger<FileDownloaderService>> _mockLogger;

        public FileDownloaderServiceTests()
        {
            _mockMessageHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _mockLogger = new Mock<ILogger<FileDownloaderService>>();
            _fileDownloaderService = new FileDownloaderService(_mockHttpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task DownloadFileAsync_ValidUrl_CallsProcessStream()
        {
            // Arrange
            var fakeUrl = "http://localhost/fakeBlobUrl";
            var responseStream = new MemoryStream();
            var writer = new StreamWriter(responseStream);
            writer.Write("fake response");
            writer.Flush();
            responseStream.Position = 0;
            var responseContent = new StreamContent(responseStream);
            responseContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");

            _mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString() == fakeUrl),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = responseContent
                });

            bool isStreamProcessed = false;

            async Task ProcessStream(Stream stream)
            {
                isStreamProcessed = true;
                Assert.NotNull(stream);
            }

            // Act
            await _fileDownloaderService.DownloadFileAsync(fakeUrl, ProcessStream, "text/csv");

            // Assert
            Assert.True(isStreamProcessed);
        }
    }
}
