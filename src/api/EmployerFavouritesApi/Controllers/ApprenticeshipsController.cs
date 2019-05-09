using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Domain;
using DfE.EmployerFavourites.ApplicationServices.Queries;

namespace DfE.EmployerFavourites.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
        public async Task<ActionResult<ApprenticeshipFavourites>> Get(string employerAccountId)
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
                Console.WriteLine(e);
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
    }
}
