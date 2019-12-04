using System.Text.RegularExpressions;

namespace DfE.EmployerFavourites.Api.Validation
{
    internal class ApprenticeshipsParameterValidator
    {
        readonly Regex _frameworkRegex = new Regex(@"^\d{3}-\d{1,2}-\d{1,2}$");
        readonly Regex _empAccRegEx = new Regex(@"^.{6,}$");

        internal bool IsValidApprenticeshipId(string id)
        {
            if (id == null)
                return false;

            if (int.TryParse(id, out int standarCode) && standarCode > 0)
                return true; // Standard

            if (_frameworkRegex.Match(id).Success)
                return true; // Framework

            return false;
        }

        internal bool IsValidProviderUkprn(int? ukprn)
        {
            if (ukprn == null || (ukprn > 9999999 && ukprn < 100000000))
                return true;
            
            return false;
        }

        internal bool IsValidEmployerAccountId(string employerAccountId)
        {
            if (employerAccountId == null)
                return false;

            return _empAccRegEx.Match(employerAccountId).Success;
        }
    }
}
