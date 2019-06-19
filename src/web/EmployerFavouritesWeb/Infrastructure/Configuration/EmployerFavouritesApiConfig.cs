using System;
using System.Linq;
using System.Reflection;

namespace DfE.EmployerFavourites.Infrastructure.Configuration
{
    public class EmployerFavouritesApiConfig
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }

        public bool HasEmptyProperties()
        {
            var type = GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var allPropertiesPopulated = properties.Select(x => x.GetValue(this, null))
                .All(y => y != null && !String.IsNullOrWhiteSpace(y.ToString()));
            return !allPropertiesPopulated;
        }
    }
}
