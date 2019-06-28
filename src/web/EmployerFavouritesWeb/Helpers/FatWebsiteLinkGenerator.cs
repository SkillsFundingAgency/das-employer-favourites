using System;
using DfE.EmployerFavourites.Domain.ReadModel;
using DfE.EmployerFavourites.Web.Configuration;

namespace DfE.EmployerFavourites.Web.Helpers
{
    internal class FatWebsiteLinkGenerator
    {
        private readonly FatWebsite _config;

        public FatWebsiteLinkGenerator(FatWebsite config)
        {
            _config = config;
        }

        public Uri GetApprenticeshipPageUrl(ApprenticeshipFavourite src)
        {
            string type = src.IsFramework ? "Framework" : "Standard";

            return new Uri(string.Format(_config.ApprenticeshipPageTemplate, type, src.ApprenticeshipId));
        }
    }
}
