using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using DfE.EmployerFavourites.Web.Application.Exceptions;
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

            LogException();

            return View(GetViewNameForStatus(Response.StatusCode), new ErrorViewModel { StatusCode = id, RequestId = HttpContext.TraceIdentifier });
        }

        private void LogException()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                string routeWhereExceptionOccurred = exceptionFeature.Path;
                var exception = exceptionFeature.Error;

                switch(exception)
                {
                    case AggregateException ex:
                        var flattenedExceptions = ex.Flatten();
                        _logger.LogError(flattenedExceptions, "Aggregate exception on path: {route}", routeWhereExceptionOccurred);

                        exception = flattenedExceptions.InnerExceptions.FirstOrDefault();
                        break;
                    case EntityNotFoundException ex:
                        Response.StatusCode = (int)HttpStatusCode.NotFound;
                        _logger.LogDebug(ex, "Entity not found");
                        return;
                    default:
                        break;
                }

                _logger.LogError(exception, "Unhandled exception on path: {route}", routeWhereExceptionOccurred);
            }
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
                case (int)HttpStatusCode.BadRequest:
                    return "BadRequest";
                default:
                    return "Error";
            }
        }
    }
}