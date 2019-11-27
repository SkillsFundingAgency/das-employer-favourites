using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Web.Models
{
    public class LocationViewModel : AddressViewModel
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
    }
}
