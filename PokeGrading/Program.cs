using PokeGrading.Utilities;
using PokeGrading.Repositories;
using PokeGrading.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

const string ReactCorsOriginsSection = "Cors:AllowedOrigins";

string[] allowedOrigins =
    builder.Configuration
        .GetSection(ReactCorsOriginsSection)
        .Get<string[]>()
    ??
    Array.Empty<string>();


// Controllers
builder.Services.AddControllers();


// Database
builder.Services.AddScoped<SQL_connection>();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<ApiKeyValidationService>();
builder.Services.AddScoped<CatalogCoverageService>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardValidationService, CardValidationService>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
builder.Services.AddScoped<IAuditService, AuditService>();


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReactFrontend",
        policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

                return;
            }

            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// HTTPS
app.UseHttpsRedirection();


// CORS
app.UseCors("AllowReactFrontend");


// Images Folder
var imagesPath =
    Path.Combine(
        builder.Environment.ContentRootPath,
        "Images"
    );

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}


// Static Files
app.UseStaticFiles();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                imagesPath
            ),

        RequestPath = "/images"
    });


// Auth
app.UseAuthorization();


// Controllers
app.MapControllers();

app.Run();