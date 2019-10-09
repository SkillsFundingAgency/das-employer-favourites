using DfE.EmployerFavourites.Web.Models;

namespace DfE.EmployerFavourites.Web.Controllers
{
    public class DeleteTrainingProviderViewModel
    {
        public string EmployerAccountId { get; set; }
        public string ApprenticeshipId { get; set; }
        public int Ukprn { get; set; }
        public TrainingProviderViewModel Provider { get; set; }
    }
}