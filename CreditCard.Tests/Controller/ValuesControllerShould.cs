using CreditCards.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CreditCard.Tests.Controller
{
    public class ValuesControllerShould
    {
        private readonly ValuesController _sut;

        public ValuesControllerShould()
        {
            _sut = new ValuesController();
        }

        [Fact]
        public void ReturnValues()
        {
            string[] result = _sut.Get().ToArray();

            Assert.Equal(2, result.Length);
            Assert.Equal("value1", result[0]);
            Assert.Equal("value2", result[1]);
        }

        [Fact]
        public void ReturnBadRequest()
        {
            IActionResult result = _sut.Get(0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Invalid request for id 0", badRequest.Value);
        }

        [Fact]
        public void StartJobOk()
        {
            IActionResult result = _sut.StartJob();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Batch Job Started", okResult.Value);
        }
    }
}
