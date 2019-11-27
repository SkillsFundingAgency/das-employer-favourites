using System;
using System.Linq;
using System.Collections.Generic;
using DfE.EmployerFavourites.Web.Configuration;
using DfE.EmployerFavourites.Web.Helpers;
using DfE.EmployerFavourites.Web.Models;

namespace DfE.EmployerFavourites.Web.Mappers
{
    internal class ApprenticeshipFavouriteMapper
    {
        private readonly FatWebsiteLinkGenerator _linkGenerator;

        public ApprenticeshipFavouriteMapper(FatWebsite fatconfig)
        {
            _linkGenerator = new FatWebsiteLinkGenerator(fatconfig);
        }

        public ApprenticeshipFavouriteViewModel Map(Domain.ReadModel.ApprenticeshipFavourite src)
        {
            return new ApprenticeshipFavouriteViewModel
            {
                Id = src.ApprenticeshipId,
                Title = src.Title,
                IsFramework = src.IsFramework,
                HasTrainingProviders = src.Providers.Any(),
                Level = GetLevelText(src.Level),
                TypicalLength = $"{src.TypicalLength} months",
                ExpiryDate = src.ExpiryDate?.AddDays(1).ToString("d MMMM yyyy"),
                FatUrl = _linkGenerator.GetApprenticeshipPageUrl(src),
                Active = src.Active
            };
        }

        public TrainingProviderViewModel Map(Domain.ReadModel.Provider src)
        {
            const string NO_DATA_AVAILABLE_MSG = "no data available";

            return new TrainingProviderViewModel
            {
                Ukprn = src.Ukprn,
                ProviderName = src.Name,
                Phone = string.IsNullOrEmpty(src.Phone) ? NO_DATA_AVAILABLE_MSG : src.Phone,
                Email = string.IsNullOrEmpty(src.Email) ? NO_DATA_AVAILABLE_MSG : src.Email,
                Website = src.Website == null || string.IsNullOrEmpty(src.Website.ToString()) ? NO_DATA_AVAILABLE_MSG : src.Website.ToString(),
                HeadOfficeAddress = src.Address != null ? Map(src.Address) : new AddressViewModel { Address1 = NO_DATA_AVAILABLE_MSG, Address2 = NO_DATA_AVAILABLE_MSG, County = NO_DATA_AVAILABLE_MSG, PostCode = NO_DATA_AVAILABLE_MSG, Town = NO_DATA_AVAILABLE_MSG },
                Locations =  src.Locations?.Select(Map).ToList() ?? new List<LocationViewModel>(),

                // This below logic for Satisfaction values is copied from FAT website for Provider Details
                // This however calls the FAT API which always returns 0 even if the data doesn't actually exist
                // This is not how the API call for provider location works where is doesn't return the property if no data exists.
                EmployerSatisfaction = src.EmployerSatisfaction > 0 ? $"{src.EmployerSatisfaction}%" : NO_DATA_AVAILABLE_MSG,
                LearnerSatisfaction = src.LearnerSatisfaction > 0 ? $"{src.LearnerSatisfaction}%" : NO_DATA_AVAILABLE_MSG,
                FatUrl = _linkGenerator.GetProviderPageUrl(src),
                Active = src.Active
            };
        }

        private static LocationViewModel Map(Domain.ReadModel.Location src)
        {
            return new LocationViewModel
            {
                Address1 = src.Address1,
                Address2 = src.Address2,
                Town = src.Town,
                PostCode = src.PostCode,
                County = src.County,
                Name = src.Name 
            };
        }

        private static AddressViewModel Map(Infrastructure.FatApiClient.ProviderAddress src)
        {
            return new AddressViewModel
            {
                Address1 = src.Primary,
                Address2 = src.Secondary,
                Town = src.Town,
                PostCode = src.Postcode
            };
        }

        private string GetLevelText(byte level)
        {

            switch (level)
            {
                case 1:
                    return "1 (equivalent to GCSEs at grades D to G)";
                case 2:
                    return "2 (equivalent to GCSEs at grades A* to C)";
                case 3:
                    return "3 (equivalent to A levels at grades A to E)";
                case 4:
                    return "4 (equivalent to certificate of higher education)";
                case 5:
                    return "5 (equivalent to foundation degree)";
                case 6:
                    return "6 (equivalent to bachelor's degree)";
                case 7:
                    return "7 (equivalent to master’s degree)";
                case 8:
                    return "8 (equivalent to doctorate)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), $"Unexpected value for level: {level}");
            }
        }
    }
}
