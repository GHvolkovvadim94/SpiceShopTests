using System;
using SpiceShop.Services;
using SpiceShop.Services.Interfaces;
using SpiceShop.Storage;

namespace SpiceShop.Helpers;

public static class SpiceShopClientFactory
{
    private static Func<ISpicesShopClient> createClient = () =>
    {
        var store = new InMemoryStore();
        return new SpicesShopClient(
            new OrdersService(store),
            new SpicesService(store)
        );
    };

    public static ISpicesShopClient Create() =>
        createClient();
}