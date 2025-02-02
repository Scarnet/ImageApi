using System.Text;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ImageApi
{
    public class KinesisEventProcessor
    {
        private ImageRepository repository = new();
        public async Task FunctionHandlerAsync(KinesisEvent kinesisEvent, ILambdaContext context)
        {
            foreach (var record in kinesisEvent.Records)
            {
                var dataJson = Encoding.UTF8.GetString(record.Kinesis.Data.ToArray());
                context.Logger.LogLine($"Processing Kinesis event: {dataJson}");

                ProcessEvent(dataJson);
            }
        }

        private async void ProcessEvent(string jsonData)
        {
            try
            {
                var eventData = JsonConvert.DeserializeObject<ImageEvent>(jsonData);
                if (eventData != null)
                {
                    Console.WriteLine($"Processing event: {eventData.ImageUrl} - {eventData.Description}");
                    repository.Add(eventData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing event: {ex.Message}");
            }
        }
    }
}