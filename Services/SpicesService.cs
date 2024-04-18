using System;
using SpiceShop.Enums;
using SpiceShop.Models;
using SpiceShop.Services.Interfaces;
using SpiceShop.Storage;

namespace SpiceShop.Services;

public class SpicesService : ISpicesService
{
    private readonly IStore store;

    public SpicesService(IStore store) => 
        this.store = store;

    public Guid AddSpice(string name, UnitType unit, int remaining)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (remaining < 0)
            throw new ArgumentOutOfRangeException(nameof(remaining), remaining, "Remaining should be non-negative");

        return this.store.AddOrUpdate(new Spice
        {
            Available = remaining,
            Name = name,
            Unit = unit
        });
    }

    public Spice IncRemaining(Guid spiceId, int value) => 
        this.store.Update<Spice>(spiceId, spice => spice.Available += value);

    public Spice GetSpice(Guid spiceId) => 
        this.store.Get<Spice>(spiceId);

    public Spice[] GetSpices() => 
        this.store.GetAll<Spice>();
}