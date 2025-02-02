using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace ImageApi;
public class LambdaEntryPoint : AbstractAspNetCoreFunction
{
    private IWebHost webHost;
    public LambdaEntryPoint()
    {
        this.webHost = new WebHostBuilder()
            .UseKestrel()
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.ConfigurePipeline();
            })
            .Build();
    }

    public async Task FunctionHandlerAsync(Stream inputStream, ILambdaContext context)
    {
        var input = await new StreamReader(inputStream).ReadToEndAsync();

        if (input.Contains("requestContext"))
        {
            context.Logger.LogLine("Processing API Gateway Event.");
            var apiEvent = JsonConvert.DeserializeObject<APIGatewayHttpApiV2ProxyRequest>(input);
            var response = await ProcessApiGatewayRequest(apiEvent, context);
            return;
        }
    }

    /// <summary>
    /// Manually processes API Gateway requests through ASP.NET Core.
    /// </summary>
    private async Task<APIGatewayHttpApiV2ProxyResponse> ProcessApiGatewayRequest(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var lambdaHandler = serviceProvider.GetRequiredService<APIGatewayHttpApiV2ProxyFunction>();
            return await lambdaHandler.FunctionHandlerAsync(request, context);
        }
    }
}