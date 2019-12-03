using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using Read = DfE.EmployerFavourites.Api.Domain.ReadModel;
using Write = DfE.EmployerFavourites.Api.Domain.WriteModel;
using System;

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
                new Read.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = GetReadModelListOfTestProviders() },
                new Read.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            var expected = new List<Write.ApprenticeshipFavourite>()
            {
                new Write.ApprenticeshipFavourite()
                {
                    ApprenticeshipId = "ABC123",
                    Providers = new List<Write.Provider>
                    {
                        new Write.Provider { Ukprn = 1},
                        new Write.Provider { Ukprn = 2, LocationIds = new List<int> { 1, 2}}
                    }
                },
                new Write.ApprenticeshipFavourite()
                {
                    ApprenticeshipId = "XYZ123"
                }
            };

            var result = sut.MapToWriteModel();

            Assert.NotNull(result);
            Assert.Equal(sut.Count, result.Count);

            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void WhenRemoveCalledWithApprenticeshipId_ThenRemoveApprenticeshipFromList()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = _apprenticeshipId, Providers = GetWriteModelListOfTestProviders() },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            sut.Remove("ABC123");

            Assert.Single(sut);
        }

        [Fact]
        public void WhenRemoveCalledWithApprenticeshipIdAndUkprn_ThenRemoveUkprnsFromList()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = _apprenticeshipId, Providers = GetWriteModelListOfTestProviders() },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "XYZ123" }
            };

            sut.Remove(_apprenticeshipId, 1);

            Assert.NotNull(sut);
            Assert.Equal(2, sut.Count);

            Assert.Equal(2, sut.FirstOrDefault(w => w.ApprenticeshipId != null && w.ApprenticeshipId == _apprenticeshipId).Providers.Count);

        }

        private IList<Read.Provider> GetReadModelListOfTestProviders( )
        {
            return new List<Read.Provider>
            {
                new Read.Provider { Ukprn = 1 },
                new Read.Provider { Ukprn = 2, LocationIds = GetReadModelListOfTestLocationIds() }
            };
        }

        private List<int> GetReadModelListOfTestLocationIds()
        {
            return new List<int> { 1, 2 };
        }

        private IList<Write.Provider> GetWriteModelListOfTestProviders()
        {
            return new List<Write.Provider>
            {
                new Write.Provider { Ukprn = 1 },
                new Write.Provider { Ukprn = 2 , LocationIds = new List<int> { 100, 101, 102 } },
                new Write.Provider { Ukprn = 3 }
            };
        }

        
    }
}
