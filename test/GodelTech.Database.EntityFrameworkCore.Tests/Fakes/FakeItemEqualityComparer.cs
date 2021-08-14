using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeItemEqualityComparer : IEqualityComparer<FakeItem>
    {
        public bool Equals(FakeItem x, FakeItem y)
        {
            // Check whether the compared objects reference the same data
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            // Check whether the objects' properties are equal.
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode([DisallowNull] FakeItem obj)
        {
            // Check whether the object is null
            if (ReferenceEquals(obj, null)) return 0;

            // Calculate the hash code for the object.
            return obj.Id.GetHashCode();
        }
    }
}