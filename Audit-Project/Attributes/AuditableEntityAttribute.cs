// Decora cada entidade com o nome da tabela principal e da tabela de log
[AttributeUsage(AttributeTargets.Class)]
public class AuditableEntityAttribute : Attribute
{
    public string TableName    { get; }
    public string LogTableName { get; }

    // Convenção automática: "products" → "product_log"
    // Mas permite sobrescrever se necessário
    public AuditableEntityAttribute(string tableName, string? logTableName = null)
    {
        TableName    = tableName;
        LogTableName = logTableName ?? $"{tableName.TrimEnd('s')}_log";
    }
}