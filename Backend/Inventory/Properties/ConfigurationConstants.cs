namespace Inventory.Properties;

public class ConfigurationConstants
{
    public const string JwtKey = "Jwt:Key";
    public const string HealthCheck = "/health";
    public const string DatabaseName = "mongodb";
    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtAudience = "Jwt:Audience";
    public const string JwtExpiryIn = "Jwt:ExpiryIn";
    public const string JwtRefreshToken = "Jwt:Refresh";
    public const string AuthenticationScheme = "Bearer";
    public const string MongoDbSettings = "MongoDbSettings";
    public const string ConnectionStringKey = "MongoDbSettings:ConnectionString";

    public class SwaggerConfig
    {
        public const string Version = "v1";
        public const string Title = "Smart Inventory Scanner";
    }
}