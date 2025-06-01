using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Phone_api;
using Phone_api.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(m => m.Value != null && m.Value.Errors.Any())
            .ToDictionary(
                m => m.Key,
                m => m.Value!.Errors.Select(e => e.ErrorMessage ?? "Lỗi không xác định").ToArray()
            );
        return new BadRequestObjectResult(new { errors });
    };
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to include XML comments and default responses
builder.Services.AddSwaggerGen(options =>
{
    // Đường dẫn tới file XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Kiểm tra file XML tồn tại
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    else
    {
        Console.WriteLine($"Warning: XML documentation file not found at {xmlPath}");
    }

    // Thêm thông tin API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Phone API",
        Version = "v1",
        Description = "API quản lý thông tin điện thoại."
    });

    // Thêm DefaultResponsesOperationFilter để tự động thêm mô tả response code
    options.OperationFilter<DefaultResponsesOperationFilter>();

    // Thêm CustomOperationFilter để tự động thêm mô tả cho tham số 'id'
    options.OperationFilter<IdParameterOperationFilter>();
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Phone API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
