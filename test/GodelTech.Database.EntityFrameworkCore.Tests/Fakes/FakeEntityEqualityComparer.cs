using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GodelTech.Database.EntityFrameworkCore.Tests.Fakes
{
    public class FakeEntityEqualityComparer : IEqualityComparer<FakeEntity>
    {
        public bool Equals(FakeEntity x, FakeEntity y)
        {
            // Check whether the compared objects reference the same data
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

            // Check whether the objects' properties are equal.
            return x.Id.Equals(y.Id)
                   && x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] FakeEntity obj)
        {
            // Check whether the object is null
            if (ReferenceEquals(obj, null)) return 0;

            // Calculate the hash code for the object.
            return obj.Id.GetHashCode()
                   ^ obj.Name.GetHashCode(StringComparison.InvariantCulture);
        }
    }
}