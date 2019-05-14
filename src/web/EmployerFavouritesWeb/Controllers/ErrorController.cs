using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using DfE.EmployerFavourites.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        
        [Route("error/{id?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int id)
        {
            Response.StatusCode = id;

            return View(GetViewNameForStatus(id), new ErrorViewModel { StatusCode = id, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetViewNameForStatus(int statusCode)
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                string routeWhereExceptionOccurred = exceptionFeature.Path;
                var exception = exceptionFeature.Error;

                if (exception is AggregateException aggregateException)
                {
                    var flattenedExceptions = aggregateException.Flatten();
                    _logger.LogError(flattenedExceptions, "Aggregate exception on path: {route}", routeWhereExceptionOccurred);

                    exception = flattenedExceptions.InnerExceptions.FirstOrDefault();
                }

                _logger.LogError(exception, "Unhandled exception on path: {route}", routeWhereExceptionOccurred);
            }

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