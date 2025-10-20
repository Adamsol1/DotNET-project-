
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Viewmodels;

namespace DOTNET_PROJECT.Controllers
{
    /// <summary>
    /// Controller responsible for loading the homepage. 
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Returns the home view. 
        /// </summary>
        
        [HttpGet]
        public IActionResult Index() => View();
    }
}
