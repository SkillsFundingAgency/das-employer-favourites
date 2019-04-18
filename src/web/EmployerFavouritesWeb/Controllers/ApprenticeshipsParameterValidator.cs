using System.Text.RegularExpressions;

namespace DfE.EmployerFavourites.Web.Controllers
{
    internal class ApprenticeshipsParameterValidator
    {
        private Regex frameworkRegex = new Regex(@"^\d{3}-\d{1,2}-\d{1,2}$");

        public bool IsValidApprenticeshipId(string id)
        {
            if (id == null)
                return false;

            if (int.TryParse(id, out int standarCode) && standarCode > 0)
                return true; // Standard
            
            if (frameworkRegex.Match(id).Success)
                return true; // Framework

            return false;
        }

        public bool IsValidProviderUkprn(int? ukprn)
        {
            if (ukprn == null || (ukprn > 9999999 && ukprn < 100000000))
                return true;
            
            return false;
        }
    }
}
