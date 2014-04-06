using System;

namespace Perrich.TemplateEngine
{
    /// <summary>
    ///     Provide way to evaluate operators
    /// </summary>
    public static class OperatorHelper
    {
        /// <summary>
        ///     Evaluates the unary operator with provided value
        /// </summary>
        /// <param name="op">The unary operator</param>
        /// <param name="value">Value for unary operator</param>
        /// <returns>Output value of the unary operator</returns>
        public static bool EvaluateUnary(string op, bool value)
        {
            if (op == Constant.NotOperator)
            {
                return !value;
            }
            throw new NotImplementedException(op + " operator not implemented");
        }

        /// <summary>
        ///     Evaluates the binary operator expression with provided values
        /// </summary>
        /// <param name="op">The binary operator</param>
        /// <param name="val1">The left-side value for the operator</param>
        /// <param name="val2">The right-side value for the operator</param>
        /// <returns>Output value of the binary expression</returns>
        /// <returns></returns>
        public static bool EvaluateBinary(string op, bool val1, bool val2)
        {
            switch (op)
            {
                case Constant.BinaryAndOperator:
                    return val1 && val2;
                case Constant.BinaryOrOperator:
                    return val1 || val2;
            }
            throw new NotImplementedException(op + " operator not implemented");
        }

        /// <summary>
        /// Gave the priority of the operator
        /// </summary>
        /// <param name="op">The operator</param>
        /// <returns>Priority</returns>
        public static int Priority(string op)
        {
            switch (op)
            {
                case Constant.LeftParenthesis:
                case Constant.RightParenthesis:
                    return -1;
                case Constant.BinaryOrOperator:
                    return 0;
                case Constant.BinaryAndOperator:
                    return 1;
                case Constant.NotOperator:
                    return 2;
                default:
                    throw new NotImplementedException(op + " is not implemented");
            }
        }
    }
}