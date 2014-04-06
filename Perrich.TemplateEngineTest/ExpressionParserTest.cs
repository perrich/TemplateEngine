using System;
using System.Linq;
using Perrich.TemplateEngine;
using NUnit.Framework;
using Perrich.TemplateEngine.Token;

namespace Perrich.TemplateEngineTest
{
    public class ExpressionParserTest
    {
        [Test]
        public void ShouldRejectEmptyExpression()
        {
            Assert.Throws<InvalidOperationException>(() => ExpressionParser.Parse(""));
        }

        [Test]
        public void ShouldReturnPostfixExpression()
        {
            var res = ExpressionParser.Parse("((A || B) && C) || true").ToList();
            Assert.AreEqual(7, res.Count);
            Assert.IsInstanceOf<VariableToken>(res[0]);
            Assert.AreEqual("A", ((VariableToken)res[0]).Name);
            Assert.IsInstanceOf<VariableToken>(res[1]);
            Assert.AreEqual("B", ((VariableToken)res[1]).Name);
            Assert.IsInstanceOf<OperatorToken>(res[2]);
            Assert.AreEqual("||", ((OperatorToken)res[2]).Operator);
            Assert.IsInstanceOf<VariableToken>(res[3]);
            Assert.AreEqual("C", ((VariableToken)res[3]).Name);
            Assert.IsInstanceOf<OperatorToken>(res[4]);
            Assert.AreEqual("&&", ((OperatorToken)res[4]).Operator);
            Assert.IsInstanceOf<ConstantToken>(res[5]);
            Assert.True(((ConstantToken)res[5]).Value);
            Assert.IsInstanceOf<OperatorToken>(res[6]);
            Assert.AreEqual("||", ((OperatorToken)res[6]).Operator);
        }
    }
}