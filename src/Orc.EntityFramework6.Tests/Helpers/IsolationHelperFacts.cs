namespace Orc.EntityFramework.Tests
{
    using System;
    using System.Data;
    using Catel.Data;
    using NUnit.Framework;

    public class IsolationHelperFacts
    {
        #region Nested type: TheTranslateTransactionLevelToSqlMethod
        [TestFixture]
        public class TheTranslateTransactionLevelToSqlMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsCorrectValueForReadUncommitted()
            {
                var expectedValue = "SET TRANSACTION LEVEL ISOLATION LEVEL READ UNCOMMITTED;";
                var actualValue = IsolationHelper.TranslateTransactionLevelToSql(IsolationLevel.ReadUncommitted);

                Assert.AreEqual(expectedValue, actualValue);
            }
            #endregion
        }
        #endregion
    }
}