namespace DevelopmentTool.Web.Controllers.Inputs;

/// <summary>
/// Ed25519签名
/// </summary>
public class Ed25519SignInput
{
    /// <summary>
    /// 待签名json字符串
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 签名私钥
    /// </summary>
    public string? PrivateKey { get; set; }
}