using System;
using Perrich.TemplateEngine;
using NUnit.Framework;

namespace Perrich.TemplateEngineTest
{
    public class OperatorHelperTest
    {
        [Test]
        public void ShouldManageTheNotUnaryOperator()
        {
            Assert.True(OperatorHelper.EvaluateUnary("!", false));
            Assert.False(OperatorHelper.EvaluateUnary("!", true));
        }

        [Test]
        public void ShouldRejectNotImplementedUnaryOperator()
        {
            Assert.Throws<NotImplementedException>(() => OperatorHelper.EvaluateUnary("&", false));
        }

        [Test]
        public void ShouldManageBinaryAndOperator()
        {
            Assert.True(OperatorHelper.EvaluateBinary("&&", true, true));
            Assert.False(OperatorHelper.EvaluateBinary("&&", true, false));
            Assert.False(OperatorHelper.EvaluateBinary("&&", false, true));
            Assert.False(OperatorHelper.EvaluateBinary("&&", false, false));
        }

        [Test]
        public void ShouldManageBinaryOrOperator()
        {
            Assert.True(OperatorHelper.EvaluateBinary("||", true, true));
            Assert.True(OperatorHelper.EvaluateBinary("||", true, false));
            Assert.True(OperatorHelper.EvaluateBinary("||", false, true));
            Assert.False(OperatorHelper.EvaluateBinary("||", false, false));
        }

        [Test]
        public void ShouldRejectNotImplementedBinaryOperator()
        {
            Assert.Throws<NotImplementedException>(() => OperatorHelper.EvaluateBinary("+", false, true));
        }
    }
}