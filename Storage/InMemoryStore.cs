using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SpiceShop.Models;

namespace SpiceShop.Storage;

public class InMemoryStore : IStore
{
    private readonly Dictionary<(Guid Id, Type Type), object> entities = new();

    /// <inheritdoc/>
    public T Get<T>(Guid id) where T : IEntity =>
        TryGet<T>(id, out var value)
            ? value
            : throw new ArgumentException($"{typeof(T).Name} with {id} doen't exists", nameof(id));

    /// <inheritdoc/>
    public bool TryGet<T>(Guid id, out T value) where T : IEntity
    {
        if (entities.TryGetValue(GetEntityKey<T>(id), out var obj))
        {
            value = Copy((T)obj);
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public Guid AddOrUpdate<T>(T entity) where T : IEntity
    {
        if (entity.Id == default)
            entity.Id = Guid.NewGuid();

        var key = GetEntityKey<T>(entity.Id);
        entities[key] = Copy(entity);

        return entity.Id;
    }

    /// <inheritdoc/>
    public T Update<T>(Guid id, Action<T> update) where T : IEntity
    {
        var entity = Get<T>(id);
        update(entity);

        AddOrUpdate(entity);
        return entity;
    }

    /// <inheritdoc/>
    public T[] GetAll<T>() where T : IEntity =>
        entities
            .Where(x => x.Key.Type == typeof(T))
            .Select(x => x.Value)
            .Cast<T>()
            .ToArray();

    private static (Guid Id, Type) GetEntityKey<T>(Guid id) where T : IEntity => 
        (id, typeof(T));

    private static T Copy<T>(T obj)
    {
        var serialized = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(serialized);
    }
}