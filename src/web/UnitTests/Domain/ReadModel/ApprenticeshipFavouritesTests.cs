using System.Collections.Generic;
using DfE.EmployerFavourites.Domain.ReadModel;
using Xunit;

namespace DfE.EmployerFavourites.UnitTests.Domain.ReadModel
{
    public class ApprenticeshipFavouritesTests
    {
        [Fact]
        public void MapToWriteModel_ReturnWriteModelWithPropertiesPopulated()
        {
            var sut = new ApprenticeshipFavourites
            {
                new ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = GetListOfTestProviders() },
                new ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            var result = sut.MapToWriteModel();

            Assert.NotNull(result);
            Assert.Equal(sut.Count, result.Count);

            Assert.Collection(result,
                item => { Assert.Equal("ABC123", item.ApprenticeshipId); Assert.Equal(new List<int> { 1, 2, 3 }, item.Ukprns); },
                item => { Assert.Equal("XYZ123", item.ApprenticeshipId); Assert.Equal(0, item.Ukprns.Count); });
        }


        private List<Provider> GetListOfTestProviders()
        {
            return new List<Provider>
            {
                new Provider { Ukprn = 1 },
                new Provider { Ukprn = 2 },
                new Provider { Ukprn = 3 }
            };
        }
    }
}
