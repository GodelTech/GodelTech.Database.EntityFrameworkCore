using System;

namespace GodelTech.Database.EntityFrameworkCore.IntegrationTests.Fakes
{
    internal sealed class FakeScope : IDisposable
    {
        public static FakeScope Instance { get; } = new FakeScope();

        private FakeScope()
        {

        }

        public void Dispose()
        {

        }
    }
}