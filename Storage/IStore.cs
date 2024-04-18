using System;
using System.Diagnostics.CodeAnalysis;
using SpiceShop.Models;

namespace SpiceShop.Storage;

public interface IStore
{
    /// <summary> Retrieves entity of type <typeparamref name="T"/> with specified <paramref name="id"/> </summary>
    /// <exception cref="ArgumentException"> Unable to find entity <typeparamref name="T"/> with supplied id </exception>
    /// <returns>
    ///     Copy of stored entity with supplied Id.
    ///     Modification of return value doesn't lead to change of stored data
    /// </returns>
    T Get<T>(Guid id) where T : IEntity;

    /// <summary> Retrieves entity of type <typeparamref name="T"/> with specified <paramref name="id"/> </summary>
    /// <param name="id"> Id of entity to get </param>
    /// <param name="value"> Copy of stored entity when found, null otherwise </param> 
    /// <returns> 'true' when entity is found, 'false' otherwise </returns>
    bool TryGet<T>(Guid id, [NotNullWhen(true)] out T value) where T : IEntity;

    /// <summary> Upserts entity of type <typeparamref name="T"/> to store </summary>
    /// <remarks> 
    ///     Copy of supplied entity is stored,
    ///     so modification of original object doesn't lead to modification of stored data
    /// </remarks>
    /// <returns> Id of upserted entity </returns>
    Guid AddOrUpdate<T>(T entity) where T : IEntity;

    /// <summary> Updates entity of <typeparamref name="T"/> with supplied id </summary>
    /// <param name="id"> Id of entity to update </param>
    /// <param name="update"> Callback to update the entity </param>
    /// <exception cref="ArgumentException"> Unable to find entity <typeparamref name="T"/> with supplied id </exception>
    T Update<T>(Guid id, Action<T> update) where T : IEntity;

    /// <summary> Retrieves all stored entities of given type <typeparamref name="T"/> </summary>
    /// <remarks>
    ///     Returns copies of stored entities.
    ///     Modification of these objects doesn't lead to change of stored data
    /// </remarks>
    T[] GetAll<T>() where T : IEntity;
}