using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Security;
using DfE.EmployerFavourites.Web.Commands;
using DfE.EmployerFavourites.Web.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using DfE.EmployerFavourites.Web.Validation;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ApprenticeshipsController : Controller
    {
        private readonly ILogger<ApprenticeshipsController> _logger;
        private readonly ExternalLinks _externalLinks;
        private readonly IMediator _mediator;

        public ApprenticeshipsController(ILogger<ApprenticeshipsController> logger, IOptions<ExternalLinks> externalLinks, IMediator mediator)
        {
            _logger = logger;
            _externalLinks = externalLinks.Value;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("accounts/{employerAccountId:minlength(6)}")]
        public IActionResult Index(string employerAccountId)
        {
            return View();
        }

        [Authorize]
        [HttpGet("save-apprenticeship-favourites")]
        public async Task<IActionResult> Add(string apprenticeshipId, int? ukprn = null)
        {
            var validator = new ApprenticeshipsParameterValidator();
            
            if (!validator.IsValidApprenticeshipId(apprenticeshipId))
                return BadRequest();
            
            if (!validator.IsValidProviderUkprn(ukprn))
                return BadRequest();

            var userId = User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier);
            await _mediator.Send(new SaveApprenticeshipFavouriteCommand { UserId = userId, ApprenticeshipId = apprenticeshipId, Ukprn = ukprn });

            return Redirect(_externalLinks.AccountsHomePage.AbsoluteUri);
        }
    }
}
