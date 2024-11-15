using MongoDB.Bson;
using MongoDB.Driver;
using Inventory.Domain;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Persistence.Database;


public class MongoDatabaseWrapper(
    IMongoDatabase _database,
    IOptions<MongoDbSettings> _options,
    ILogger<MongoDatabaseWrapper> _logger
)
{
    #region Database Options
    private int MaxRetries => _options.Value.MaxRetries;
    private int BatchSize => _options.Value.BatchSize;
    private string DocumentsCollection => _options.Value.CollectionName;
    private string SummariesCollection => _options.Value.CollectionNameTwo;
    #endregion

}