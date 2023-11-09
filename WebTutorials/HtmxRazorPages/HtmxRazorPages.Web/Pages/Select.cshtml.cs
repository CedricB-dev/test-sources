using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HtmxRazorPages.Web.Pages;

public class Select : PageModel
{
    private readonly Dictionary<string, List<string>> _cuisines = new()
    {
        { "Italian", new() { "Pizza", "Lasagna" } },
        { "Mexican", new() { "Tacos", "Churros" } },
        { "American", new() { "Burger", "Hot dogs" } }
    };

    public List<SelectListItem> CuisineItems
    {
        get
        {
            var selectListItems = _cuisines.Keys
                .Select(x => new SelectListItem(x, x))
                .ToList();
            
            selectListItems.Insert(0, new SelectListItem("Choose an option", "")
            {
                Disabled = true,
                Selected = true
            });

            return selectListItems;
        }
    }

    [BindProperty(SupportsGet = true)] public string? Cuisine { get; set; }
    
    [BindProperty(SupportsGet = true)] public string? Food { get; set; }
    
    public void OnGet()
    {
    }

    public IActionResult OnGetFoods()
    {
        return Content("", "text/html");
    }
}