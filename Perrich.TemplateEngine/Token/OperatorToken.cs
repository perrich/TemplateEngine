namespace Perrich.TemplateEngine.Token
{
    /// <summary>
    /// an Unary or Binary operator token
    /// </summary>
    public class OperatorToken : ITypedToken
    {
        /// <summary>
        /// The operator
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Is it an unary operator?
        /// </summary>
        public bool IsUnary
        {
            get { return Constant.NotOperator == Operator; }
        }

        public TokenType TokenType
        {
            get { return TokenType.Operator; }
        }
    }
}