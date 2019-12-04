using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Application.Commands;
using DfE.EmployerFavourites.Api.Application.Queries;
using DfE.EmployerFavourites.Api.Domain.WriteModel;
using DfE.EmployerFavourites.Api.Models;
using DfE.EmployerFavourites.Api.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WriteModel = DfE.EmployerFavourites.Api.Domain.WriteModel;
using System.Text.RegularExpressions;
using Provider = DfE.EmployerFavourites.Api.Models.Provider;

namespace DfE.EmployerFavourites.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApprenticeshipsController : ControllerBase
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
                var apprenticeships = await _mediator.Send(new GetApprenticeshipFavouritesRequest
                { EmployerAccountId = employerAccountId });

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
        public async Task<IActionResult> Put(string employerAccountId, [FromBody] List<Favourite> favourites)
        {
            try
            {
                if (!_paramValidator.IsValidEmployerAccountId(employerAccountId))
                {
                    throw new ArgumentException("Employer account id is invalid.");
                }

                favourites.ForEach(ValidateApprenticeship);

                var response = await _mediator.Send(new SaveApprenticeshipFavouriteCommand
                {
                    EmployerAccountId = employerAccountId,
                    Favourites = MapToWriteModel(favourites)
                });

                if (response.CommandResult == Domain.WriteModel.DomainUpdateStatus.Created)
                    return CreatedAtAction("Get", new { employerAccountId }, string.Empty);

                return NoContent();
            }
            catch (ArgumentException ex)
            {       
                return BadRequest(ex.Message);              
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
                _logger.LogError($"The apprenticeship id {apprenticeship.ApprenticeshipId} is invalid");
                throw new ArgumentException("An apprenticeship id is invalid");
            }

            apprenticeship.Providers.ToList().ForEach(ValidateProviders);
        }

        private void ValidateProviders(Provider provider)
        {
            if (!_paramValidator.IsValidProviderUkprn(provider.Ukprn))
            {
                _logger.LogError($"The Ukprn {provider.Ukprn} is invalid");
                throw new ArgumentException("A ukprn is invalid");
            }
            
            provider.LocationIds.ForEach(ValidateLocationId);
        }

        private void ValidateLocationId(int locationId)
        {
            if (!_paramValidator.IsValidLocationId(locationId))
            {
                _logger.LogError($"The LocationId {locationId} is invalid");
                throw new ArgumentException("A locationId is invalid");
            }
        }

        private ApprenticeshipFavourites MapToWriteModel(List<Favourite> favourites)
        {
            var writeModel = new ApprenticeshipFavourites();

            if (favourites == null)
            {
                return writeModel;
            }

            var items = favourites.Select(x => new ApprenticeshipFavourite
            {
                ApprenticeshipId = x.ApprenticeshipId,
                Providers = x.Providers.Select(s => new WriteModel.Provider() { Ukprn = s.Ukprn, LocationIds = s.LocationIds }).ToList()
            });

            writeModel.AddRange(items);

            return writeModel;
        }

        // DELETE api/Apprenticeships/ABC123/123-1-2
        /// <summary>
        /// deletes a single apprenticeship favourite from the Employer Account provided
        /// </summary>
        /// <param name="employerAccountId">Hashed Employer Account Id</param>
        /// <param name="apprenticeshipId">Apprenticeship Id</param>
        /// <response code="204">Apprenticeship deleted from Employer Favourite </response>
        /// <response code="404">No Employer favourites found for Employer account Id or apprenticeship does not exist in favourites</response>
        /// <response code="400">The Employer account id or apprenticeship Id provided is invalid</response>  
        /// <response code="401">The client is not authorized to access this endpoint</response>   
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Route("{employerAccountId}/{apprenticeshipId}")]
        public async Task<IActionResult> Delete(string employerAccountId, string apprenticeshipId)
        {
            try
            {
                var response = await _mediator.Send(new DeleteApprenticeshipFavouriteCommand()
                { EmployerAccountId = employerAccountId, ApprenticeshipId = apprenticeshipId });

                if (response.CommandResult == DomainUpdateStatus.Failed)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid arguments were provided for delete apprenticeship favourite");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in delete apprenticeship favourites");
                return StatusCode(500, ServerErrorMessage);
            }
        }

        // DELETE api/Apprenticeships/ABC123/123-1-2/123456789
        /// <summary>
        /// deletes a provider from apprenticeship favourite of the Employer Account provided
        /// </summary>
        /// <param name="employerAccountId">Hashed Employer Account Id</param>
        /// <param name="apprenticeshipId">Apprenticeship Id</param>
        /// <param name="ukprn">Ukprn</param>
        /// <response code="204">provider deleted from Employer apprenticeship Favourite </response>
        /// <response code="404">No Employer favourites found for Employer account Id or apprenticeship/ukprn does not exist in favourites</response>
        /// <response code="400">The Employer account id, apprenticeship Id or ukprn provided is invalid</response>  
        /// <response code="401">The client is not authorized to access this endpoint</response>   
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Route("{employerAccountId}/{apprenticeshipId}/{ukprn}")]
        public async Task<IActionResult> Delete(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            try
            {
                var response = await _mediator.Send(new DeleteProviderFavouriteCommand()
                {
                    EmployerAccountId = employerAccountId,
                    ApprenticeshipId = apprenticeshipId, Ukprn = ukprn
                });

                if (response.CommandResult == DomainUpdateStatus.Failed)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid arguments were provided for delete apprenticeship provider favourites");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in delete apprenticeship provider favourites");
                return StatusCode(500, ServerErrorMessage);
            }
        }
    }
}
