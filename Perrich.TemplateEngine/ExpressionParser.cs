using System;
using System.Collections.Generic;
using System.Globalization;
using Perrich.TemplateEngine.Token;

namespace Perrich.TemplateEngine
{
    public static class ExpressionParser
    {
        private static readonly IList<char> IgnoredCharsList = new List<char> {' ', '\t', '\n', '\r'};

        /// <summary>
        ///     Converts the input infix expression into the postfix and creates the list
        ///     of token in that order
        /// </summary>
        /// <param name="expression">Infix expression (e.g. (A && C) || B)</param>
        /// <returns>List of atomic expressions sorted in the postfix order</returns>
        public static IEnumerable<ITypedToken> Parse(string expression)
        {
            var expressions = new List<ITypedToken>();
            string variable = string.Empty;
            var operatorStack = new Stack<string>();

            try
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    char c = expression[i];

                    if (IgnoredCharsList.Contains(c)) continue; // ignore some characters

                    // variable or number
                    if (IsVariableNameCharacter(c))
                    {
                        variable += c;
                    }
                    else
                    {
                        switch (c)
                        {
                            case '(':
                                operatorStack.Push(Constant.LeftParenthesis);
                                break;
                            case ')':
                                CheckVariable(variable, expressions);
                                variable = string.Empty;
                                while (!operatorStack.Peek().Equals(Constant.LeftParenthesis))
                                {
                                    expressions.Add(new OperatorToken {Operator = operatorStack.Pop()});
                                }
                                operatorStack.Pop();
                                break;
                            default:
                                string sc = c.ToString(CultureInfo.InvariantCulture);
                                CheckVariable(variable, expressions);
                                variable = string.Empty;
                                if (i < expression.Length - 1)
                                {
                                    char next = expression[i + 1];
                                    if (!IgnoredCharsList.Contains(c) && !IsVariableNameCharacter(next) && next != '(' && next != ')')
                                    {
                                        sc += next;
                                        i++;
                                    }
                                }
                                while (operatorStack.Count > 0 &&
                                       OperatorHelper.Priority(operatorStack.Peek()) >= OperatorHelper.Priority(sc))
                                {
                                    expressions.Add(new OperatorToken {Operator = operatorStack.Pop()});
                                }
                                operatorStack.Push(sc);
                                break;
                        }
                    }
                }

                CheckVariable(variable, expressions);
                while (operatorStack.Count > 0)
                {
                    expressions.Add(new OperatorToken {Operator = operatorStack.Pop()});
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Cannot parse the current expression '" + expression + "'!");
            }

            if (expressions.Count == 0)
            {
                throw new InvalidOperationException("Cannot evaluate an empty expression!");
            }

            return expressions;
        }

        private static void CheckVariable(string variable, List<ITypedToken> expressions)
        {
            if (variable == string.Empty) return;

            bool outb;
            if (bool.TryParse(variable, out outb))
            {
                expressions.Add(new ConstantToken {Value = outb});
            }
            else
            {
                expressions.Add(new VariableToken {Name = variable});
            }
        }

        private static bool IsVariableNameCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }
    }
}