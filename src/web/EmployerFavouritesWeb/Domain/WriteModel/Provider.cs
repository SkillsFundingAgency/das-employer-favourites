using System.Collections.Generic;
using DfE.EmployerFavourites.Domain.ReadModel;

namespace DfE.EmployerFavourites.Domain.WriteModel
{
    public class Provider
    {
        public Provider(int ukprn, List<Location> locations)
        {
            Ukprn = ukprn;
            Locations = new List<int>();

            if (locations != null)
            {
                foreach (var location in locations)
                {
                    Locations.Add(location.LocationId);
                }
            }
            
        }
        public Provider(int ukprn, IList<int> locations)
        {
            Ukprn = ukprn;
            Locations = locations;
        }

        public int Ukprn { get; set; }
       
        public IList<int> Locations { get; set; }
    }
}