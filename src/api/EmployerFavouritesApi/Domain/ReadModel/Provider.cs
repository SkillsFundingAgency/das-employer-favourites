using System.Collections.Generic;

namespace DfE.EmployerFavourites.Api.Domain.ReadModel
{
    public class Provider
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public List<int> LocationIds { get; set; }
        public List<Location> Locations { get; set; }
    }
}