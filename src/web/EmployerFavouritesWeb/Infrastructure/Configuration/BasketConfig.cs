namespace DfE.EmployerFavourites.Infrastructure.Configuration
{
    public class BasketConfig
    {
        public string BasketRedisConnectionString { get; set; }
        public int SlidingExpiryDays { get; set; }
    }
}
