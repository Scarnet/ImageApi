namespace ImageApi;

public static class StartupHelper
{
    private static List<ImageEvent> imageEvents = new();

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

            imageEvents.Add(eventRecord);

            if (imageEvents.Count > 100)
            {
                imageEvents.RemoveAt(0);
            }

            return Results.Ok(new { Message = "Image event stored!", eventRecord });
        });
    }

    private static void CreateImageGetHandler(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/events", () =>
        {
            var lastHourEventsCount = imageEvents
                .Where(e => e.Timestamp > DateTime.UtcNow.AddHours(-1))
                .Count();

            return Results.Ok(new
            {
                LastHourCount = lastHourEventsCount,
                LatestEvent = imageEvents.LastOrDefault()
            });
        });
    }
}