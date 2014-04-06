using System;
using System.Collections.Generic;
using Perrich.TemplateEngine;
using NUnit.Framework;

namespace Perrich.TemplateEngineTest
{
    public class ExpressionEvaluatorTest
    {
        [Test]
        public void ShouldEvaluateExpression()
        {
            var dict = new Dictionary<string, bool> {{"A", true}, {"B", false}, {"another", true}};
            AssertEquals(true, "A || B && !another", dict);
            AssertEquals(false, "A && B || !another", dict);
        }

        [Test]
        public void ShouldApplyOperatorPriorities()
        {
            var dict = new Dictionary<string, bool> { { "A", true }, { "B", false } };
            AssertEquals(true, "A || B && B", dict);
        }

        [Test]
        public void ShouldApplyParenthesisPriorities()
        {
            var dict = new Dictionary<string, bool> { { "A", true }, { "B", false } };
            AssertEquals(false, "(A || B) && B", dict);
        }

        [Test]
        public void ShouldManageConstant()
        {
            var dict = new Dictionary<string, bool> { { "A", true }, { "B", false } };
            AssertEquals(false, "(false)", dict);
            AssertEquals(true, "A && (true || B)", dict);
        }

        [Test]
        public void ShouldRejectExpressionWithMissingVariable()
        {
            Assert.Throws<InvalidOperationException>(() => AssertEquals(true, "A", new Dictionary<string, bool>()));
        }

        [Test]
        public void ShouldRejectInvalidExpression()
        {
            var dict = new Dictionary<string, bool> { { "A", true }, { "B", false } };
            Assert.Throws<InvalidOperationException>(() => AssertEquals(true, "(A || ) B", dict));
            Assert.Throws<InvalidOperationException>(() => AssertEquals(true, "(A ", dict));
        }

        [Test]
        public void ShouldRejectExpressionWithNotImplementedOperator()
        {
            var dict = new Dictionary<string, bool> { { "A", true }, { "B", false } };
            Assert.Throws<NotImplementedException>(() => AssertEquals(true, "A + B", dict));
        }

        private void AssertEquals(bool expected, string str, Dictionary<string, bool> dict)
        {
            var s = new ExpressionEvaluator(dict);
            Assert.AreEqual(expected, s.Evaluate(str));
        }
    }
}