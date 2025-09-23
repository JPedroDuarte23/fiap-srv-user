﻿using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using AspNetCore.DataProtection.Aws.S3;
using FiapCloudGames.Infrastructure.Configuration;
using FiapSrvUser.Application.Interfaces;
using FiapSrvUser.Application.Interfaces.Repositories;
using FiapSrvUser.Application.Services;
using FiapSrvUser.Infrastructure.Configuration;
using FiapSrvUser.Infrastructure.Mappings;
using FiapSrvUser.Infrastructure.Middleware;
using FiapSrvUser.Infrastructure.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();
builder.Host.UseSerilog();

// 1. Configuração da AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();
builder.Services.AddAWSService<Amazon.S3.IAmazonS3>(); 

string mongoConnectionString;
string jwtSigningKey;
string databaseName = builder.Configuration["MongoDbSettings:DatabaseName"]!;

if (!builder.Environment.IsDevelopment())
{
    Log.Information("Ambiente de Produção. Buscando segredos do AWS Parameter Store.");
    var ssmClient = new AmazonSimpleSystemsManagementClient();

    // Busca a Connection String do MongoDB
    var mongoParameterName = builder.Configuration["ParameterStore:MongoConnectionString"];
    var mongoResponse = await ssmClient.GetParameterAsync(new GetParameterRequest
    {
        Name = mongoParameterName,
        WithDecryption = true
    });
    mongoConnectionString = mongoResponse.Parameter.Value;

    // Busca a Chave de Assinatura do JWT
    var jwtParameterName = builder.Configuration["ParameterStore:JwtSigningKey"];
    var jwtResponse = await ssmClient.GetParameterAsync(new GetParameterRequest
    {
        Name = jwtParameterName,
        WithDecryption = true
    });
    jwtSigningKey = jwtResponse.Parameter.Value;

    // 2. Configuração do Data Protection com AWS S3
    var s3Bucket = builder.Configuration["DataProtection:S3BucketName"];
    var s3KeyPrefix = builder.Configuration["DataProtection:S3KeyPrefix"];
    var s3DataProtectionConfig = new S3XmlRepositoryConfig(s3Bucket) { KeyPrefix = s3KeyPrefix };

    builder.Services.AddDataProtection()
        .SetApplicationName("FiapSrvUser")
        .PersistKeysToAwsS3(s3DataProtectionConfig);
}
else
{
    Log.Information("Ambiente de Desenvolvimento. Usando appsettings.json.");
    mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection")!;
    jwtSigningKey = builder.Configuration["Jwt:DevKey"]!;
}

// 3. Configuração do MongoDB e Repositórios
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
MongoMappings.ConfigureMappings();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


// 4. Configuração de Autenticação e Autorização
builder.Services.ConfigureJwtBearer(builder.Configuration, jwtSigningKey);
builder.Services.AddAuthorization();

// -- Resto da configuração (Controllers, Swagger, etc.) --
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "FIAP Cloud Games - User API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
   {
       {
           new OpenApiSecurityScheme
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
           },
           Array.Empty<string>()
       }
   });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();