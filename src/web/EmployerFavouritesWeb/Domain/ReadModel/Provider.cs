using System;

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
    }
}