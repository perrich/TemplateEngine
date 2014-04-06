namespace Perrich.TemplateEngine.Token
{
    /// <summary>
    /// A boolean value token
    /// </summary>
    public class ConstantToken : ITypedToken
    {
        /// <summary>
        /// The value
        /// </summary>
        public bool Value { get; set; }

        public TokenType TokenType
        {
            get { return TokenType.Value; }
        }
    }
}