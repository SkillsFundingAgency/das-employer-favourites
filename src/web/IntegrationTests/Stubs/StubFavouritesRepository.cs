using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
using DfE.EmployerFavourites.Domain.ReadModel;
using ReadModel = DfE.EmployerFavourites.Domain.ReadModel;
using WriteModel = DfE.EmployerFavourites.Domain.WriteModel;

namespace DfE.EmployerFavourites.Web.IntegrationTests.Stubs
{
    internal class StubFavouritesRepository : IFavouritesReadRepository, IFavouritesWriteRepository
    {
        public Task<ReadModel.ApprenticeshipFavourites> GetApprenticeshipFavourites(string employerAccountId)
        {
            ReadModel.ApprenticeshipFavourites favourites;

            switch(employerAccountId)
            {
                case "NORESULTS":
                    favourites = GenerateEmpty();
                    break;
                case "SINGLERESULT":
                    favourites = GenerateSingle();
                    break;
                case "ONELOCATION":
                    favourites = GenerateSingleWithOneLocation();
                    break;
                default:
                    favourites = GenerateMultiple();
                    break;
            }
            
            return Task.FromResult(favourites);
        }

        public Task SaveApprenticeshipFavourites(string employerAccountId, WriteModel.ApprenticeshipFavourites apprenticeshipFavourite)
        {
            return Task.CompletedTask;
        }

        public Task DeleteApprenticeshipFavourites(string employerAccountId, string apprenticeshipId)
        {
            return Task.CompletedTask;
        }

        public Task DeleteApprenticeshipProviderFavourites(string employerAccountId, string apprenticeshipId, int ukprn)
        {
            return Task.CompletedTask;
        }

        private ReadModel.ApprenticeshipFavourites GenerateEmpty()
        {
            return new ReadModel.ApprenticeshipFavourites();
        }

        private ReadModel.ApprenticeshipFavourites GenerateSingle()
        {
            return new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "123",
                    Title = "Test Standard1",
                    Level = 3,
                    TypicalLength = 24,
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider",
                            Ukprn = 10000020,
                            Phone = "020 1234 5678",
                            Email = "test@test.com",
                            Website = new Uri("https://www.testprovider.com"),
                            EmployerSatisfaction = 86,
                            LearnerSatisfaction = 98,
                            Active = true,
                            Address = new Infrastructure.FatApiClient.ProviderAddress
                            {
                                Primary = "1 Head Office",
                                Secondary = "Training Provider",
                                Street = "Training Provider Street",
                                Town = "Training",
                                Postcode = "AA1 1BB",
                                ContactType = "LEGAL"
                            }
                        },
                    }
                }
            };
        }

        private ReadModel.ApprenticeshipFavourites GenerateMultiple()
        {
            return new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "123",
                    Title = "Test Standard1",
                    Level = 3,
                    Active = true,
                    TypicalLength = 18,
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider",
                            Ukprn = 10000020,
                            Phone = "020 1234 5678",
                            Email = "test@test.com",
                            Website = new Uri("https://www.testprovider.com"),
                            EmployerSatisfaction = 86,
                            LearnerSatisfaction = 98,
                            Active = true,
                            Address = new Infrastructure.FatApiClient.ProviderAddress
                            {
                                Primary = "1 Head Office",
                                Secondary = "Training Provider",
                                Street = "Training Provider Street",
                                Town = "Training",
                                Postcode = "AA1 1BB",
                                ContactType = "LEGAL"
                            }
                        },
                        new ReadModel.Provider { Name = "Test Provider2", Ukprn = 10000028, Active = true, Address = new Infrastructure.FatApiClient.ProviderAddress { Primary = "1 Head Office",
                                Secondary = "Training Provider",
                                Street = "Training Provider Street",
                                Town = "Training",
                                Postcode = "AA1 1BB",
                                ContactType = "LEGAL"} }
                    }
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "456",
                    Title = "Test Standard2",
                    Active = true,
                    Level = 3,
                    TypicalLength = 18,
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "123-1-2",
                    Title = "Test Framework1",
                    Active = true,
                    Level = 3,
                    TypicalLength = 18,
                    ExpiryDate = new DateTime(2020, 1, 1),
                    Providers = new List<ReadModel.Provider>
                    {
                       new ReadModel.Provider { Name = "Test Provider",
                            Ukprn = 10000020,
                            Phone = "020 1234 5678",
                            Email = "test@test.com",
                            Website = new Uri("https://www.testprovider.com"),
                            EmployerSatisfaction = 86,
                            LearnerSatisfaction = 98,
                            Active = true,
                            Address = new Infrastructure.FatApiClient.ProviderAddress
                            {
                                Primary = "1 Head Office",
                                Secondary = "Training Provider",
                                Street = "Training Provider Street",
                                Town = "Training",
                                Postcode = "AA1 1BB",
                                ContactType = "LEGAL"

                            }
                            
                        },
                    }
                }
            };
        }

        private ReadModel.ApprenticeshipFavourites GenerateSingleWithOneLocation()
        {
            var newFavourites = new ReadModel.ApprenticeshipFavourites
            {
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "890",
                    Title = "Test Standard1",
                    Level = 3,
                    TypicalLength = 24,
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020,  Address = new Infrastructure.FatApiClient.ProviderAddress
                            {
                                Primary = "1 Head Office",
                                Secondary = "Training Provider",
                                Street = "Training Provider Street",
                                Town = "Training",
                                Postcode = "AA1 1BB",
                                ContactType = "LEGAL"

                            }, LocationIds = new List<int> { 1 }, 
                                                Locations = new List<Location> { new Location { Address1 = "1 Address One", Address2 = "Address 2", PostCode = "AA1 2BB", LocationId = 1, Name = "Test Location 1" } }

                        }
                    },
                   
                }
            };

            return newFavourites;
        }

    }
}