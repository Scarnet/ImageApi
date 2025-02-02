namespace ImageApi;

public static class StartupHelper
{
    private static ImageRepository repository = new();
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder.AllowAnyOrigin() // Only for testing
                       .WithMethods("GET", "POST")
                       .WithHeaders("Content-Type");
            });
        });
    }

    public static void ConfigurePipeline(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors("AllowSpecificOrigins");
        app.UseEndpoints(endpoints =>
        {
            CreateImagePostHandler(endpoints);
            CreateImageGetHandler(endpoints);
        });
    }

    private static void CreateImagePostHandler(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/image", (ImageEvent request) =>
        {
            if (string.IsNullOrWhiteSpace(request.ImageUrl) || string.IsNullOrWhiteSpace(request.Description))
                return Results.BadRequest("Invalid request");

            var eventRecord = new ImageEvent
            {
                ImageUrl = request.ImageUrl,
                Description = request.Description,
                Timestamp = DateTime.UtcNow
            };

            repository.Add(eventRecord);

            return Results.Ok(new { Message = "Image event stored!", eventRecord });
        });
    }

    private static void CreateImageGetHandler(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/events", () =>
        {
            var lastHourEventsCount = repository.GetLastHourEventCount();

            return Results.Ok(new
            {
                LastHourCount = lastHourEventsCount,
                LatestEvent = repository.GetLatestEvent(),
            });
        });
    }
}