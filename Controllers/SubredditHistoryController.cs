using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using archiver_net.Models;
using System.Text.Json;

namespace archiver_net.Controllers;

public class SubredditHistoryController : Controller {

    private readonly ILogger<HomeController> _logger;

    public SubredditHistoryController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult GetHistory(string id)
    {   
        string infoJSON = TempData["fullInfo"] as string;
        var info = JsonSerializer.Deserialize<List<Listing>>(infoJSON);
        string fullString = "";
        
        foreach (Listing listing in info) {
            fullString += listing.toString();
        }
        
        return Content(fullString);
    }
}