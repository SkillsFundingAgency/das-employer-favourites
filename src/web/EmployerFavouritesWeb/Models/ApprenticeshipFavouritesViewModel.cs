using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Models
{
    public class ApprenticeshipFavouritesViewModel
    {
        public IReadOnlyList<ApprenticeshipFavouriteViewModel> Items { get; set; }
        public IEnumerable<char> EmployerAccountId { get; set; }
    }
}