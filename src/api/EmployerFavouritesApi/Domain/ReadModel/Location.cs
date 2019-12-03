using DfE.EmployerFavourites.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Api.Domain.ReadModel
{
    public class Location
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
