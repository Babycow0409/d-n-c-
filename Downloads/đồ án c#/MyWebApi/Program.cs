using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Services;
using MyWebApi.DTOs;

var builder = WebApplication.CreateBuilder(args);

// ===== CẤU HÌNH DỊCH VỤ =====

// 1. Cấu hình DbContext với SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=MultilingualExplanationDb.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
);

// 2. Đăng ký các Services
builder.Services.AddScoped<IExplanationService, ExplanationService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IMobilePoiService, MobilePoiService>();

// 3. Thêm Controllers
builder.Services.AddControllers();

// 4. Cấu hình Swagger/OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. CORS - cho phép tất cả origins (có thể cấu hình chặt hơn sau)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 6. Cấu hình Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// ===== CẤU HÌNH HTTP PIPELINE =====

// 1. Áp dụng migrations tự động (nếu cần)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Tạo database nếu chưa tồn tại
    await dbContext.Database.EnsureCreatedAsync();
    Console.WriteLine("✅ Database đã được tạo/kết nối");
}

// 2. Cấu hình middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1");
        c.RoutePrefix = string.Empty; // Swagger tại /
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// 3. Map Controllers
app.MapControllers();

// 4. API default route
app.MapGet("/health", () => Results.Ok(new { status = "API is running ✅", timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi();

app.Run();

