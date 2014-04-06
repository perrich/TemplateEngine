using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Perrich.TemplateEngine
{
    /// <summary>
    /// Create a template engine wich can parse a text :
    /// - apply tag replacement
    /// - use boolean expression to show/hide parts of the text
    /// </summary>
    public class TemplateEngine
    {
        private readonly Dictionary<string, string> replacements;
        private readonly ExpressionEvaluator evaluator;
        private readonly Regex regex;
        private readonly string tagPrefix;
        private readonly string tagSuffix;

        /// <summary>
        /// Create a template engine
        /// </summary>
        /// <param name="evaluator">The boolean expression evaluator</param>
        /// <param name="needToEscapeBrace">Is braces are saved with a backslash in text to read</param>
        /// <param name="replacements">List of tag replacements</param>
        public TemplateEngine(ExpressionEvaluator evaluator, bool needToEscapeBrace, Dictionary<string, string> replacements)
        {
            this.replacements = replacements;
            this.evaluator = evaluator;

            if (needToEscapeBrace)
            {
                regex = new Regex(@"\\\{if=""(?<condition>[^""]+?)""\\\}(?<text>.*?)\\\{/if\\\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
                tagPrefix = @"\{tag=";
                tagSuffix = @"\}";
            }
            else
            {
                regex = new Regex(@"\{if=""(?<condition>[^""]+?)""\}(?<text>.*?)\{/if\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
                tagPrefix = @"{tag=";
                tagSuffix = @"}";
            }
        }

        /// <summary>
        /// Apply replacement and boolean condition to the provided text
        /// </summary>
        /// <param name="text">The text with template code</param>
        /// <returns>The result</returns>
        public string Apply(string text)
        {
            var result = ApplyConditions(text);

            foreach (var replacement in replacements)
            {
                result = result.Replace(tagPrefix + replacement.Key + tagSuffix, replacement.Value);
            }

            return result;
        }

        private string ApplyConditions(string text)
        {
            var matches = regex.Matches(text);
            var statementsList = new List<IfStatement>(matches.Count);

            foreach (Match match in matches)
            {
                var condition = match.Groups["condition"].Value;
                IfStatement statement = new IfStatement
                {
                    Start = match.Index,
                    End = match.Index + match.Length - 1,
                };
                if (evaluator.Evaluate(condition))
                {
                    statement.Text = match.Groups["text"].Value;
                }

                statementsList.Add(statement);
            }

            var pos = 0;
            var sb = new StringBuilder();
            foreach (var ifStatement in statementsList)
            {
                sb.Append(text.Substring(pos, ifStatement.Start - pos));
                if (ifStatement.Text != null)
                    sb.Append(ifStatement.Text);
                pos = ifStatement.End + 1;
            }

            if (pos < text.Length)
            {
                sb.Append(text.Substring(pos, text.Length - pos));
            }

            return sb.ToString();
        }
    }
}
