namespace Funda.Shared.Configurations
{
    public sealed class FundaConfig
    {
        public const string SectionName = $"{nameof(FundaConfig)}";

        public string ApiKey { get; init; } = string.Empty;
        public string BaseUrl { get; init; } = string.Empty;
        public ObjectsLookupApiConfig ObjectsLookupApi { get; init; } = new();
    }

    public sealed class ObjectsLookupApiConfig
    {
        public int MaxNumberOfRequestsPerMinute { get; init; }
        public int MaxRetryAttempts { get; init; }
        public int PageSize { get; init; }
    }
}
