using System.Collections.Generic;
using System.Linq;
using Xunit;
using Read = DfE.EmployerFavourites.Api.Domain.ReadModel;
using Write = DfE.EmployerFavourites.Api.Domain.WriteModel;

namespace DfE.EmployerFavourites.Api.UnitTests.Domain.ReadModel
{
    public class ApprenticeshipFavouritesTests
    {
        private readonly string _apprenticeshipId = "ABC123";

        [Fact]
        public void MapToWriteModel_ReturnWriteModelWithPropertiesPopulated()
        {
            var sut = new Read.ApprenticeshipFavourites
            {
                new Read.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = GetListOfTestProviders() },
                new Read.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            var result = sut.MapToWriteModel();

            Assert.NotNull(result);
            Assert.Equal(sut.Count, result.Count);

            //Assert.Collection(result,
            //    item => { Assert.Equal("ABC123", item.ApprenticeshipId); Assert.Equal(new List<int> { 1, 2, 3 }, item.Ukprns); },
            //    item => { Assert.Equal("XYZ123", item.ApprenticeshipId); Assert.Equal(0, item.Ukprns.Count); });
        }


        private List<Read.Provider> GetListOfTestProviders()
        {
            return new List<Read.Provider>
            {
                new Read.Provider { Ukprn = 1 },
                new Read.Provider { Ukprn = 2 },
                new Read.Provider { Ukprn = 3 }
            };
        }

        private List<int> GetIntListOfTestProviders()
        {
            return new List<int>
            {
               1,2,3
            };
        }

        [Fact]
        public void WhenRemoveCalledWithApprenticeshipId_ThenRemoveApprenticeshipFromList()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = _apprenticeshipId, Ukprns = GetIntListOfTestProviders() },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            sut.Remove("ABC123");

            Assert.NotNull(sut);
            Assert.Equal(1,sut.Count);
        }

        [Fact]
        public void WhenRemoveCalledWithApprenticeshipIdAndUkprn_ThenRemoveUkprnsFromList()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = _apprenticeshipId, Ukprns = GetIntListOfTestProviders() },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            sut.Remove(_apprenticeshipId, 1);

            Assert.NotNull(sut);
            Assert.Equal(2, sut.Count);

            Assert.Equal(2,sut.FirstOrDefault(w => w.ApprenticeshipId != null && w.ApprenticeshipId == _apprenticeshipId).Ukprns.Count);
        }

    }
}
