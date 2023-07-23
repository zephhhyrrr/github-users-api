using GitHubUsers.Service;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// add config got the httpclient and register the service for dependency injection
builder.Services.AddHttpClient<IGitHubService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
