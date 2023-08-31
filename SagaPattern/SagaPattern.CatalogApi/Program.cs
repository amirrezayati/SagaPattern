using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using SagaPattern.Choreography.CatalogApi;
using SagaPattern.Choreography.CatalogApi.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
var sqlConnectionString = configuration.GetSection("ConnectionStrings");

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
});

// Configure Sqlite
builder.Services.AddDbContext<CatalogContext>(options => options.UseSqlServer(sqlConnectionString["DefaultConnection"]));

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));
builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
    "catalog_exchange",
    ExchangeType.Topic));

builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(s.GetService<IConnectionProvider>(),
    "order_exchange",
    "order_response_queue",
    "order_created_routingkey",
    ExchangeType.Topic
));

builder.Services.AddHostedService<OrderCreatedListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
