using DfE.EmployerFavourites.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DfE.EmployerFavourites.Api.Infrastructure.Interfaces
{
    public interface IFatRepository
    {
        Task<string> GetApprenticeshipNameAsync(string apprenticeshipId);
        Task<string> GetProviderNameAsync(int ukprn);
    }
}