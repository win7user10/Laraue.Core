using System;

namespace Laraue.Core.DataAccess.Contracts;

/// <summary>
/// Entity that was deleted.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// When the entity was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}