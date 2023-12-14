// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EfConnectionStringHelperFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if EF5

namespace Orc.EntityFramework.Tests
{
    using System;
    using Catel.Data;
    using Catel.Tests;
    using NUnit.Framework;
    using ObjectContext;

    public class EfConnectionStringHelperFacts
    {
        [TestFixture]
        public class TheGetEntityFrameworkConnectionStringMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullContextType()
            {
                Assert.Throws<ArgumentNullException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(null, TestConnectionStrings.ObjectContextDefault));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyConnectionString()
            {
                Assert.Throws<ArgumentException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), null));
                Assert.Throws<ArgumentException>(() => EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), string.Empty));
            }


            [TestCase]
            public void ReturnsCorrectValueForTestObjectContext()
            {
                string expectedValue = string.Format("metadata=res://*/TestObjectContext.csdl|res://*/TestObjectContext.ssdl|res://*/TestObjectContext.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"", TestConnectionStrings.ObjectContextDefault);

                var connectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(typeof(TestObjectContextContainer), TestConnectionStrings.ObjectContextDefault);

                Assert.AreEqual(expectedValue, connectionString);
            }
        }
    }
}

#endif