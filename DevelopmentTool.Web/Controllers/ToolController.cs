using System.Text;
using System.Text.RegularExpressions;
using DevelopmentTool.Extensions;
using DevelopmentTool.Web.Controllers.Inputs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace DevelopmentTool.Web.Controllers;

/// <summary>
/// 工具集
/// </summary>
[ApiController]
[Route("[controller]/[Action]")]
public class ToolController : ControllerBase
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<ToolController> _logger;

    /// <summary>
    /// 缓存
    /// </summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="distributedCache"></param>
    public ToolController(ILogger<ToolController> logger,
        IDistributedCache distributedCache)
    {
        _logger = logger;
        _distributedCache = distributedCache;
    }

    /// <summary>
    /// json属性名转换Camel格式
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost(Name = "ConvertToCamel")]
    public string ConvertToCamel(ConvertToCamelInput input)
    {
        _logger.LogInformation($"Json转换请求：{input.JsonStr}");

        if (string.IsNullOrWhiteSpace(input.JsonStr))
        {
            _logger.LogInformation($"Json转换请求：转换json为空");
            return string.Empty;
        }
        
        JTokenWriter writer = new JTokenWriter();
        var jToken = JToken.Parse(input.JsonStr!);

        ProcessJson(jToken, writer);

        var result = writer.Token!.ToString();
        _logger.LogInformation($"Json转换响应：{input.JsonStr}");
        return result;
    }

    /// <summary>
    /// 查询Token
    /// </summary>
    /// <returns></returns>
    [HttpPost(Name = "QueryRedisValue")]
    public async Task<IActionResult> QueryRedisValueAsync()
    {
        var result = await _distributedCache.GetValueAsync<dynamic>(
            "c:EcommerceCloud.ApiClient.Filters.ParkApiActionFilter+ParkTokenInfo,k:ParkTokenCacheKay");

        _logger.LogInformation($"获取Redis值：{result}");

        var data = result as JObject;

        return Ok(new { token = data?["token"]!.Value<string>() });
    }

    /// <summary>
    /// ED25519私钥签名
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet(Name = "Ed25519Sign")]
    public string Ed25519Sign([FromQuery] Ed25519SignInput input)
    {
        _logger.LogInformation($"ED25519私钥签名：{input.Message}");

        if (string.IsNullOrWhiteSpace(input.Message))
        {
            _logger.LogInformation($"ED25519私钥签名：Message为空");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(input.PrivateKey))
        {
            _logger.LogInformation($"ED25519私钥签名：PrivateKey为空");
            return string.Empty;
        }

        var result = CreateSignature(input.Message,input.PrivateKey);

        _logger.LogInformation($"ED25519私钥签名结果：{input.Message}");

        return result;
    }

    private void ProcessJson(JToken jToken, JTokenWriter writer)
    {
        switch (jToken)
        {
            case JArray a:
                writer.WriteStartArray();
                foreach (var item in a)
                {
                    ProcessJson(item, writer);
                }

                writer.WriteEndArray();
                break;
            case JObject o:
                writer.WriteStartObject();
                foreach (var item in o.Children())
                {
                    ProcessJson(item, writer);
                }

                writer.WriteEndObject();
                break;
            case JProperty p:
                var firstChar = p.Name[0];
                var pName = Regex.Replace(p.Name, "^[A-Za-z]", firstChar.ToString().ToLower());
                writer.WritePropertyName(pName);
                ProcessJson(p.Value, writer);
                break;
            case JValue p:
                writer.WriteValue(p.Value);
                break;
            default:
                throw new Exception("未处理异常");
        }
    }

    private string CreateSignature(string auditLogJson, string privateKey)
    {
        var ed25519pkcs8Parameters = new Ed25519PrivateKeyParameters(Convert.FromBase64String(privateKey));

        // Sign
        var dataToSign = Encoding.UTF8.GetBytes(auditLogJson);
        ISigner signer = new Ed25519Signer();
        signer.Init(true, ed25519pkcs8Parameters);
        signer.BlockUpdate(dataToSign, 0, dataToSign.Length);

        return Convert.ToBase64String(signer.GenerateSignature());
    }

}