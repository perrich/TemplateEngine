namespace Perrich.TemplateEngine.Token
{
    /// <summary>
    /// A named variable token
    /// </summary>
    public class VariableToken : ITypedToken
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; set; }

        public TokenType TokenType
        {
            get { return TokenType.Variable; }
        }
    }
}