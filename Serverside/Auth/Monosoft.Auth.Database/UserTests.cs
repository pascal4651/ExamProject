// <copyright file="UserTests.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.Auth.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests specific for a user
    /// </summary>
    [TestClass]
    public class UserTests
    {
        /// <summary>
        /// Check that email validation works as expected
        /// </summary>
        /// <param name="email">email to check</param>
        /// <param name="valid">expected result for the email supplied</param>
        [DataTestMethod]
        [DataRow("test@test.dk", true)]
        [DataRow("test.test@test.dk", true)]
        [DataRow("test1@test.dk", true)]
        [DataRow("test+1@test.dk", true)]
        [DataRow("test@domain.com", true)]
        [DataRow("t@stest.dk", true)]
        [DataRow("t!st@test.dk", false)]
        [DataRow("test(at)test.dk", false)]
        [DataRow("test@domain", false)]
        [DataRow("test@domain.test.dk", true)]
        public void TestEmailValidation(string email, bool valid)
        {
            Assert.AreEqual(valid, Database.Auth.Datalayer.User.ValidEmail(email));
        }

        /// <summary>
        /// Check that phone validation works as expected
        /// </summary>
        /// <param name="phone">phone to check</param>
        /// <param name="valid">expected result for the phone supplied</param>
        [TestMethod]
        [DataRow("+45 12345678", false)]
        [DataRow("+45123345678", true)]
        [DataRow("+45 12 34 56 78", false)]
        [DataRow("0045 123345678", false)]
        [DataRow("45123345678", true)]
        [DataRow("ab123345678", false)]
        public void TestPhoneValidation(string phone, bool valid)
        {
            Assert.AreEqual(valid, Database.Auth.Datalayer.User.ValidPhone(phone));
        }
    }
}