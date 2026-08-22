using Microsoft.AspNetCore.Cors.Infrastructure;
using financeiroApi.Code.Connection;
using financeiroApi.Code.Business.Debt;
using financeiroApi.Code.Business.Income;
using financeiroApi.Code.Business.Import;
using financeiroApi.Code.Business.Dashboard;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddScoped<MySqlAccess>();

var corsPolicy = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.AllowAnyOrigin()  // Permite qualquer origem
                  .AllowAnyMethod()   // Permite qualquer m�todo (GET, POST, etc.)
                  .AllowAnyHeader();  // Permite qualquer cabe�alho
        });
});



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DebtDAL>();
builder.Services.AddScoped<DebtBLL>();
builder.Services.AddScoped<IncomeDAL>();
builder.Services.AddScoped<IncomeBLL>();
builder.Services.AddScoped<ImportBLL>();
builder.Services.AddScoped<DashboardDAL>();
builder.Services.AddScoped<DashboardBLL>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
