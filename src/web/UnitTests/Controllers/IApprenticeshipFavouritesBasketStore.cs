using System;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Web.UnitTests.Controllers
{
    public interface IApprenticeshipFavouritesBasketStore
    {
        Task<ApprenticeshipFavouritesBasket> GetAsync(Guid basketId);
        Task UpdateAsync(ApprenticeshipFavouritesBasket basket);
    }
}