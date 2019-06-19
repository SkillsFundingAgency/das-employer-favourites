using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet, Route("logout", Name = RouteNames.Logout_Get)]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            await HttpContext.SignOutAsync("oidc");
        }
    }
}
