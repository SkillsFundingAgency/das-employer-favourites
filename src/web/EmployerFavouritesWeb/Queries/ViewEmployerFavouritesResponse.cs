using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Domain;

namespace DfE.EmployerFavourites.Web.Queries
{
    public class ViewEmployerFavouritesResponse
    {
        public EmployerAccount EmployerAccount { get; internal set; }
        public ApprenticeshipFavourites EmployerFavourites { get; internal set; }
    }
}