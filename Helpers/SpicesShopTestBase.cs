using NUnit.Framework;
using SpiceShop.Services.Interfaces;

namespace SpiceShop.Helpers;

public abstract class SpicesShopTestBase
{
    protected ISpicesShopClient Client { get; private set; }

    [SetUp]
    public void SetUp() => 
        Client = SpiceShopClientFactory.Create();
}