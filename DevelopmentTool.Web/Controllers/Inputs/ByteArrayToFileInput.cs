namespace DevelopmentTool.Web.Controllers.Inputs;

public class ByteArrayToFileInput
{
    public FileType Type { get; set; }

    public string Content { get; set; }
}

public enum FileType
{
    xlsx=1,
}