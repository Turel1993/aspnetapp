using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspnetapp.Models;

namespace aspnetapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Update(IFormFile userfile)
    {
        try
        {
            IndexModel.filename = userfile.FileName;
            IndexModel.filename = Path.GetFileName(IndexModel.filename);
            string uploadfilepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot//image",IndexModel.filename);
            var stream = new FileStream(uploadfilepath,FileMode.Create);
            userfile.CopyToAsync(stream);
            ViewBag.message = "Foto Subida";
            ViewBag.picture = IndexModel.filename;
        }
        catch(Exception ex)
        {
            ViewBag.message = ex.ToString();
        }
        return View("Index");
    }

    public IActionResult Guardar()
    {
        try
        {
            System.IO.File.Delete("..//aspnetapp//wwwroot//image//"+IndexModel.filename);
        }
        catch
        {

        }

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
