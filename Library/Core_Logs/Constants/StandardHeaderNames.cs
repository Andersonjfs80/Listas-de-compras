namespace Core_Logs.Constants;

public static class StandardHeaderNames
{
    public const string Token = "TOKEN"; 
    public const string AppSigla = "SIGLA-APLICACAO";
    public const string SessionId = "SESSAO-ID";
    public const string MessageId = "MESSAGE-ID";

    public static readonly string[] MandatoryHeaders = new[]
    {
        Token,
        AppSigla,
        SessionId,
        MessageId
    };
}
