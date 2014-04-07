using System;
using System.Collections.Generic;
using Perrich.TemplateEngine.Token;

namespace Perrich.TemplateEngine
{
    public class ExpressionEvaluator
    {
        private readonly bool ignoreCase;
        private readonly Dictionary<string, bool> variables;

        /// <summary>
        ///     Create the expression evaluator
        /// </summary>
        /// <param name="variables">List of boolean variables and their values used during the evaluation</param>
        /// <param name="ignoreCase">Is key is case sensitive?</param>
        public ExpressionEvaluator(Dictionary<string, bool> variables, bool ignoreCase)
        {
            this.ignoreCase = ignoreCase;

            if (ignoreCase)
            {
                this.variables = new Dictionary<string, bool>();
                foreach (var var in variables)
                {
                    this.variables.Add(var.Key.ToUpper(), var.Value);
                }
            }
            else
            {
                this.variables = variables;
            }
        }

        /// <summary>
        ///     Evaluate the expression using provided variables
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns></returns>
        public bool Evaluate(string expression)
        {
            var stack = new Stack<bool>();
            bool result = false;

            foreach (ITypedToken exp in ExpressionParser.Parse(expression))
            {
                switch (exp.TokenType)
                {
                    case TokenType.Value:
                        result = ((ConstantToken) exp).Value;
                        break;
                    case TokenType.Variable:
                        var var = (VariableToken) exp;
                        if (!variables.TryGetValue((ignoreCase ? var.Name.ToUpper() : var.Name), out result))
                        {
                            throw new InvalidOperationException("Trying to evaluate an unknown variable named '" +
                                                                var.Name + "', please check your variable dictionary!");
                        }
                        break;
                    case TokenType.Operator:
                        var op = (OperatorToken) exp;
                        if (op.IsUnary)
                        {
                            result = OperatorHelper.EvaluateUnary(op.Operator, stack.Pop());
                        }
                        else // binary operator
                        {
                            bool p2 = stack.Pop();
                            bool p1 = stack.Pop();
                            result = OperatorHelper.EvaluateBinary(op.Operator, p1, p2);
                        }
                        break;
                }
                stack.Push(result);
            }

            return stack.Pop();
        }
    }
}