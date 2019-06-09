using CreditCards.Core.Interfaces;
using CreditCards.Core.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CreditCard.Tests.Model
{
    public class CreditCardApplicationEvaluatorShould
    {
        private const int ExpectedLowIncomeTreshhold = 20_000;
        private const int ExpectedHighIncomeTreshhold = 100_000;

        private readonly CreditCardApplicationEvaluator _sut;
        private readonly Mock<IFrequentFlyerNumberValidator> _moqValidator;

        public CreditCardApplicationEvaluatorShould()
        {
            _moqValidator = new Mock<IFrequentFlyerNumberValidator>();
            _moqValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            _sut = new CreditCardApplicationEvaluator(_moqValidator.Object);
        }

        [Theory]
        [InlineData(ExpectedHighIncomeTreshhold)]
        [InlineData(ExpectedHighIncomeTreshhold + 1)]
        [InlineData(int.MaxValue)]
        public void AcceptAllHighIncomeApplicants(int income)
        {
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income,
            };

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, _sut.Evaluate(application));
        }

        [Theory]
        [InlineData(20)]
        [InlineData(19)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void ReferYoungApplicantsWhoAreNotHighIncome(int age)
        {
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = ExpectedHighIncomeTreshhold - 1,
                Age = age,
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, _sut.Evaluate(application));
        }

        [Theory]
        [InlineData(ExpectedLowIncomeTreshhold)]
        [InlineData(ExpectedLowIncomeTreshhold + 1)]
        [InlineData(ExpectedHighIncomeTreshhold - 1)]
        public void ReferNonYoungApplicantsWhoAreMiddleIncome(int income)
        {
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income,
                Age = 21
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, _sut.Evaluate(application));
        }

        [Theory]
        [InlineData(ExpectedLowIncomeTreshhold - 1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void DeclineAllApplicantsWhoAreLowIncome(int income)
        {
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income,
                Age = 21
            };

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, _sut.Evaluate(application));
        }

        [Fact]
        public void ReferInvalidFrequentNumbers_RealValidator()
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = ExpectedLowIncomeTreshhold,
                Age = 21,
                FrequentFlyerNumber = "0a2c3v3"
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, sut.Evaluate(application));
        }

        [Fact]
        public void ReferInvalidFrequentNumber_MoqValidator()
        {
            _moqValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var application = new CreditCardApplication();

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, _sut.Evaluate(application));
        }
    }
}
