namespace Core_Logs.Constants;

public static class StandardHeaderNames
{    
    public const string AppSigla = "SIGLA-APLICACAO";
    public const string SessionId = "SESSAO-ID";
    public const string MessageId = "MESSAGE-ID";
    public const string MessageIdModulo = "MESSAGE-ID-MODULO";
    public const string HardwareId = "HARDWARE-ID";
    public const string Token = "Authorization";

    public static readonly string[] MandatoryHeaders = new[]
    {
        AppSigla,
        SessionId,
        MessageId,
        MessageIdModulo,
        HardwareId        
    };
}
