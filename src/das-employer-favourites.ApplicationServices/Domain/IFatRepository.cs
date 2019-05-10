using System.Threading.Tasks;

namespace DfE.EmployerFavourites.ApplicationServices.Domain
{
    public interface IFatRepository
    {
        string GetApprenticeshipName(string apprenticeshipId);
    }
}