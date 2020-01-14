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
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId))
            {
                _logger.LogDebug($"Invalid parameter: {nameof(employerAccountId)}({employerAccountId})");

                return BadRequest();
            }

            var response = await _mediator.Send(new ViewEmployerFavouritesQuery
            {
                EmployerAccountId = employerAccountId
            });

            var mapper = new ApprenticeshipFavouriteMapper(_fatConfig);

            var items = response.EmployerFavourites.Select(mapper.Map).ToList();

            items.ForEach(apprenticeship =>
            {
                apprenticeship.CreateVacancyUrl = $"{string.Format(_externalLinks.CreateVacancyUrl, employerAccountId)}?ProgrammeId={ apprenticeship.Id }";
            });

            var model = new ApprenticeshipFavouritesViewModel
            {
                EmployerAccountId = employerAccountId,
                Items = items,
                HasLegalEntity = response.EmployerAccount.HasLegalEntities
            };
            
            return View(model);
        }

        [HttpGet("save-apprenticeship-favourites")]
        public async Task<IActionResult> SaveBasket(Guid basketId)
        {
            if (!_paramValidator.IsValidBasketId(basketId))
            {
                _logger.LogDebug($"Invalid BasketId: {basketId}");

                return BadRequest();
            }

            var userId = User.GetUserId();
            
            var accounts = await _mediator.Send(new GetUserAccountsQuery(userId));
          
            if (accounts.Count > 1)
            {
                return View("ChooseAccount", accounts);
            }
            
            var accountId = await _mediator.Send(new SaveApprenticeshipFavouriteBasketCommand { UserId = userId, BasketId = basketId });
            
            var redirectUrl = string.Format(_externalLinks.AccountsDashboardPage, accountId);
            
            return Redirect(redirectUrl);
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
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId) ||
                !_paramValidator.IsValidApprenticeshipId(apprenticeshipId))
            {
                _logger.LogDebug($"Invalid parameters in the following: {nameof(employerAccountId)}({employerAccountId}), {nameof(apprenticeshipId)}({apprenticeshipId})");
                return BadRequest();
            }

            var response = await _mediator.Send(new ViewApprenticeshipFavouriteQuery
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId
            });

            if (response.Favourite.Providers.Count == 0)
            {
                _logger.LogWarning(($"No providers exist for the given apprenticeship: {apprenticeshipId}, redirecting to apprenticeship favourites"));

                return RedirectToAction("Index", new{employerAccountId});
            }
                

            var mapper = new ApprenticeshipFavouriteMapper(_fatConfig);

            var items = response.Favourite.Providers.Select(mapper.Map).ToList();

            items.ForEach(apprenticeshipProvider => {
                apprenticeshipProvider.CreateVacancyUrl = $"{string.Format(_externalLinks.CreateVacancyUrl, employerAccountId)}?Ukprn={ apprenticeshipProvider.Ukprn }&ProgrammeId={ apprenticeshipId }";
            });
            
            var model = new TrainingProvidersViewModel
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Items = items,
                HasLegalEntity =  response.HasLegalEntities
            };

            return await Task.FromResult(View(model));
        }

        [HttpGet("accounts/{employerAccountId:minlength(6)}/apprenticeships/{apprenticeshipId}/delete")]
        public async Task<IActionResult> Delete(string employerAccountId, string apprenticeshipId)
        {
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId) ||
                !_paramValidator.IsValidApprenticeshipId(apprenticeshipId))
            {
                _logger.LogDebug($"Invalid parameters in the following: {nameof(employerAccountId)}({employerAccountId}), {nameof(apprenticeshipId)}({apprenticeshipId})");
                return BadRequest();
            }

            var response = await _mediator.Send(new ViewApprenticeshipFavouriteQuery
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId
            });


            var mapper = new ApprenticeshipFavouriteMapper(_fatConfig);

            var model = new TrainingProvidersViewModel
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Apprenticeship = mapper.Map(response.Favourite),
                Items = response.Favourite.Providers.Select(mapper.Map).ToList()
            };

            return View("Delete", model);
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
            if (!_paramValidator.IsValidEmployerAccountId(employerAccountId) ||
                !_paramValidator.IsValidApprenticeshipId(apprenticeshipId) || !_paramValidator.IsValidProviderUkprn(ukprn))
            {
                _logger.LogDebug($"Invalid parameters in the following: {nameof(employerAccountId)}({employerAccountId}), {nameof(apprenticeshipId)}({apprenticeshipId}), {nameof(ukprn)}({ukprn})");
                return BadRequest();
            }

            var response = await _mediator.Send(new ViewTrainingProviderForApprenticeshipFavouriteQuery
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Ukprn = ukprn
                
            });

            if (response.Provider == null)
                throw new EntityNotFoundException($"Provider exist for the given apprenticeship: {apprenticeshipId} and ukprn: {ukprn}");

            var mapper = new ApprenticeshipFavouriteMapper(_fatConfig);

            var model = new DeleteTrainingProviderViewModel
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Provider = mapper.Map(response.Provider),
                Ukprn = ukprn
            };


            return View("DeleteProvider", model);
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
