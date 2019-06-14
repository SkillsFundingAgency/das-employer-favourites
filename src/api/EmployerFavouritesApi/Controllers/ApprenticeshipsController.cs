using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Application.Queries;
using Microsoft.AspNetCore.Mvc.Routing;
using DfE.EmployerFavourites.Api.Application.Commands;

namespace DfE.EmployerFavourites.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApprenticeshipsController : ControllerBase
    {
        private readonly ILogger<ApprenticeshipsController> _logger;
        private readonly IMediator _mediator;

        public ApprenticeshipsController(ILogger<ApprenticeshipsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
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
                var apprenticeships = await _mediator.Send(new GetApprenticeshipFavouritesRequest() { EmployerAccountID = employerAccountId });

                if (apprenticeships.Count > 0)
                {
                    return Ok(apprenticeships);
                }

                return NoContent();
            }

            catch (ArgumentException e)
            {
                _logger.LogError(e,"Invalid arguments were provided for get apprenticeship favourites");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in get apprenticeship favourites");
                return StatusCode(500);
            }
        }

        // PUT api/Apprenticeships/ABC123
        /// <summary>
        /// Save apprenticeship favourite to the Employer Account provided
        /// </summary>
        /// <param name="employerAccountId">Hashed Employer Account Id</param>
        /// <param name="apprenticeshipId">Standard code or Framework Id</param>
        /// <param name="ukprn">Provider Ukprn</param>
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
        public async Task<IActionResult> Put(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            //TODO: LWA validate parameters

            var response = await _mediator.Send(new SaveApprenticeshipFavouriteCommand() 
            { 
                EmployerAccountId = employerAccountId,
                ApprenticeshipId = apprenticeshipId,
                Ukprn = ukprn
            });

            if (response.CommandResult == Domain.WriteModel.DomainUpdateStatus.Created)
                return CreatedAtAction("Get", new { employerAccountId }, string.Empty);
                
            return NoContent();
        }
    }
}
