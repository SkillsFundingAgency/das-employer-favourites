using System.Collections.Generic;

namespace DfE.EmployerFavourites.Web.Domain
{
    public class EmployerAccount
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string EmployerAccountId { get; internal set; }
    }
}