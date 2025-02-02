namespace ImageApi
{
    public class ImageRepository
    {
        private static List<ImageEvent> images;

        public ImageRepository()
        {
            if (images == null)
                images = new ();
        }

        public ImageEvent Add(ImageEvent imageEvent)
        {
            images.Add(imageEvent);

            if (images.Count > 100)
            {
                images.RemoveAt(0);
            }

            return imageEvent;
        }

        public ImageEvent GetLatestEvent()
        {
            return images.LastOrDefault();
        }

        public int GetLastHourEventCount()
        {
            var lastHourEventsCount = images
                .Where(e => e.Timestamp > DateTime.UtcNow.AddHours(-1))
                .Count();

            return lastHourEventsCount;
        }
    }
}
