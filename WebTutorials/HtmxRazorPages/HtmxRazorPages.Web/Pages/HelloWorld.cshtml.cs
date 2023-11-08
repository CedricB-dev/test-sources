using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxRazorPages.Web.Pages;

public class HelloWorld : PageModel
{
    private readonly ILogger<HelloWorld> _logger;

    public HelloWorld(ILogger<HelloWorld> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Request.IsHtmx()
            ? Content("<span>Hello World !</span>", "text/html")
            : Page();
    }
}