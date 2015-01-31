using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Perrich.TemplateEngine.Token;

namespace Perrich.TemplateEngine
{
    /// <summary>
    ///     Create a template engine wich can parse a text :
    ///     - apply tag replacement
    ///     - use boolean expression to show/hide parts of the text
    /// </summary>
    public class TemplateEngine
    {
        private readonly ExpressionEvaluator evaluator;
        private readonly bool ignoreCase;
        private readonly Regex regexIf;
        private readonly Regex regexReplacement;
        private readonly Dictionary<string, string> replacements;

        /// <summary>
        ///     Create a template engine
        /// </summary>
        /// <param name="evaluator">The boolean expression evaluator</param>
        /// <param name="needToEscapeBrace">Is braces are saved with a backslash in text to read</param>
        /// <param name="replacements">List of tag replacements</param>
        /// <param name="ignoreCase">Is key is case sensitive?</param>
        public TemplateEngine(ExpressionEvaluator evaluator, bool needToEscapeBrace, bool ignoreCase,
            Dictionary<string, string> replacements)
        {
            this.evaluator = evaluator;
            this.ignoreCase = ignoreCase;
            if (ignoreCase)
            {
                this.replacements = new Dictionary<string, string>();
                foreach (var var in replacements)
                {
                    this.replacements.Add(var.Key.ToUpper(), var.Value);
                }
            }
            else
            {
                this.replacements = replacements;
            }

            if (needToEscapeBrace)
            {
                regexIf = new Regex(@"\\\{if=""(?<condition>[^""]+?)""\\\}(?<text>.*?)\\\{/if\\\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
                regexReplacement = new Regex(@"\\\{tag=(?<tag>[^}]+?)\\\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            }
            else
            {
                regexIf = new Regex(@"\{if=""(?<condition>[^""]+?)""\}(?<text>.*?)\{/if\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
                regexReplacement = new Regex(@"\{tag=(?<tag>[^}]+?)\}",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            }
        }

        /// <summary>
        ///     Apply replacement and boolean condition to the provided text
        /// </summary>
        /// <param name="text">The text with template code</param>
        /// <returns>The result</returns>
        public string Apply(string text)
        {
            return ApplyReplacements(ApplyConditions(text));
        }

        /// <summary>
        ///     Retrieve all available tags or variable in the provided text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public HashSet<string> GetAvailableTags(string text)
        {
            var results = new HashSet<string>();
            FindReplacements(text, results);
            FindConditionVariables(text, results);

            return results;
        }

        private string ApplyConditions(string text)
        {
            MatchCollection matches = regexIf.Matches(text);
            var blocksList = new List<TextBlock>(matches.Count);

            foreach (Match match in matches)
            {
                string condition = match.Groups["condition"].Value;
                var block = new TextBlock
                {
                    Start = match.Index,
                    End = match.Index + match.Length - 1,
                };
                if (evaluator.Evaluate(condition))
                {
                    block.Text = match.Groups["text"].Value;
                }

                blocksList.Add(block);
            }

            return UpdateBlocks(text, blocksList);
        }
        
        private string ApplyReplacements(string text)
        {
            MatchCollection matches = regexReplacement.Matches(text);
            var blocksList = new List<TextBlock>(matches.Count);

            foreach (Match match in matches)
            {
                string tag = ignoreCase ? match.Groups["tag"].Value.ToUpper() : match.Groups["tag"].Value;
                string result;
                if (!replacements.TryGetValue(tag, out result))
                {
                    throw new InvalidOperationException("Trying to replace an unknown tag named '" +
                                                        tag + "', please check your tag dictionary!");
                }
                var block = new TextBlock
                {
                    Start = match.Index,
                    End = match.Index + match.Length - 1,
                    Text = result,
                };

                blocksList.Add(block);
            }

            return UpdateBlocks(text, blocksList);
        }

        private void FindConditionVariables(string text, HashSet<string> results)
        {
            foreach (Match match in regexIf.Matches(text))
            {
                foreach (var exp in ExpressionParser.Parse(match.Groups["condition"].Value))
                {
                    if (exp.TokenType != TokenType.Variable)
                        continue;
                    results.Add(ignoreCase ? ((VariableToken)exp).Name.ToUpper() : ((VariableToken)exp).Name);
                }
            }
        }

        private void FindReplacements(string text, HashSet<string> results)
        {
            foreach (Match match in regexReplacement.Matches(text))
            {
                results.Add(ignoreCase ? match.Groups["tag"].Value.ToUpper() : match.Groups["tag"].Value);
            }
        }

        private static string UpdateBlocks(string text, IEnumerable<TextBlock> blocksList)
        {
            int pos = 0;
            var sb = new StringBuilder();
            foreach (TextBlock block in blocksList)
            {
                sb.Append(text.Substring(pos, block.Start - pos));
                if (block.Text != null)
                    sb.Append(block.Text);
                pos = block.End + 1;
            }

            if (pos < text.Length)
            {
                sb.Append(text.Substring(pos, text.Length - pos));
            }

            return sb.ToString();
        }
    }
}