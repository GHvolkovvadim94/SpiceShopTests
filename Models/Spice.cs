using System;
using SpiceShop.Enums;

namespace SpiceShop.Models;

public class Spice : IEntity
{
    /// <summary> Идентификатор </summary>
    public Guid Id { get; set; }

    /// <inheritdoc cref="UnitType"/>
    public UnitType Unit { get; set; }

    /// <summary> Название </summary>
    public string Name { get; set; }

    /// <summary> Доступное для заказа количество </summary>
    public int Available { get; set; }
}