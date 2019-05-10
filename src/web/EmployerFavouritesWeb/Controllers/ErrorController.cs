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

            return View(GetViewNameForStatus(id), new ErrorViewModel { StatusCode = id, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetViewNameForStatus(int statusCode)
        {
            switch(statusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    return "PageNotFound";
                case (int)HttpStatusCode.Forbidden:
                case (int)HttpStatusCode.Unauthorized:
                    return "AccessDenied";
                default:
                    return "Error";
            }
        }
    }
}