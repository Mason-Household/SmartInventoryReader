using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Inventory.Services;

namespace Inventory.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        // Create a mock CurrentUserService for design-time
        var mockCurrentUserService = new MockCurrentUserService();

        return new AppDbContext(optionsBuilder.Options, mockCurrentUserService);
    }
}

// Mock service for design-time factory
public class MockCurrentUserService : ICurrentUserService
{
    public Guid? GetCurrentUserId() => null;
    public long? GetCurrentOrganizationId() => null;
    public string? GetUserEmail() => null;
}
