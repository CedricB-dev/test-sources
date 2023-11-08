using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxRazorPages.Web.Pages;

public class Counter : PageModel
{
    private static int count;
    
    public void OnGet()
    {
        count = 0;
    }

    public IActionResult OnPost()
    {
        return Content($"<span>{count++}</span>", "text/html");
    }
}