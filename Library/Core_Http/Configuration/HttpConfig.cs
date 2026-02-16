namespace Core_Http.Configuration;

public class HttpConfig
{
    public const string SectionName = "Core_Http_Settings";

    public int RetryCount { get; set; } = 3;
    public int BaseDelaySeconds { get; set; } = 2;
    public int FailureThreshold { get; set; } = 5;
    public int DurationOfBreakSeconds { get; set; } = 30;
    public bool DesativarSegurancaSsl { get; set; } = false;
}
