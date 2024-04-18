using System;
using System.Text.RegularExpressions;
using SpiceShop.Enums;
using SpiceShop.Models;
using SpiceShop.Services.Interfaces;
using SpiceShop.Storage;
using SpiceShop.Util;

namespace SpiceShop.Services;

public class OrdersService : IOrdersService
{
    protected readonly IStore Store;

    public OrdersService(IStore store) =>
        this.Store = store;

    public virtual Guid CreateOrder(Guid spiceId, int quantity, string customerName)
    {
        ValidateOrder(spiceId, quantity, customerName);

        UpdateAvailableSpice(spiceId, quantity);
        return this.Store.AddOrUpdate(new Order
        {
            Id = Guid.NewGuid(),
            Quantity = quantity,
            CustomerName = customerName,
            SpiceId = spiceId
        });
    }

    public Order GetOrder(Guid orderId) =>
        this.Store.Get<Order>(orderId);

    public Order[] GetAllOrders() =>
        this.Store.GetAll<Order>();

    protected virtual void UpdateAvailableSpice(Guid spiceId, int quantity)
    {
        this.Store.Update<Spice>(spiceId, spice => spice.Available -= quantity);
    }

    protected virtual void ValidateOrder(Guid spiceId, int quantity, string customerName)
    {
        var spice = GetSpice(spiceId);

        ValidateQuantity(quantity, spice.Unit);
        ValidateAvailability(quantity, spice.Available);
        ValidateCustomerName(customerName);
    }

    protected virtual Spice GetSpice(Guid spiceId) =>
        this.Store.TryGet<Spice>(spiceId, out var spice)
            ? spice
            : throw new SpiceShopException($"Couldn't find Spice with id {spiceId}");

    protected virtual void ValidateAvailability(int quantity, int available)
    {
        if (quantity > available)
            throw new SpiceShopException("Requested spice count exceeds remainings");
    }

    protected virtual void ValidateCustomerName(string customerName)
    {
        if (customerName is null)
            throw new SpiceShopException("Customer name is null");

        if (customerName.Length is < 3 or > 20)
            throw new SpiceShopException("Customer name should have length in range [3;20]");

        if (string.IsNullOrWhiteSpace(customerName))
            throw new SpiceShopException("Customer name should contain at least one non-whitespace character");

        if (!Regex.IsMatch(customerName, "^[A-Za-zА-Яа-яЁёЙй ]+$"))
            throw new SpiceShopException("Customer name should contain only cyrillic or latin characters");
    }

    protected virtual void ValidateQuantity(int quantity, UnitType spiceUnit)
    {
        switch (spiceUnit)
        {
            case UnitType.Grams:
                ValidateGramsQuantity(quantity);
                break;
            case UnitType.Pieces:
                ValidatePiecesQuantity(quantity);
                break;
        }
    }

    protected virtual void ValidatePiecesQuantity(int quantity)
    {
        if (quantity is < 1 or > 10)
            throw new SpiceShopException("Quantity should be in range [1;10]");
    }

    protected virtual void ValidateGramsQuantity(int quantity)
    {
        if (quantity is < 10 or > 1000)
            throw new SpiceShopException("Quantity should be in range [10;1000]");
    }
}