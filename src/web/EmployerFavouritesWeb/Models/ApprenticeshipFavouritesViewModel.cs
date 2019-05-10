using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class ApprenticeshipFavouritesViewModel
    {
        public string EmployerName { get; set; }
        public IReadOnlyList<ApprenticeshipFavouriteViewModel> Items { get; set; }
    }
}