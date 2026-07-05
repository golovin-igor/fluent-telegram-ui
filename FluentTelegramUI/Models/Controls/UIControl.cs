namespace FluentTelegramUI.Models
{
    /// <summary>
    /// Base class for UI controls in the Fluent Telegram UI
    /// </summary>
    public abstract class UIControl
    {
        /// <summary>
        /// Unique identifier for the control
        /// </summary>
        public string Id { get; set; } = IdGenerator.GenerateShortId();

        /// <summary>
        /// The style of the control
        /// </summary>
        public FluentStyle Style { get; set; } = FluentStyle.Default;

        /// <summary>
        /// Converts the control to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public virtual Message ToMessage(ScreenRenderContext context) => ToMessage();

        /// <summary>
        /// Converts the control to a Message object for rendering
        /// </summary>
        /// <returns>A Message object</returns>
        public abstract Message ToMessage();
    }

    /// <summary>
    /// Represents text alignment options
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>Left alignment</summary>
        Left,

        /// <summary>Center alignment</summary>
        Center,

        /// <summary>Right alignment</summary>
        Right
    }
}
