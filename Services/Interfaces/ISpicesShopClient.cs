namespace SpiceShop.Services.Interfaces;

public interface ISpicesShopClient
{
    IOrdersService Orders { get; }

    ISpicesService Spices { get; }
}