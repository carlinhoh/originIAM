namespace OriginIAM.Application.Interfaces
{
    public interface ICsvParser<T>
    {
        IAsyncEnumerable<T> ParseCsvAsync(Stream csvStream);
    }
}