using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Application.Commands;
using DfE.EmployerFavourites.Application.Queries;
using DfE.EmployerFavourites.Web.Application.Exceptions;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Mappers;
using DfE.EmployerFavourites.Web.Models;
using DfE.EmployerFavourites.Web.Validation;
using EmployerFavouritesWeb.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Types;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class ApprenticeshipsController : Controller
    {
        private readonly ExternalLinks _externalLinks;
        private readonly FatWebsite _fatConfig;
        private readonly IMediator _mediator;
        private readonly ApprenticeshipsParameterValidator _paramValidator;
        private readonly ILogger<ApprenticeshipsController> _logger;
  
        public ApprenticeshipsController(
            IOptions<ExternalLinks> externalLinks,
            IOptions<FatWebsite> fatConfig,
            IMediator mediator,
            ILogger<ApprenticeshipsController> logger) 
        {
            _externalLinks = externalLinks.Value;
            _fatConfig = fatConfig.Value;
            _mediator = mediator;
            _paramValidator = new ApprenticeshipsParameterValidator();
            _logger = logger;
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships")]
        public async Task<IActionResult> Index(string employerAccountId)
        {
            return RedirectPermanent(_externalLinks.AccountsHomePage);
        }

        [HttpGet("save-apprenticeship-favourites")]
        public async Task<IActionResult> SaveBasket(Guid basketId)
        {
            return RedirectPermanent(_externalLinks.AccountsHomePage);
        }

        [HttpPost("save-apprenticeship-favourites")]
        public async Task<IActionResult> ChooseAccount(string chosenHashedAccountId, Guid basketId)
        {
            if (chosenHashedAccountId is null)
            {
                ModelState.AddModelError("chosenHashedAccountId", "Please choose an account to continue");
                var accounts = await _mediator.Send(new GetUserAccountsQuery(User.GetUserId()));
                return View("ChooseAccount", accounts);
            }

            var userId = User.GetUserId();
            
            await _mediator.Send(new SaveApprenticeshipFavouriteBasketCommand { UserId = userId, BasketId = basketId, ChosenHashedAccountId = chosenHashedAccountId});

            var redirectUrl = string.Format(_externalLinks.AccountsDashboardPage, chosenHashedAccountId);

            return Redirect(redirectUrl);
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/providers")]
        public async Task<IActionResult> TrainingProvider(string employerAccountId, string apprenticeshipId)
        {
            return RedirectPermanent(_externalLinks.AccountsHomePage);
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/delete")]
        public async Task<IActionResult> Delete(string employerAccountId, string apprenticeshipId)
        {
            return RedirectPermanent(_externalLinks.AccountsHomePage);
        }

        [HttpPost("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string employerAccountId, string apprenticeshipId, bool confirmDelete)
        {
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId) ||
                !_paramValidator.IsValidApprenticeshipId(apprenticeshipId))
            {
                _logger.LogDebug($"Invalid parameters in the following: {nameof(employerAccountId)}({employerAccountId}), {nameof(apprenticeshipId)}({apprenticeshipId})");
                return BadRequest();
            }

            if (confirmDelete)
            {
                await _mediator.Send(new DeleteApprenticeshipFavouriteCommand()
                    { ApprenticeshipId = apprenticeshipId, EmployerAccountId = employerAccountId });
            }

            return RedirectToAction("Index", new { employerAccountId });
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/Provider/{ukprn}/delete")]
        public async Task<IActionResult> DeleteProvider(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            return RedirectPermanent(_externalLinks.AccountsHomePage);
        }

        [HttpPost("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/Provider/{ukprn}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProviderPost(string employerAccountId, string apprenticeshipId, int ukprn, bool confirmDelete)
        {
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId) ||
                !_paramValidator.IsValidApprenticeshipId(apprenticeshipId) ||
                !_paramValidator.IsValidProviderUkprn(ukprn))
            {
                _logger.LogDebug($"Invalid parameters in the following: {nameof(employerAccountId)}({employerAccountId}), {nameof(apprenticeshipId)}({apprenticeshipId}), {nameof(ukprn)}({ukprn})");
                return BadRequest();
            }

            if (confirmDelete)
            {

                await _mediator.Send(new DeleteApprenticeshipProviderFavouriteCommand()
                {
                    ApprenticeshipId = apprenticeshipId,
                    EmployerAccountId = employerAccountId,
                    Ukprn = ukprn
                });
            }

            return RedirectToAction("TrainingProvider", new {apprenticeshipId, employerAccountId});
        }
    }
}
