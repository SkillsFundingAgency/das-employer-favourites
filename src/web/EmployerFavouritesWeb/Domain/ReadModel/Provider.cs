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
        public int EmployerSatisfaction { get; set; }
        public int LearnerSatisfaction { get; set; }
    }
}