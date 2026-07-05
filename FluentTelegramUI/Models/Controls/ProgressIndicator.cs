using System;

namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Represents a progress indicator control
    /// </summary>
    public class ProgressIndicator : UIControl
    {
        /// <summary>
        /// The label for the progress indicator
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// The current progress value (0-100)
        /// </summary>
        public int Progress { get; set; } = 0;

        /// <summary>
        /// Whether to show the percentage text
        /// </summary>
        public bool ShowPercentage { get; set; } = true;

        /// <summary>
        /// The character to use for filled progress
        /// </summary>
        public string FilledChar { get; set; } = "█";

        /// <summary>
        /// The character to use for empty progress
        /// </summary>
        public string EmptyChar { get; set; } = "░";

        /// <summary>
        /// The total width of the progress bar
        /// </summary>
        public int Width { get; set; } = 10;

        /// <summary>
        /// Creates a new progress indicator
        /// </summary>
        /// <param name="label">The progress label</param>
        /// <param name="progress">The initial progress value (0-100)</param>
        public ProgressIndicator(string label, int progress = 0)
        {
            Label = label;
            Progress = Math.Clamp(progress, 0, 100);
        }

        /// <summary>
        /// Converts the progress indicator to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public override Message ToMessage()
        {
            int filledCount = (int)Math.Round(Progress / 100.0 * Width);
            string progressBar = string.Concat(
                string.Concat(new string(FilledChar[0], filledCount)),
                string.Concat(new string(EmptyChar[0], Width - filledCount))
            );

            string percentageText = ShowPercentage ? $" {Progress}%" : string.Empty;

            return new Message
            {
                Text = $"{Label}: {progressBar}{percentageText}",
                Style = Style
            };
        }
    }
}
