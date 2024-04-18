using SpiceShop.Services.Interfaces;
using SpiceShop.Storage;

namespace SpiceShop.Services;

public class SpicesShopClient : ISpicesShopClient
{
    public SpicesShopClient(IOrdersService orders, ISpicesService spices)
    {
        Orders = orders;
        Spices = spices;
    }

    public IOrdersService Orders { get; }

    public ISpicesService Spices { get; }

    public static SpicesShopClient Create(IStore store = null)
    {
        store ??= new InMemoryStore();

        return new SpicesShopClient(
            new OrdersService(store), 
            new SpicesService(store)
        );
    }
}