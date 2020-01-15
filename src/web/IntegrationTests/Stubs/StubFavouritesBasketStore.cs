using System;
using System.Threading.Tasks;
using Sfa.Das.Sas.Shared.Basket.Interfaces;
using Sfa.Das.Sas.Shared.Basket.Models;

namespace DfE.EmployerFavourites.Web.IntegrationTests.Stubs
{
    public class StubFavouritesBasketStore : IApprenticeshipFavouritesBasketStore
    {
        public Task RemoveAsync(Guid basketId)
        {
            return Task.CompletedTask;
        }

        public Task<ApprenticeshipFavouritesBasket> GetAsync(Guid basketId)
        {
            var basket = new ApprenticeshipFavouritesBasket();
            basket.Add("123456");

            return Task.FromResult(basket); 
        }

        public Task UpdateAsync(ApprenticeshipFavouritesBasket basket)
        {
            return Task.CompletedTask;
        }
    }
}
