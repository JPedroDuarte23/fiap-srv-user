﻿
using System.Diagnostics.CodeAnalysis;

namespace FiapSrvUser.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
