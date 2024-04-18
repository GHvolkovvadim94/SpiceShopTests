using System;

namespace SpiceShop.Models;

public interface IEntity
{
    public Guid Id { get; set; }
}