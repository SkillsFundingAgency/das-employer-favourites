using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.EmployerFavourites.ApplicationServices.Commands;
using DfE.EmployerFavourites.Web.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DfE.EmployerFavourites.Api.Controllers
{
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

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get(string employerAccountId)
        {
            try
            {

                var apprenticeships = await _mediator.Send(new GetApprenticeshipFavouritesRequest() {EmployerAccountID = employerAccountId});

                return Ok(apprenticeships);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}
