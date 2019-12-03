using DfE.EmployerFavourites.Domain.ReadModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Read = DfE.EmployerFavourites.Domain.ReadModel;
using Write = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.Web.UnitTests.Domain.WriteModel
{
    public class ApprenticeshipFavouritesTests
    {

        [Fact]
        public void Update_FromNoApprenticeshipsToAnApprenticeshipWithNoTrainingProviders()
        {
            var sut = new Write.ApprenticeshipFavourites { }; 
           
            var result = sut.Update("DEF123", null);

            Assert.True(result);
        }

        [Fact]
        public void Update_FromOneApprenticeshipsToAnotherApprenticeshipWithNoTrainingProviders()
        {
            var sut = GetAnApprenticeshipWithNoProvider();

            var result = sut.Update("DEF123", null);

            Assert.True(result);
        }

        [Fact]
        public void Update_FromOneApprenticeshipsToAnotherApprenticeshipWithOneTrainingProvidersNoLocation()
        {
            var sut = GetAnApprenticeshipWithNoProvider();

            var result = sut.Update("DEF123", new Dictionary<int, IList<int>> { { 12345678, null } });

            Assert.True(result);
        }

        [Fact]
        public void Update_FromOneApprenticeshipsToAnotherApprenticeshipWithOneTrainingProvidersOneLocation()
        {
            var sut = GetAnApprenticeshipWithNoProvider();

            var result = sut.Update("DEF123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1 } } });

            Assert.True(result);
        }

        [Fact]
        public void Update_AddProviderAndLocationToExistingApprenticeship()
        {
            var sut = GetAnApprenticeshipWithNoProvider();

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1 } } });

            Assert.True(result);
        }

        [Fact]
        public void Update_AddAnotherLocationToAnExistingProvider()
        {

            var sut = GetAnApprenticeshipWithAProvider();
            
            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 10000020, new List<int> { 4 } } });

            Assert.True(result);
        }
        [Fact]
        public void Update_DoesntAddALocationIdIfItAlreadyExists()
        {

            var sut = GetAnApprenticeshipWithAProvider();

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 10000020, new List<int> { 1 } } });

            Assert.False(result);
        }

        [Fact]
        public void Update_AddMultipleLocationsToAnExistingProvider()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } }
            };

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 4, 5 } } });
            
            Assert.True(result);
        }

        [Fact]
        public void Update_AddApprenticeship()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } }
            };

            var result = sut.Update("XYZ123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 4, 5 } } });

            Assert.True(result);
        }

        [Fact]
        public void Update_AddingMultipleLocationsThatAlreadyExists()
        {

            var sut = GetAnApprenticeshipWithAProvider();

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 10000020, new List<int> { 1, 2, 3 } } });

            Assert.False(result);
        }

        [Fact]
        public void Update_AddingAnApprenticeshipThatIsAlreadyInAListOfApprenticeships()
        {
            var sut = GetMultipleApprenticeshipsWithNoProviders();

            var result = sut.Update("DEF123", null);

            Assert.False(result);
        }

        [Fact]
        public void Update_AddingMultipleApprenticeshipsWithLocationsThatAlreadyExists()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } },

                new Write.ApprenticeshipFavourite { ApprenticeshipId = "DEF123", Providers = new List<Write.Provider> { new Write.Provider( 87654321, new List<int> { 1, 2, 3 }) } }
            };

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1, 2, 3 } } });
            
            Assert.False(result);
        }


        public Write.ApprenticeshipFavourites GetAnApprenticeshipWithNoProvider()
        {
            return new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" }
            };
        }

        public Write.ApprenticeshipFavourites GetAnApprenticeshipWithAProvider()
        {
            return new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" , Providers = new List<Write.Provider> { new Write.Provider(10000020, new List<int> {1, 2, 3 }) }
                }
            };
        }

        public Write.ApprenticeshipFavourites GetMultipleApprenticeshipsWithNoProviders()
        {
            return new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "DEF123" },
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "GHI123" }
            };
        }

    }
}
