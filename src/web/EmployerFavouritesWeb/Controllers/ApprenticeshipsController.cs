using System;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            var model = new ApprenticeshipFavouritesViewModel
            {
                EmployerAccountId = employerAccountId,
                Items = response.EmployerFavourites.Select(mapper.Map).ToList()
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
            var accountId = await _mediator.Send(new SaveApprenticeshipFavouriteBasketCommand { UserId = userId, BasketId = basketId });

            var redirectUrl = string.Format(_externalLinks.AccountsDashboardPage, accountId);

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

            var response = await _mediator.Send(new ViewTrainingProviderForApprenticeshipFavouriteQuery
            {
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId
            });

            if (response.Favourite.Providers.Count == 0)
                throw new EntityNotFoundException($"No providers exist for the given apprenticeship: {apprenticeshipId}");

            var mapper = new ApprenticeshipFavouriteMapper(_fatConfig);

            var model = new TrainingProvidersViewModel
            {
                EmployerAccountId = employerAccountId,
                Items = response.Favourite.Providers.Select(mapper.Map).ToList()
            };

            return await Task.FromResult(View(model));
        }
    }
}
