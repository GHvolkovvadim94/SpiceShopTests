using System;

namespace SpiceShop.Models;

public class Order : IEntity
{
    /// <summary> Идентификатор заказа </summary>
    public Guid Id { get; set; }

    /// <summary> Идентификатор заказанного продукта </summary>
    public Guid SpiceId { get; set; }

    /// <summary> Количество товара для заказа </summary>
    public int Quantity { get; set; }

    /// <summary> Полное имя покупателя </summary>
    public string CustomerName { get; set; }
}