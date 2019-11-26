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
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" }
            };

            var result = sut.Update("DEF123", null);

            Assert.True(result);
        }

        [Fact]
        public void Update_FromOneApprenticeshipsToAnotherApprenticeshipWithOneTrainingProvidersNoLocation()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" }
            };

            var result = sut.Update("DEF123", new Dictionary<int, IList<int>> { { 12345678, null } });

            Assert.True(result);
        }

        [Fact]
        public void Update_FromOneApprenticeshipsToAnotherApprenticeshipWithOneTrainingProvidersOneLocation()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" }
            };

            var result = sut.Update("DEF123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1 } } });

            Assert.True(result);
        }

        [Fact]
        public void Update_AddProviderAndLocationToExistingApprenticeship()
        {
            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123" }
            };

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1 } } });

            Assert.True(result);
        }

        [Fact]
        public void Update_AddAnotherLocationToAnExistingProvider()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } }
            };
            
            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 4 } } });

            Assert.True(result);
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
        public void Update_AddingLocationThatAlreadyExists()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } }
            };

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1 } } });

            Assert.False(result);
        }

        [Fact]
        public void Update_AddingMultipleLocationsThatAlreadyExists()
        {

            var sut = new Write.ApprenticeshipFavourites
            {
                new Write.ApprenticeshipFavourite { ApprenticeshipId = "ABC123", Providers = new List<Write.Provider> { new Write.Provider( 12345678, new List<int> { 1, 2, 3 }) } }
            };

            var result = sut.Update("ABC123", new Dictionary<int, IList<int>> { { 12345678, new List<int> { 1, 2, 3 } } });

            //Assert.Equal(5,)
            Assert.False(result);
        }
        // Tests
        // Correctly returns whether changes have been made for each apprenticeship
        // Scenarios

        // empty -> add apprenticeship
        // empty -> add apprenticeship, tp
        // empty -> add apprenticeship, tp, location
        // empty -> add apprenticeship, tp, multiple locations
        // 1 apprenticeship -> add a different apprenticeship
        // 1 apprenticeship -> add a different apprenticeship + tp
        // 1 apprenticeship -> add a different apprenticeship, tp, location
        // 1 apprenticeship -> add identical apprenticeship, 
        // 1 apprenticeship -> add identical apprenticeship, tp
        // 1 apprenticeship -> add identical apprenticeship, tp, multiple location
        // 1 apprenticeship -> add identical apprenticeship, tp, multiple locations
        // 2 apprenticeships -> add a different apprenticeship
        // 2 apprenticeships -> add a different apprenticeship + tp
        // 2 apprenticeships -> add a different apprenticeship, tp, location
        // 2 apprenticeships -> add identical apprenticeship, 
        // 2 apprenticeships -> add identical apprenticeship, tp
        // 2 apprenticeships -> add identical apprenticeship, tp, multiple location
        // 2 apprenticeships -> add identical apprenticeship, tp, multiple locations


        private IDictionary<int, IList<int>> GetDictionaryOfApprenticeshipWithNoProvider()
        {
            return new Dictionary<int, IList<int>>
            {
                { 10000020, null }

            };
        }
        private IList<Write.Provider> GetNoProvider()
        {
            return new List<Write.Provider> { };
        }

        private Dictionary<int, IList<int>> GetDictionaryOfApprenticeshipWithOneProvider()
        {
            return new Dictionary<int, IList<int>>
            {
                { 10000020, new List<int> { 1 } }
            };
        }
    }
}
