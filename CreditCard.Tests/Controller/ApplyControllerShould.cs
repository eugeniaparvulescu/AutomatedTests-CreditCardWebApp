using CreditCards.Controllers;
using CreditCards.Core.Interfaces;
using CreditCards.Core.Model;
using CreditCards.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CreditCard.Tests.Controller
{
    public class ApplyControllerShould
    {
        public readonly Mock<ICreditCardApplicationRepository> _mockRepository;
        public readonly ApplyController _sut;

        public ApplyControllerShould()
        {
            _mockRepository = new Mock<ICreditCardApplicationRepository>();
            _sut = new ApplyController(_mockRepository.Object);
        }

        [Fact]
        public void ReturnViewForIndex()
        {
            IActionResult result = _sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void ReturnViewForInvalidState()
        {
            _sut.ModelState.AddModelError("x", "Errorrr");

            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Geni"
            };

            IActionResult result = await _sut.Index(application);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<NewCreditCardApplicationDetails>(viewResult.Model);
            Assert.Equal(application.FirstName, model.FirstName);
        }

        [Fact]
        public async void NotSaveApplicattionWhenModelError()
        {
            _sut.ModelState.AddModelError("x", "Errorrr");

            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Geni"
            };

            await _sut.Index(application);

            _mockRepository.Verify(x => x.AddAsync(It.IsAny<CreditCardApplication>()), Times.Never);
        }

        [Fact]
        public async void SaveApplicationWhenValidModel()
        {
            CreditCardApplication savedApplication = null;

            _mockRepository.Setup(x => x.AddAsync(It.IsAny<CreditCardApplication>()))
                .Returns(Task.CompletedTask)
                .Callback<CreditCardApplication>(x => savedApplication = x);

            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Eugenia",
                LastName = "Parvulescu",
                Age = 21,
                GrossAnnualIncome = 100_000,
                FrequentFlyerNumber = "012345-A"
            };
            await _sut.Index(application);

            _mockRepository.Verify(x => x.AddAsync(It.IsAny<CreditCardApplication>()), Times.Once);

            Assert.Equal(application.FirstName, savedApplication.FirstName);
            Assert.Equal(application.LastName, savedApplication.LastName);
            Assert.Equal(application.Age, savedApplication.Age);
            Assert.Equal(application.GrossAnnualIncome, savedApplication.GrossAnnualIncome);
            Assert.Equal(application.FrequentFlyerNumber, savedApplication.FrequentFlyerNumber);
        }

        [Fact]
        public async void ReturnApplicationCompleteViewWhenValidModel()
        {
            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Eugenia",
                LastName = "Parvulescu",
                Age = 21,
                GrossAnnualIncome = 100_000,
                FrequentFlyerNumber = "012345-A"
            };
            var result = await _sut.Index(application);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ApplicationComplete", viewResult.ViewName);
        }
    }
}
