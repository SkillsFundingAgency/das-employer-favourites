using System;
using System.Collections.Generic;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;

namespace DfE.EmployerFavourites.Domain.ReadModel
{
    public class Provider
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Uri Website { get; set; }
        public double EmployerSatisfaction { get; set; }
        public double LearnerSatisfaction { get; set; }
        public List<Location> Locations { get; set; }
        public ProviderAddress Address { get; set; }
        public IList<int> LocationIds { get; set; }
        public bool Active { get; set; } = false;
    }
}