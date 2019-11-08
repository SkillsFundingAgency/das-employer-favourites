using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Api.Models
{
    public class LocationData
    {
        public int LocationId { get; set; }

        public string LocationAddress { get; set; }
        public string LocationPostcode { get; set; }
        public string LocationPhone { get; set; }
        public string LocationEmail { get; set; }

    }
}
