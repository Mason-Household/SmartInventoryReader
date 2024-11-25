namespace Inventory.Properties;

public class ConfigurationConstants
{
    public const string ApiVersion = "1.0";
    public const string JwtKey = "Jwt:Key";
    public const string HealthCheck = "/health";
    public const string DatabaseName = "mongodb";
    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtAudience = "Jwt:Audience";
    public const string JwtExpiryIn = "Jwt:ExpiryIn";
    public const string JwtRefreshToken = "Jwt:Refresh";
    public const string FullLogFilePath = "logs/log-.txt"; 
    public const string AuthenticationScheme = "Bearer";
    public const string MigrationsAssembly = "Inventory";
    public const string MongoDbSettings = "MongoDbSettings";
    public const string DefaultConnection = "DefaultConnection";
    public const string ApplicationURL = "http://0.0.0.0:8080";
    public static readonly string[] SupportedOrigins = [ "localhost:3000" ];
    public const string InventoryApiRoute = "api/[controller]";
    public const string ConnectionStringKey = "MongoDbSettings:ConnectionString";

    public class SwaggerConfig
    {
        public const string Version = "v1";
        public const string Title = "Smart Inventory Scanner";
    }

    public class JwtConfig
    {
        public const string Description = "JWT Authorization header using the Bearer scheme";
        public const string SecurityScheme = "Authorization";

        public class Claims
        {
            public const string UserId = "UserId";
            public const string Role = "Role";
        }
    }

}
