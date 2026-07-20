namespace MarketPulse.Api.Models.Imports;

public class ImportResult
{
    public int Imported { get; init; }

    public int Skipped { get; init; }

    public int Failed { get; init; }

    public long DurationMs { get; init; }

    public IReadOnlyList<ImportError> Errors { get; init; }
        = Array.Empty<ImportError>();
}