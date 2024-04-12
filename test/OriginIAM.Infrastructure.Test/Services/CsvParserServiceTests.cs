using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OriginIAM.Application.Models;
using OriginIAM.Infrastructure.Services;
using Xunit;

namespace OriginIAM.Infrastructure.Tests.Services
{
    public class CsvParserServiceTests
    {
        private readonly CsvParserService<EligibilityRecord> parserService = new CsvParserService<EligibilityRecord>();

        [Fact]
        public async Task ParseCsvAsync_ValidCsvStream_ReturnsExpectedRecords()
        {
            // Arrange
            var csvContent = @"email,full name,country,birth_date,salary
daniel93@dijkstra.com,Jessica Drake,NR,2006-01-13,76921
jeanette46@example.net,Ann Wilson,BW,1961-03-16,104120";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var records = new List<EligibilityRecord>();
            await foreach (var record in parserService.ParseCsvAsync(stream))
            {
                records.Add(record);
            }

            // Assert
            Assert.Equal(2, records.Count);
            Assert.Equal("daniel93@dijkstra.com", records[0].Email);
            Assert.Equal("Jessica Drake", records[0].FullName);
            Assert.Equal("NR", records[0].Country);
            Assert.Equal(new DateTime(2006, 01, 13), records[0].BirthDate);
            Assert.Equal(76921M, records[0].Salary);
        }

        [Fact]
        public async Task ParseCsvAsync_EmptyStream_ReturnsNoRecords()
        {
            // Arrange
            var csvContent = string.Empty;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var records = new List<EligibilityRecord>();
            await foreach (var record in parserService.ParseCsvAsync(stream))
            {
                records.Add(record);
            }

            // Assert
            Assert.Empty(records);
        }
    }
}
