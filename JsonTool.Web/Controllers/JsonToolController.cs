using System.Text.RegularExpressions;
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

    [HttpPost(Name = "ConvetToCamel")]
    public string ConvetToCamel(string json)
    {
        var result= string.Empty;

        JTokenWriter writer = new JTokenWriter();
        var jToken= JToken.Parse(json);

        ProcessJson(jToken,writer);

        result=writer.Token!.ToString();

        return result;
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
