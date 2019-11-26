using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Web.Infrastructure.FatApiClient
{
    public class FatProviderLocationAddress
    {
        public JsonLocation Location { get; set; }
       
    }

    public class JsonLocation
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public JsonAddress Address { get; set; }
      
    }

    public class JsonAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
    }
}

