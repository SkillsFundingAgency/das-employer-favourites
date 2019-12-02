using System.Collections.Generic;
using System.Linq;

namespace DfE.EmployerFavourites.Domain.WriteModel
{
    public class ApprenticeshipFavourites : List<ApprenticeshipFavourite>
    {
        public bool Update(string apprenticeshipId, IDictionary<int,IList<int>> providers)
        {
            var existing = this.SingleOrDefault(x => x.ApprenticeshipId == apprenticeshipId);
            
            if (existing == null)
            {
                if (providers == null || providers.Count == 0)
                {
                    Add(new ApprenticeshipFavourite(apprenticeshipId));
                    return true;
                }
                else
                {
                    Add(new ApprenticeshipFavourite(apprenticeshipId, providers));
                    return true;
                }
            }
            else
            {
                if (providers != null ) //|| providers.Count > 0)
                {
                    var changeMade = false;

                    var existingUkprns = existing.Providers.Select(x => x.Ukprn).ToList();
                    
                    foreach (var provider in providers)
                    {

                        if (!existingUkprns.Contains(provider.Key))
                        {
                            existing.Providers.Add(new Provider(provider.Key, provider.Value));

                            changeMade = true;
                        }
                        else
                        {
                            var currentProvider = existingUkprns.FindIndex(x => x == provider.Key);

                            var existingLocations = existing.Providers[currentProvider].LocationIds;

                            foreach (var location in provider.Value)
                            {
                                if (!existingLocations.Contains(location))
                                {
                                    existing.Providers[currentProvider].LocationIds.Add(location);
                                    changeMade = true;
                                }
                            }
                        }
                    }

                    return changeMade;
                }
            }

            return false;
        }
    }
}