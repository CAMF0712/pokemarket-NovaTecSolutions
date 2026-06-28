using PokeGrading.Utilities;
using PokeGrading.Repositories;
using PokeGrading.Services;
using Microsoft.Extensions.FileProviders;
using PokeGrading.Services.Application;
using PokeGrading.Services.ImagePreprocessing;
using PokeGrading.Services.FeatureExtraction;
using PokeGrading.Services.Grading;

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


// Repositories
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


// Services
builder.Services.AddScoped<ICardValidationService, CardValidationService>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddScoped<IOcrParsingService, OcrParsingService>();
builder.Services.AddScoped<ISearchScoringService, SearchScoringService>();
builder.Services.AddScoped<IGradingPersistenceService, GradingPersistenceService>();

// Grading
builder.Services.AddScoped<
    IGradingApplicationService,
    GradingApplicationService>();

builder.Services.AddScoped<
    IImagePreprocessingService,
    ImagePreprocessingService>();

builder.Services.AddScoped<
    IFeatureExtractionService,
    FeatureExtractionService>();

builder.Services.AddScoped<
    IGradingEngine,
    GradingEngine>();


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

var gradingImagesPath =
    Path.Combine(
        builder.Environment.ContentRootPath,
        "GradingImages");

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

if (!Directory.Exists(gradingImagesPath))
{
    Directory.CreateDirectory(
        gradingImagesPath);
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

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                gradingImagesPath),

        RequestPath = "/grading-images"
    });


// Auth
app.UseAuthorization();


// Controllers
app.MapControllers();

app.Run();
