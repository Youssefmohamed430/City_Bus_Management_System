using City_Bus_Management_System.DataLayer.Data;
using Core_Layer.IRepositries;
using Mapster;
namespace Data_Access_Layer.Repositries
{
    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        public AppDbContext context { get; set; }
        public WalletRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public TDto GetWalletByPassengerId<TDto>(string passengerId)
        {
            var wallet = context.Wallets
                .Include(w => w.passenger)
                .ThenInclude(p => p.User)
                .Where(w => w.passengerId == passengerId)
                .ProjectToType<TDto>()
                .FirstOrDefault();

            return wallet!;
        }
    }
}
