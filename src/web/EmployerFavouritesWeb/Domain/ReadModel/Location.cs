using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Domain.ReadModel
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Name { get; set; }
    }
}
