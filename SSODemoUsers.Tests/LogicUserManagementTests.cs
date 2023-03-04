using System.Net;
using Amazon;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Configuration;
using SSODemoUsers.Logic;

namespace SSODemoUsers.Tests
{
    public class LogicUserManagementTests
    {
        private readonly string _userPoolId;
        private readonly string _appClientId;
        private readonly string _profile;

        public LogicUserManagementTests()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddUserSecrets<LogicUserManagementTests>().Build();

            _userPoolId = config["Cognito:UserPoolId"];
            _appClientId = config["Cognito:AppClientId"];
            _profile = config["Aws:Credentials:Profile"];
        }

        [Fact]
        public async Task List_Users_Success_Test()
        {
            // arrange
            
            var regionEndpoint = RegionEndpoint.USEast1;
            var attributesToGet = new List<string>();
            var filter = "";
            var limit = 20;
            string paginationToken = null;

            var target = new CognitoUserManagement(_profile, regionEndpoint);

            // act
            var listResponse = await target.AdminListUsersAsync(_userPoolId, attributesToGet, filter, limit, paginationToken);

            // asserts
            Assert.NotNull(listResponse);
            Assert.Equal(HttpStatusCode.OK, listResponse.HttpStatusCode);
        }

        [Fact]
        public async Task Admin_Users_Add_Delete_Success_Test()
        {
            // arrange
            var regionEndpoint = RegionEndpoint.USEast1;
            var name = "User Test";
            var username = "cognito-test1@josealonso.dev";
            var password = "Pass123$";
            var attributeTypes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = name }
            };

            var target = new CognitoUserManagement(_profile, regionEndpoint);

            // act
            await target.AdminCreateUserAsync(username, password,  _userPoolId, _appClientId, attributeTypes);


            // assert
            //Assert.NotNull(addResponse);

            // Delete the created user
            await target.AdminDeleteUserAsync(username, _userPoolId);
        }

        [Fact]
        public async Task Admin_Users_Add_Remove_User_To_Group_Success_Test()
        {
            // arrange
            var regionEndpoint = RegionEndpoint.USEast1;
            var name = "User Test";
            var username = "cognito-test1@josealonso.dev";
            var password = "Pass123$";
            var attributeTypes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = name }
            };

            var target = new CognitoUserManagement(_profile, regionEndpoint);
            await target.AdminCreateUserAsync(username, password, _userPoolId, _appClientId, attributeTypes);

            // act
            await target.AdminAddUserToGroupAsync(username, _userPoolId, "SSOGroup");

            await target.AdminRemoveUserFromGroupAsync(username, _userPoolId, "SSOGroup");


            // assert
            //Assert.NotNull(addResponse);

            // Delete the created user
            await target.AdminDeleteUserAsync(username, _userPoolId);
        }

        [Fact]
        public async Task Admin_Users_Disable_Enable_User_Success_Test()
        {
            // arrange
            var regionEndpoint = RegionEndpoint.USEast1;
            var name = "User Test";
            var username = "cognito-test1@josealonso.dev";
            var password = "Pass123$";
            var attributeTypes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = name }
            };

            var target = new CognitoUserManagement(_profile, regionEndpoint);
            await target.AdminCreateUserAsync(username, password, _userPoolId, _appClientId, attributeTypes);

            // act
            await target.AdminDisableUserAsync(username, _userPoolId);
            await target.AdminEnableUserAsync(username, _userPoolId);


            // assert
            //Assert.NotNull(addResponse);

            // Delete the created user
            await target.AdminDeleteUserAsync(username, _userPoolId);
        }


        [Fact]
        public async Task Admin_Users_Authenticate_User_Success_Test()
        {
            // arrange
            var regionEndpoint = RegionEndpoint.USEast1;
            var name = "User Test";
            var username = "cognito-test1@josealonso.dev";
            var password = "Pass123$";
            var attributeTypes = new List<AttributeType>
            {
                new AttributeType { Name = "name", Value = name }
            };

            var target = new CognitoUserManagement(_profile, regionEndpoint);
            await target.AdminCreateUserAsync(username, password, _userPoolId, _appClientId, attributeTypes);

            // act
            var authResponse = await target.AdminAuthenticateUserAsync(username, password, _userPoolId, _appClientId);


            // assert
            Assert.NotNull(authResponse);

            // Delete the created user
            await target.AdminDeleteUserAsync(username, _userPoolId);
        }
    }
}