using System.Diagnostics;
using DfE.EmployerFavourites.Web.Models;
using EmployerFavouritesWeb.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ApprenticeshipsController : Controller
    {
        private readonly ILogger<ApprenticeshipsController> _logger;
        private readonly ExternalLinks _externalLinks;

        public ApprenticeshipsController(ILogger<ApprenticeshipsController> logger, IOptions<ExternalLinks> externalLinks)
        {
            _logger = logger;
            _externalLinks = externalLinks.Value;
        }

        [HttpGet]
        public IActionResult Add(string apprenticeshipId, int? ukprn = null)
        {
            var validator = new ApprenticeshipsParameterValidator();
            
            if (!validator.IsValidApprenticeshipId(apprenticeshipId))
                return BadRequest();
            
            if (!validator.IsValidProviderUkprn(ukprn))
                return BadRequest();

            return Redirect(_externalLinks.AccountsHomePage.AbsoluteUri);
        }
    }
}
