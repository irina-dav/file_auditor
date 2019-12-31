using FaClient.Controllers;
using FaClient.ViewModels;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;

namespace FaClient.Tests
{
    [Apartment(ApartmentState.STA)]
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void DeserializeListOfRules_LoadFourRulesFromTwoFiles()
        {
            var controllerMock = new Mock<IRulesController>();
            var windowMock = new Mock<Window>();
            var target = new CClientViewModel(controllerMock.Object, windowMock.Object);
            List<string> dataFiles = new List<string>();
            string testDirectory = TestContext.CurrentContext.TestDirectory;
            dataFiles.Add(File.ReadAllText(Path.Combine(testDirectory, @"TestData\TwoCorrectJsonFiles\rules1.json")));
            dataFiles.Add(File.ReadAllText(Path.Combine(testDirectory, @"TestData\TwoCorrectJsonFiles\rules2.json")));

            var rules = target.DeserializeListOfRules(dataFiles);

            Assert.AreEqual(rules.Count, 4);
        }

        [Test]
        public void DeserializeListOfRules_LoadEmptyListFromIncorrectFiles()
        {
            var controllerMock = new Mock<IRulesController>();
            var windowMock = new Mock<Window>();
            var target = new CClientViewModel(controllerMock.Object, windowMock.Object);
            List<string> dataFiles = new List<string>();
            string testDirectory = TestContext.CurrentContext.TestDirectory;
            dataFiles.Add(File.ReadAllText(Path.Combine(testDirectory, @"TestData\IncorrectJsonFiles\rules1.json")));
            dataFiles.Add(File.ReadAllText(Path.Combine(testDirectory, @"TestData\IncorrectJsonFiles\rules2.json")));

            var rules = target.DeserializeListOfRules(dataFiles);

            Assert.IsEmpty(rules);
        }
    }
}
