namespace DfE.EmployerFavourites.Domain.ReadModel
{
    public class EmployerAccount
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string EmployerAccountId { get; internal set; }
    }
}