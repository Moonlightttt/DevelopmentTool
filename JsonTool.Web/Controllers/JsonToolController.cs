using System.Text.RegularExpressions;
using JsonTool.Controllers.Inputs;
using JsonTool.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace JsonTool.Controllers;

[ApiController]
[Route("[controller]/[Action]")]
public class JsonToolController : ControllerBase
{
    private readonly ILogger<JsonToolController> _logger;

    public JsonToolController(ILogger<JsonToolController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "ConvertToCamel")]
    public string ConvertToCamel(ConvertToCamelInputs inputs)
    {
        JTokenWriter writer = new JTokenWriter();
        var jToken= JToken.Parse(inputs.JsonStr);

        ProcessJson(jToken,writer);

        var result = writer.Token!.ToString();

        return result;
    }
    
    [HttpPost(Name = "QueryRedisValue")]
    public IActionResult QueryRedisValue()
    {
        var value = RedisManager.Instance.HashGet(
            "c:EcommerceCloud.ApiClient.Filters.ParkApiActionFilter+ParkTokenInfo,k:ParkTokenCacheKay", "data");

        var jObject = JObject.Parse(value!);

        return Ok(new { token = jObject["token"]!.Value<string>()});
    }

    private void ProcessJson(JToken jToken, JTokenWriter writer){
        switch (jToken)
        {
            case JArray a:
                writer.WriteStartArray();
                foreach (var item in a)
                {
                    ProcessJson(item,writer);
                }
                writer.WriteEndArray();
                break;
            case JObject o:
                writer.WriteStartObject();
                foreach (var item in o.Children())
                {
                    ProcessJson(item,writer);
                }
                writer.WriteEndObject();
                break;
            case JProperty p:
                var firstChar= p.Name[0];
                var pName =Regex.Replace(p.Name,"^[A-Za-z]",firstChar.ToString().ToLower());
                writer.WritePropertyName(pName);
                ProcessJson(p.Value,writer);
                break;
            case JValue p:
                writer.WriteValue(p.Value);
                break;             
            default:
                throw new Exception("未处理异常");
        }
    }
}
