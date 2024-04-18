using System;
using SpiceShop.Models;

namespace SpiceShop.Services.Interfaces;

public interface IOrdersService
{
    Guid CreateOrder(Guid spiceId, int quantity, string customerName);

    Order GetOrder(Guid orderId);

    Order[] GetAllOrders();
}