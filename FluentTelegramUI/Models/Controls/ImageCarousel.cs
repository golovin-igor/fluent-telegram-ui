using System;
using System.Collections.Generic;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents an image carousel control
    /// </summary>
    public class ImageCarousel : UIControl
    {
        /// <summary>
        /// The list of image URLs
        /// </summary>
        public List<string> ImageUrls { get; set; } = new();

        /// <summary>
        /// The current image index
        /// </summary>
        public int CurrentIndex { get; set; } = 0;

        /// <summary>
        /// The captions for the images
        /// </summary>
        public List<string> Captions { get; set; } = new();

        /// <summary>
        /// Creates a new image carousel
        /// </summary>
        /// <param name="imageUrls">The list of image URLs</param>
        /// <param name="captions">The captions for the images</param>
        public ImageCarousel(List<string> imageUrls, List<string>? captions = null)
        {
            ImageUrls = imageUrls;

            if (captions != null)
            {
                Captions = captions;
                // Make sure we have captions for all images
                while (Captions.Count < ImageUrls.Count)
                {
                    Captions.Add(string.Empty);
                }
            }
            else
            {
                // No captions provided, initialize with empty strings
                Captions = new List<string>();
                for (int i = 0; i < ImageUrls.Count; i++)
                {
                    Captions.Add(string.Empty);
                }
            }
        }

        /// <summary>
        /// Converts the image carousel to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            return ToMessage(new ScreenRenderContext(0, new Screen(), (_, defaultValue) => defaultValue!));
        }

        public override Message ToMessage(ScreenRenderContext context)
        {
            int currentIndex = context.GetControlState(this, "index", CurrentIndex);
            currentIndex = Math.Clamp(currentIndex, 0, Math.Max(ImageUrls.Count - 1, 0));

            var message = new Message
            {
                Text = Captions[currentIndex],
                ImageUrl = ImageUrls[currentIndex],
                Style = Style,
                Buttons = new()
            };

            if (ImageUrls.Count > 1)
            {
                if (currentIndex > 0)
                {
                    message.Buttons.Add(new Button { Text = "◀️ Prev", CallbackData = $"{CallbackPrefixes.Carousel}{Id}:{CarouselActions.Previous}" });
                }

                message.Buttons.Add(new Button { Text = $"{currentIndex + 1}/{ImageUrls.Count}", CallbackData = $"{CallbackPrefixes.Carousel}{Id}:{CarouselActions.Info}" });

                if (currentIndex < ImageUrls.Count - 1)
                {
                    message.Buttons.Add(new Button { Text = "Next ▶️", CallbackData = $"{CallbackPrefixes.Carousel}{Id}:{CarouselActions.Next}" });
                }

                message.ButtonsPerRow = 3;
            }

            return message;
        }
    }
}
