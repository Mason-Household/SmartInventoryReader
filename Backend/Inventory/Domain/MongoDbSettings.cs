namespace Inventory.Domain;

public class MongoDbSettings
{
    public int MaxRetries { get; set; }
    public int BatchSize { get; set; }
    public string DocumentsCollection { get; set; }
    public string SummariesCollection { get; set; }
}