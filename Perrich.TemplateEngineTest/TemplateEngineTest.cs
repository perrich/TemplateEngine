using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Perrich.TemplateEngine;

namespace Perrich.TemplateEngineTest
{
    public class TemplateEngineTest
    {
        private const string RtfText = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1036{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 6.3.9600}\viewkind4\uc1 
\pard\sa200\sl276\slmult1\b\f0\fs24\lang12 Sample of a RTF file.\par
\b0\fs22 It can contain some paragraph. With a lot of text or not. Everything is possible.\{if=""visible""\} This part should be visible.\{/if\}\par
\b We've added \{tag=value\}. \par
}
 
";

        private const string Text =
            @"It's a sample with {if=""Displayed""}a displayed {tag=value}.{/if}{if=""!Displayed""}nothing.{/if}";

        [Test]
        public void ShouldApplyTemplateOnRtf()
        {
            var dict = new Dictionary<string, bool> { { "visible", true } };
            var replacements = new Dictionary<string, string> { { "value", "some extra text" } };
            var evaluator = new ExpressionEvaluator(dict, false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, true, false, replacements);

            var result = engine.Apply(RtfText);
            Assert.AreEqual(@"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1036{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 6.3.9600}\viewkind4\uc1 
\pard\sa200\sl276\slmult1\b\f0\fs24\lang12 Sample of a RTF file.\par
\b0\fs22 It can contain some paragraph. With a lot of text or not. Everything is possible. This part should be visible.\par
\b We've added some extra text. \par
}
 
", result);
        }

        [Test]
        public void ShouldApplyTemplateOnText()
        {
            var dict = new Dictionary<string, bool> { { "Displayed", true } };
            var replacements = new Dictionary<string, string> { { "value", "text" } };
            var evaluator = new ExpressionEvaluator(dict, false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, false, replacements);

            var result = engine.Apply(Text);
            Assert.AreEqual("It's a sample with a displayed text.", result);
        }

        [Test]
        public void ShouldAllowDictionnaryIgnoringCase()
        {
            var dict = new Dictionary<string, bool> { { "displaYED", true } };
            var replacements = new Dictionary<string, string> { { "VALue", "text" } };
            var evaluator = new ExpressionEvaluator(dict, true);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, true, replacements);

            var result = engine.Apply(Text);
            Assert.AreEqual("It's a sample with a displayed text.", result);
        }

        [Test]
        public void ShouldThrowExceptionIfReplacementDoesNotExists()
        {
            var dict = new Dictionary<string, bool> { { "Displayed", true } };
            var replacements = new Dictionary<string, string>();
            var evaluator = new ExpressionEvaluator(dict, true);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, false, replacements);

            Assert.Throws<InvalidOperationException>(() => engine.Apply(Text));
        }

        [Test]
        public void ShouldRetrieveAllTags()
        {
            var dict = new Dictionary<string, bool>();
            var replacements = new Dictionary<string, string>();
            var evaluator = new ExpressionEvaluator(dict, false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, false, replacements);

            var result = engine.GetAvailableTags(Text).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.Contains("Displayed", result);
            Assert.Contains("value", result);
        }

        [Test]
        public void ShouldRetrieveAllTagsIgnoringCase()
        {
            var dict = new Dictionary<string, bool>();
            var replacements = new Dictionary<string, string>();
            var evaluator = new ExpressionEvaluator(dict, true);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, true, replacements);

            var result = engine.GetAvailableTags(Text).ToList();
            Assert.AreEqual(2, result.Count);
            Assert.Contains("DISPLAYED", result);
            Assert.Contains("VALUE", result);
        }

        [Test]
        public void ShouldDoNotConvertLineSeparatorOnText()
        {
            const string replacement = "text\nwith\r\nnew lines";
            var replacements = new Dictionary<string, string> { { "value", replacement } };
            var evaluator = new ExpressionEvaluator(new Dictionary<string, bool>(), false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, false, false, replacements);

            var result = engine.Apply("Sample with {tag=value}.");
            Assert.AreEqual("Sample with " + replacement + ".", result);
        }

        [Test]
        public void ShouldConvertLineSeparatorForTextOnRtf()
        {
            const string replacement = "text\nwith\r\nnew lines";
            var replacements = new Dictionary<string, string> { { "value", replacement } };
            var evaluator = new ExpressionEvaluator(new Dictionary<string, bool>(), false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, true, false, replacements);

            var result = engine.Apply("{\\rtf1\\ansi Sample with \\{tag=value\\}.}");
            Assert.AreEqual("{\\rtf1\\ansi Sample with text\\line\nwith\\line\nnew lines.}", result);
        }

        [Test]
        public void ShouldNotConvertLineSeparatorForRtfOnRtf()
        {
            const string replacement = "{\\rtf1\\ansi text\nwith\r\nnew lines }";
            var replacements = new Dictionary<string, string> { { "value", replacement } };
            var evaluator = new ExpressionEvaluator(new Dictionary<string, bool>(), false);

            var engine = new TemplateEngine.TemplateEngine(evaluator, true, false, replacements);

            var result = engine.Apply("\\{tag=value\\}");
            Assert.AreEqual(replacement, result);
        }
    }
}