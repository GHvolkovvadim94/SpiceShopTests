using SpiceShop.Helpers;

namespace SpiceShop;

public static class Program
{
    public static void Main()
    {
        _ = SpiceShopClientFactory.Create();
    }
}