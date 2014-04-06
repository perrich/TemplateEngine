namespace Perrich.TemplateEngine.Token
{
    /// <summary>
    /// A typed token retrieved by the parser
    /// </summary>
    public interface ITypedToken
    {
        /// <summary>
        /// Type of the token
        /// </summary>
        TokenType TokenType { get; }
    }
}