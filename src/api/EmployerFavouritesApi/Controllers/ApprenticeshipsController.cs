using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Application.Commands;
using DfE.EmployerFavourites.Api.Application.Queries;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using DfE.EmployerFavourites.Api.Models;
using DfE.EmployerFavourites.Api.Models.Exceptions;
using DfE.EmployerFavourites.Api.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DfE.EmployerFavourites.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public partial class ApprenticeshipsController : ControllerBase
    {
        private const string ServerErrorMessage = "Internal Server Error";
        private readonly ILogger<ApprenticeshipsController> _logger;
        private readonly IMediator _mediator;
        private readonly ApprenticeshipsParameterValidator _paramValidator;
        
        
        public ApprenticeshipsController(ILogger<ApprenticeshipsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _paramValidator = new ApprenticeshipsParameterValidator();
        }

        // GET api/Apprenticeships/ABC123
        /// <summary>
        /// Gets all apprenticeship favourites for the Employer account Id provided
        /// </summary>
        /// <param name="employerAccountId"></param>
        /// <returns>List of Apprenticeship favourites </returns>
        /// <response code="204">The employer account has no apprenticeship favourites</response>
        /// <response code="400">The Employer account id provided is invalid</response>  
        /// <response code="401">The client is not authorized to access this endpoint</response>    
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [HttpGet]
        [Route("{employerAccountId}")]
        public async Task<ActionResult<Domain.ReadModel.ApprenticeshipFavourites>> Get(string employerAccountId)
        {
            try
            {
                var apprenticeships = await _mediator.Send(new GetApprenticeshipFavouritesRequest { EmployerAccountId = employerAccountId });

                if (apprenticeships.Count > 0)
                {
                    return Ok(apprenticeships);
                }

                return NoContent();
            }

            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid arguments were provided for get apprenticeship favourites");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in get apprenticeship favourites");
                return StatusCode(500, ServerErrorMessage);
            }
        }

        // PUT api/Apprenticeships/ABC123
        /// <summary>
        /// Save apprenticeship favourite to the Employer Account provided
        /// </summary>
        /// <param name="employerAccountId">Hashed Employer Account Id</param>
        /// <param name="favourites">Employer Favourites</param>
        /// <response code="201">Employer Favourites created for employer account</response>
        /// <response code="204">Employer Favourite added to existing</response>
        /// <response code="400">The Employer account id provided is invalid</response>  
        /// <response code="401">The client is not authorized to access this endpoint</response>    
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [HttpPut]
        [Route("{employerAccountId}")]
        public async Task<IActionResult> Put(string employerAccountId, [FromBody]List<Favourite> favourites)
        {
            try
            {
                favourites.ForEach(s => ValidateApprenticeship(s));

                var response = await _mediator.Send(new SaveApprenticeshipFavouriteCommand
                {
                    EmployerAccountId = employerAccountId,
                    Favourites = MapToWriteModel(favourites)
                });

                if (response.CommandResult == Domain.WriteModel.DomainUpdateStatus.Created)
                    return CreatedAtAction("Get", new { employerAccountId }, string.Empty);

                return NoContent();
            }
            catch (InvalidUkprnException ex)
            {
                return StatusCode(500, ex);
            }
            catch (InvalidApprenticeshipIdException ex)
            {
                return StatusCode(500, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in get apprenticeship favourites");
                return StatusCode(500, ServerErrorMessage);
            }

        }

        private void ValidateApprenticeship(Favourite apprenticeship)
        {
            if (!_paramValidator.IsValidApprenticeshipId(apprenticeship.ApprenticeshipId))
            {
                throw new InvalidApprenticeshipIdException("An apprenticeship id is invalid");
            }

            apprenticeship.Ukprns.ToList().ForEach(f => ValidateProviders(f));
        }

        private void ValidateProviders(int Ukprn)
        {
            if (!_paramValidator.IsValidProviderUkprn(Ukprn))
            {
                throw new InvalidUkprnException("A ukprn is invalid");
            }

            _logger.LogInformation($"Ukprn {Ukprn} is Valid.");
        }

        private ApprenticeshipFavourites MapToWriteModel(List<Favourite> favourites)
        {
            var writeModel = new ApprenticeshipFavourites();

            if (favourites == null)
            {
                return writeModel;
            }

            var items = favourites.Select(x => new ApprenticeshipFavourite { ApprenticeshipId = x.ApprenticeshipId, Ukprns = x.Ukprns });
            writeModel.AddRange(items);

            return writeModel;
        }
    }
}
