﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Domain;
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
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020 }
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
                    TypicalLength = 18,
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020, Phone = "020 1234 5678", Email = "test@test.com", Website = new Uri("https://www.testprovider.com"), EmployerSatisfaction = 86, LearnerSatisfaction = 98 },
                        new ReadModel.Provider { Name = "Test Provider2", Ukprn = 10000028 }
                    }
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "456",
                    Title = "Test Standard2",
                    Level = 3,
                    TypicalLength = 18,
                },
                new ReadModel.ApprenticeshipFavourite
                {
                    ApprenticeshipId = "123-1-2",
                    Title = "Test Framework1",
                    Level = 3,
                    TypicalLength = 18,
                    ExpiryDate = new DateTime(2020, 1, 1),
                    Providers = new List<ReadModel.Provider>
                    {
                        new ReadModel.Provider { Name = "Test Provider", Ukprn = 10000020 }
                    }
                }
            };
        }

    }
}