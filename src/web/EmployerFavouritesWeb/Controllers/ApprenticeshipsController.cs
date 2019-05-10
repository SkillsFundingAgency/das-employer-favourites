using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Commands;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Mappers;
using DfE.EmployerFavourites.Web.Models;
using DfE.EmployerFavourites.Web.Queries;
using DfE.EmployerFavourites.Web.Validation;
using EmployerFavouritesWeb.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ApprenticeshipsController : Controller
    {
        private readonly ExternalLinks _externalLinks;
        private readonly IMediator _mediator;

        public ApprenticeshipsController(IOptions<ExternalLinks> externalLinks, IMediator mediator)
        {
            _externalLinks = externalLinks.Value;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("accounts/{employerAccountId:minlength(6)}")]
        public async Task<IActionResult> Index([RegularExpression(@"^.{6,}$")]string employerAccountId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(new ViewEmployerFavouritesQuery
            {
                EmployerAccountId = employerAccountId
            });

            var mapper = new ApprenticeshipFavouriteMapper();

            var model = new ApprenticeshipFavouritesViewModel
            {
                EmployerName = response.EmployerAccount.Name,
                Items = response.EmployerFavourites.Select(mapper.Map).ToList()
            };

            return View(model);
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

            var userId = User.GetUserId();
            await _mediator.Send(new SaveApprenticeshipFavouriteCommand { UserId = userId, ApprenticeshipId = apprenticeshipId, Ukprn = ukprn });

            return Redirect(_externalLinks.AccountsHomePage.AbsoluteUri);
        }
    }
}
