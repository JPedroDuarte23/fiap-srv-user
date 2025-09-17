using System.Diagnostics.CodeAnalysis;

namespace FiapSrvUser.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class MongoMappings
{
    public static void ConfigureMappings() 
    {
        UserMapping.Configure();
    }
}