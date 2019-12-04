using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Web.Models
{
    public class AddressViewModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }

        public string GetCommaDelimitedAddress()
        {
            var addressParts = new List<string>(5);
            if (!string.IsNullOrWhiteSpace(Address1)) addressParts.Add(Address1);
            if (!string.IsNullOrWhiteSpace(Address2)) addressParts.Add(Address2);
            if (!string.IsNullOrWhiteSpace(Town)) addressParts.Add(Town);
            if (!string.IsNullOrWhiteSpace(County)) addressParts.Add(County);

            return string.Join(", ", addressParts);
        }

    }
}
