using System;
using SpiceShop.Enums;
using SpiceShop.Models;

namespace SpiceShop.Services.Interfaces;

public interface ISpicesService
{
    Guid AddSpice(string name, UnitType unit, int remaining);

    Spice IncRemaining(Guid spiceId, int value);

    Spice GetSpice(Guid spiceId);

    Spice[] GetSpices();
}