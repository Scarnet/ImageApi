using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;

namespace ImageApi;
public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.ConfigureServices();
        });

        builder.Configure(app =>
        {
            app.UseRouting();
            app.ConfigurePipeline();
        });
    }

    protected override void MarshallRequest(InvokeFeatures features, APIGatewayHttpApiV2ProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
    {
        lambdaContext.Logger.LogLine("Received request from API Gateway.");

        if (apiGatewayRequest == null)
        {
            lambdaContext.Logger.LogLine("Error: API Gateway request is null!");
            throw new ArgumentNullException(nameof(apiGatewayRequest), "API Gateway request is null.");
        }

        base.MarshallRequest(features, apiGatewayRequest, lambdaContext);
    }
}