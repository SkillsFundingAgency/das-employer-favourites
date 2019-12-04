using System;
using System.Collections.Generic;
using DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using DfE.EmployerFavourites.Web.Infrastructure;

using Xunit;
using DfE.EmployerFavourites.Infrastructure;

namespace DfE.EmployerFavourites.Web.UnitTests.Domain.ReadModel
{
    public class ApprenticeshipFavouriteTests
    {
        [Fact]
        public void IsFramework_ReturnException_WhenApprenticeshipIdNotSet()
        {
            var sut = new ApprenticeshipFavourite();

            Assert.Throws<ArgumentException>(() => sut.IsFramework);      
        }

        [Theory]
        [InlineData("420-2-1")]
        [InlineData("321-22-1")]
        public void IsFramework_ReturnTrue_WhenFramework(string id)
        {
            var sut = new ApprenticeshipFavourite
            {
                ApprenticeshipId = id
            };

            Assert.True(sut.IsFramework);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("22")]
        public void IsFramework_ReturnFalse_WhenStandard(string id)
        {
            var sut = new ApprenticeshipFavourite
            {
                ApprenticeshipId = id
            };

            Assert.False(sut.IsFramework);
        }
    }
}
