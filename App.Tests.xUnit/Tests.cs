﻿using Core.Services;
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

    }
}
