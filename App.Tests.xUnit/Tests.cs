using System;

using App.ViewModels;
using Core.Services;
using Moq;
using Xunit;

namespace App.Tests.XUnit
{
    // TODO WTS: Add appropriate tests
    public class Tests
    {
        private readonly Mock<IMandrillService> _mandrillServiceMock;

        public Tests()
        {
            _mandrillServiceMock = new Mock<IMandrillService>();
        }

        [Fact]
        public void TestMethod1()
        {
        }

        // TODO WTS: Add tests for functionality you add to ListDetailViewModel.
        [Fact]
        public void TestListDetailViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new TemplatesViewModel(_mandrillServiceMock.Object);
            Assert.NotNull(vm);
        }

        // TODO WTS: Add tests for functionality you add to SettingsViewModel.
        [Fact]
        public void TestSettingsViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new SettingsViewModel();
            Assert.NotNull(vm);
        }
    }
}
