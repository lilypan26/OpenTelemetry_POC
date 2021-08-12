using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matrix.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace Matrix.Controllers.Tests
{
    [TestClass()]
    public class MatrixControllerTests
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [TestMethod()]
        public void GetTest()
        {
            var res = HttpClient.GetStringAsync("https://localhost:5003/Matrix").Result;
            Assert.IsTrue(res == "red pill" || res == "blue pill");
        }
    }
}