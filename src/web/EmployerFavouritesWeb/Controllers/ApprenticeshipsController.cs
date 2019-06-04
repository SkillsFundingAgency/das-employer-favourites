using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Application.Commands;
using DfE.EmployerFavourites.Application.Queries;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Mappers;
using DfE.EmployerFavourites.Web.Models;
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
        private readonly ApprenticeshipsParameterValidator _paramValidator;

        public ApprenticeshipsController(IOptions<ExternalLinks> externalLinks, IMediator mediator)
        {
            _externalLinks = externalLinks.Value;
            _mediator = mediator;
            _paramValidator = new ApprenticeshipsParameterValidator();
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships")]
        public async Task<IActionResult> Index([RegularExpression(@"^.{6,}$")]string employerAccountId)
        {
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId))
                return BadRequest();

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

        [HttpGet("save-apprenticeship-favourites")]
        public async Task<IActionResult> Add(string apprenticeshipId, int? ukprn = null)
        {
            if (!_paramValidator.IsValidApprenticeshipId(apprenticeshipId))
                return BadRequest();
            
            if (!_paramValidator.IsValidProviderUkprn(ukprn))
                return BadRequest();

            var userId = User.GetUserId();
            var accountId = await _mediator.Send(new SaveApprenticeshipFavouriteCommand { UserId = userId, ApprenticeshipId = apprenticeshipId, Ukprn = ukprn });

            var redirectUrl = string.Format(_externalLinks.AccountsDashboardPage, accountId);

            return Redirect(redirectUrl);
        }

        [AllowAnonymous]
        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/providers/{ukprn}")]
        public async Task<IActionResult> TrainingProvider(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            if (!_paramValidator.IsValidApprenticeshipId(apprenticeshipId))
                return BadRequest();

            if (!_paramValidator.IsValidProviderUkprn(ukprn))
                return BadRequest();

            var response = await _mediator.Send(new ViewTrainingProviderForApprenticeshipFavouriteQuery
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Ukprn = ukprn
            });

            var model = new TrainingProviderViewModel
            {
                ProviderName = response.Provider.Name
            };

            return await Task.FromResult(View(model));
        }
    }
}
