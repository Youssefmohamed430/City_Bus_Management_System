namespace Core_Layer.IRepositries
{
    public interface IWalletRepository : IBaseRepository<Wallet>
    {
        TDto GetWalletByPassengerId<TDto>(string passengerId);
    }
}
