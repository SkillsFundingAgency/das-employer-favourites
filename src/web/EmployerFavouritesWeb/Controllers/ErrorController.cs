using System.Diagnostics;
using System.Net;
using DfE.EmployerFavourites.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ErrorController : Controller
    {        
        [Route("error/{id?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id)
        {
            Response.StatusCode = id;

            return View(new ErrorViewModel { StatusCode = id, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}