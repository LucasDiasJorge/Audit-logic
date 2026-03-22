namespace Audit_Project.Domain;

/// <summary>
/// Base class for all entities. Provides common properties: Id, CreatedAt, UpdatedAt, and IsDeleted.
/// </summary>
public class BaseModel
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Date and time when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the entity was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Indicates if the entity is logically deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
