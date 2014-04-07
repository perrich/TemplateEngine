namespace Perrich.TemplateEngine
{
    /// <summary>
    ///     Define a text block (used by replace behavior)
    /// </summary>
    public class TextBlock
    {
        /// <summary>
        ///     Start of the text block
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        ///     End of the text block
        /// </summary>
        public int End { get; set; }

        /// <summary>
        ///     Text to be used instead of previous
        /// </summary>
        public string Text { get; set; }
    }
}