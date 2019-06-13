using DfE.EmployerFavourites.Api.Controllers;
using DfE.EmployerFavourites.Api.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace DfE.EmployerFavourites.Api.UnitTests.Controllers
{
    public class ApprenticeshipsControllerTests
    {
        private const string EmployerAccountIdNewList = "ABC123";
        private const string EmployerAccountIdExistingList = "XYZ123";
        private readonly ApprenticeshipsController _sut;
        private readonly Mock<IFavouritesWriteRepository> _mockWriteRepository = new Mock<IFavouritesWriteRepository>();

        public ApprenticeshipsControllerTests()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            _sut = new ApprenticeshipsController(new NullLogger<ApprenticeshipsController>(), mediator);
        }

        [Fact]
        public void Put_ReturnsCreatedResult_WhenNewListCreatedForEmployerAccount()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = _sut.Put(EmployerAccountIdNewList, "50", 10000020);
            
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }

        [Fact]
        public void Put_CreatesNewListInRepo_ForFirstSaveForEmployerAccountId()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = _sut.Put(EmployerAccountIdNewList, "50", 10000020);

            _mockWriteRepository.Verify(x => x.SaveApprenticeshipFavourites(EmployerAccountIdNewList, It.IsAny<Domain.WriteModel.ApprenticeshipFavourites>()));
        }

        [Fact]
        public void Put_ReturnsNoContentResult_WhenNewListCreatedEmployerAccountWithExistingList()
        {
            ServiceProvider provider = BuildDependencies();
            var mediator = provider.GetService<IMediator>();

            var result = _sut.Put(EmployerAccountIdExistingList, "50", 10000020);

            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        private ServiceProvider BuildDependencies()
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddScoped<IFavouritesWriteRepository>(x => _mockWriteRepository.Object);
            var provider = services.BuildServiceProvider();

            return provider;
        }
    }
}
