using Microsoft.AspNetCore.Mvc;

namespace DevelopmentTool.Controllers;

/// <summary>
/// 首页
/// </summary>
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return Redirect("swagger");
    }
}