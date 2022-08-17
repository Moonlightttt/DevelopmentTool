using Microsoft.AspNetCore.Mvc;

namespace DevelopmentTool.Web.Controllers;

/// <summary>
/// 首页
/// </summary>
public class HomeController : Controller
{
    // GET
    /// <summary>
    /// 首页跳转
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return Redirect("swagger");
    }
}