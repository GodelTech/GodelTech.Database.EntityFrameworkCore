using GodelTech.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    [ExcludeFromCodeCoverage]
    public class FakeEntity : IEntity<int>
    {
        public int Id { get; set; }

        public bool Equals(IEntity<int> x, IEntity<int> y)
        {
            throw new Exception("Equals is fake method!");
        }

        public int GetHashCode(IEntity<int> obj)
        {
            throw new Exception("GetHashCode is fake method!");
        }
    }
}
