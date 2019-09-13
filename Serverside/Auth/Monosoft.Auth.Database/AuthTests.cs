// <copyright file="AuthTests.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.Auth.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Monosoft.Database.Auth.Datalayer;
    using Monosoft.Database.Auth;
    using Monosoft.Common.DTO;

    /// <summary>
    /// Misc tests for the auth service
    /// </summary>
    [TestClass]
    public class AuthTests
    {

        private string ServiceName = "Monosoft.Database.Auth.Unittest";
        private User adminDbUser = null;
        private Common.DTO.TokenData adminTokenData = null;

        ///// <summary>
        ///// Initialize the data for the test
        ///// </summary>
        //[TestInitialize]
        //public void Init()
        //{
        //    var adminuser = new Monosoft.Auth.DTO.User()
        //    {
        //        Email = "unittest@test.com",
        //        Metadata = null,
        //        Mobile = "0011223344",
        //        Userid = Guid.NewGuid(),
        //        Username = "unittest",
        //        Organisations = null
        //    };
        //    this.adminDbUser = User.CreateAdmin(adminuser, Guid.Empty);

        //    this.adminTokenData = Database.Auth.Datalayer.Token.Login(this.adminDbUser, ServiceName);
        //}

        ///// <summary>
        ///// Check if invalid login is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void InvalidLogin()
        //{
        //    var login = new Monosoft.Auth.DTO.Login() { Password = "dummy", Scope = ServiceName, Username = "dummy" };
        //    var tokendata = Database.Auth.Datalayer.Token.Login(login, "-");

        //    Assert.IsNotNull(tokendata);
        //    Assert.IsTrue(tokendata.Tokenid == Guid.Empty);
        //}

        ///// <summary>
        ///// Check if login is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void ValidLogin()
        //{
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.NewGuid(), OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "ValidLogin@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = "ValidLoginTestUser", Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(Guid.Empty, this.adminTokenData, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);
        //    var tokendata = Database.Auth.Datalayer.Token.Login(dbusr, ServiceName);

        //    Assert.IsNotNull(tokendata);
        //    Assert.IsTrue(tokendata.Tokenid != Guid.Empty);
        //    Assert.IsTrue(tokendata.ValidUntil > DateTime.Now);
        //}

        ///// <summary>
        ///// Check if invalid token is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void VerifyInvalidToken()
        //{
        //    var reloadedusr = MemoryCache.FindUserByToken(Guid.NewGuid(), null);
        //    Assert.IsNull(reloadedusr);
        //}

        /// <summary>
        /// Check if valid token is handled correctly
        /// </summary>
        //[TestMethod]
        //public void VerifyValidToken()
        //{
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.Empty, OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "VerifyValidToken@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = "ValidLoginTestUser", Organisations = new List<Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new MessageHandlers.CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);
        //    var tokendata = Token.Login(dbusr, ServiceName);
        //    var reloadedusr = MemoryCache.FindUserByToken(tokendata.Tokenid, null);
        //    Assert.IsNotNull(reloadedusr);
        //}

        ///// <summary>
        ///// Check if logout is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void Logout()
        //{
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.Empty, OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "Logout@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = "ValidLoginTestUser", Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);
        //    var tokendata = Token.Login(dbusr, ServiceName);
        //    Token.Logout(new Monosoft.Common.DTO.Token() { Tokenid = tokendata.Tokenid, Scope = ServiceName });

        //    //var reloadedusr = MemoryCache. .FindUserByToken(tokendata.Tokenid, null);
        //    Assert.IsNull(reloadedusr);
        //}

        ///// <summary>
        ///// Check if create new user is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void CreateNewUser()
        //{
        //    string usrName = Guid.NewGuid().ToString("N").ToUpper();
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.Empty, OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "CreateNewUser@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = usrName, Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);

        //    Assert.IsNotNull(dbusr);
        //    Assert.AreEqual(usrName, dbusr.Name);
        //    Assert.AreEqual(1, dbusr.UserInOrganisations.Count);
        //    Assert.IsNull(dbusr.UserInGroups);
        //}

        ///// <summary>
        ///// Check if new user with missing claims is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void CreateNewUserMissingClaims()
        //{
        //    string usrName = Guid.NewGuid().ToString("N").ToUpper();
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.NewGuid(), OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "CreateNewUser@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = usrName, Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(org.Id, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);

        //    Assert.IsNull(dbusr);
        //}

        ///// <summary>
        ///// Check if valid user create is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void CreateValidUser()
        //{
        //    string usrName = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8);
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.NewGuid(), OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "CreateValidUser@test.dk", Metadata = null, Mobile = "004512345678", Userid = Guid.NewGuid(), Username = usrName, Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);

        //    Assert.IsNotNull(dbusr);
        //    Assert.IsNotNull(dbusr.Email);
        //    Assert.IsNotNull(dbusr.Mobile);
        //}

        ///// <summary>
        ///// Check if create invalid user is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void CreateInvalidUser()
        //{
        //    string usrName = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8);
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.NewGuid(), OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "CreateInvalidUser.test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = usrName, Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var cc = new CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);

        //    Assert.IsNotNull(dbusr);
        //    Assert.IsNull(dbusr.Email);
        //    Assert.IsNull(dbusr.Mobile);
        //}

        ///// <summary>
        ///// Check if user in multiple usergroups is handled correctly
        ///// </summary>
        //[TestMethod]
        //public void MultipleUserGroups()
        //{
        //    string usrName = Guid.NewGuid().ToString("N").ToUpper();
        //    var org = new Monosoft.Auth.DTO.OrganisationClaims() { Id = Guid.Empty, OrgClaims = null, UserClaims = null };
        //    var usr = new Monosoft.Auth.DTO.User() { Email = "CreateNewUser@test.dk", Metadata = null, Mobile = "0045xxxxxxxx", Userid = Guid.NewGuid(), Username = usrName, Organisations = new List<Monosoft.Auth.DTO.OrganisationClaims>() { org }.ToArray() };
        //    var claim1 = new Common.DTO.MetaData() { Scope = "unittest", Key = "testkey1", Value = "true" };
        //    var claim2 = new Common.DTO.MetaData() { Scope = "unittest", Key = "testkey2", Value = "true" };
        //    var usrGrp1 = new Monosoft.Auth.DTO.UserGroup() { Name = "usergrp 1", Organisationid = org.Id, Users = new List<Monosoft.Auth.DTO.User>() { usr }.ToArray(), Claims = new List<Common.DTO.MetaData>() { claim1 }.ToArray() };
        //    var usrGrp2 = new Monosoft.Auth.DTO.UserGroup() { Name = "usergrp 2", Organisationid = org.Id, Users = new List<Monosoft.Auth.DTO.User>() { usr }.ToArray(), Claims = new List<Common.DTO.MetaData>() { claim2 }.ToArray() };

        //    var cc = new CallContext(Guid.Empty, this.adminTokenData.Tokenid, usr, DateTime.Now);
        //    var dbusr = User.CreateUser(cc);
        //    UserGroup.CreateUserGroup(cc, usrGrp1);
        //    UserGroup.CreateUserGroup(cc, usrGrp2);
        //    var token = Database.Auth.Datalayer.Token.Login(dbusr, "unittest");

        //    Assert.IsTrue(token.Claims.ToList()
        //        .Where(p =>
        //            p.Key == claim1.Key &&
        //            p.Scope == claim1.Scope &&
        //            p.Value == claim1.Value).Any());
        //    Assert.IsTrue(token.Claims.ToList()
        //        .Where(p =>
        //            p.Key == claim2.Key &&
        //            p.Scope == claim2.Scope &&
        //            p.Value == claim2.Value).Any());
        //    Assert.IsFalse(token.Claims.ToList()
        //        .Where(p =>
        //            p.Key == "invalid key" &&
        //            p.Scope == claim2.Scope &&
        //            p.Value == claim2.Value).Any());
        //}

        // Update user test
        // Delete user test
        // Read user test
        // create usergroup test
        // Update usergroup test
        // Delete usergroup test
        // Read usergroup test
    }
}
