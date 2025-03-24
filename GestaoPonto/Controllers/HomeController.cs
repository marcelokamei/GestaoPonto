using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestaoPonto.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestaoPonto.Controllers;

[Authorize]
public class HomeController : Controller
{
    
    public IActionResult Index()
    {
        ViewData["UserName"] = User.Identity.Name;
        return View();
    }

}
