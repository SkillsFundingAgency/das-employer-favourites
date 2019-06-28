using System;
using System.Linq;
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
                Ukprn = src.Providers.FirstOrDefault()?.Ukprn,
                Level = GetLevelText(src.Level),
                TypicalLength = $"{src.TypicalLength} months",
                ExpiryDate = src.ExpiryDate?.AddDays(1).ToString("d MMMM yyyy"),
                FatUrl = _linkGenerator.GetApprenticeshipPageUrl(src)
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
