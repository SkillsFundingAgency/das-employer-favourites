using System;
using System.Threading.Tasks;
using Sfa.Das.Sas.Shared.Basket.Interfaces;
using Sfa.Das.Sas.Shared.Basket.Models;

namespace DfE.EmployerFavourites.Web.IntegrationTests.Stubs
{
    public class StubFavouritesBasketStore : IApprenticeshipFavouritesBasketStore
    {
        public Task<ApprenticeshipFavouritesBasket> GetAsync(Guid basketId)
        {
            return Task.FromResult(new ApprenticeshipFavouritesBasket());
        }

        public Task UpdateAsync(ApprenticeshipFavouritesBasket basket)
        {
            return Task.CompletedTask;
        }
    }
}
