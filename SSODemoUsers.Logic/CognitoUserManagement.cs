using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace SSODemoUsers.Logic
{
    public class CognitoUserManagement
    {
        private readonly AmazonCognitoIdentityProviderClient _adminAmazonCognitoIdentityProviderClient;
        private readonly AmazonCognitoIdentityProviderClient _anonymousAmazonCognitoIdentityProviderClient;

        public CognitoUserManagement(string profileName, RegionEndpoint regionEndpoint)
        {
            var credentialProfileStoreChain = new CredentialProfileStoreChain();

            if (credentialProfileStoreChain.TryGetAWSCredentials(profileName, out var internalAwsCredentials))
            {
                _adminAmazonCognitoIdentityProviderClient =
                    new AmazonCognitoIdentityProviderClient(internalAwsCredentials, regionEndpoint);
                _anonymousAmazonCognitoIdentityProviderClient =
                    new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), regionEndpoint);
            }
            else
            {
                throw new ArgumentNullException(nameof(AWSCredentials));
            }
        }

        public async Task AdminCreateUserAsync(string username, string password, string userPoolId, string appClientId,
            List<AttributeType> attributeTypes)
        {
            var adminCreateUserRequest = new AdminCreateUserRequest
            {
                Username = username,
                TemporaryPassword = password, 
                UserPoolId = userPoolId,
                UserAttributes = attributeTypes
            };
            var adminCreateUserResponse =
                await _adminAmazonCognitoIdentityProviderClient.AdminCreateUserAsync(adminCreateUserRequest);

            var adminUpdateUserAttributesRequest = new AdminUpdateUserAttributesRequest
            {
                Username = username,
                UserPoolId = userPoolId,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType()
                    {
                        Name = "email_verified", Value = "true"
                    }
                }
            };

            var adminUpdateUserAttributesResponse =
                await _adminAmazonCognitoIdentityProviderClient.AdminUpdateUserAttributesAsync(
                    adminUpdateUserAttributesRequest);

            var adminInitiateAuthRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = userPoolId,
                ClientId = appClientId,
                AuthFlow = "ADMIN_NO_SRP_AUTH",
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "PASSWORD", password }
                }
            };

            var adminInitiateAuthResponse =
                await _adminAmazonCognitoIdentityProviderClient.AdminInitiateAuthAsync(adminInitiateAuthRequest);

            var adminRespondToAuthChallengeRequest = new AdminRespondToAuthChallengeRequest
            {
                ChallengeName = ChallengeNameType.NEW_PASSWORD_REQUIRED,
                ClientId = appClientId,
                UserPoolId = userPoolId,
                ChallengeResponses = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "NEW_PASSWORD", password }
                },
                Session = adminInitiateAuthResponse.Session
            };

            var adminRespondToAuthChallengeResponse =
                await _adminAmazonCognitoIdentityProviderClient.AdminRespondToAuthChallengeAsync(
                    adminRespondToAuthChallengeRequest);

        }

        public async Task AdminAddUserToGroupAsync(string username, string userPoolId, string groupName)
        {
            var adminAddUserToGroupRequest = new AdminAddUserToGroupRequest
            {
                Username = username,
                UserPoolId = userPoolId,
                GroupName = groupName
            };

            var adminAddUserToGroupResponse =
                await _adminAmazonCognitoIdentityProviderClient.AdminAddUserToGroupAsync(adminAddUserToGroupRequest);
        }

        public async Task<AdminInitiateAuthResponse> AdminAuthenticateUserAsync(string username, string password,
            string userPoolId, string appClientId)
        {
            var adminInitiateAuthRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = userPoolId,
                ClientId = appClientId,
                AuthFlow = "ADMIN_NO_SRP_AUTH",
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "PASSWORD", password }
                }
            };

            return await _adminAmazonCognitoIdentityProviderClient.AdminInitiateAuthAsync(adminInitiateAuthRequest);
        }

        public async Task AdminRemoveUserFromGroupAsync(string username, string userPoolId, string groupName)
        {
            var adminRemoveUserFromGroupRequest = new AdminRemoveUserFromGroupRequest
            {
                Username = username,
                UserPoolId = userPoolId,
                GroupName = groupName
            };

            await _adminAmazonCognitoIdentityProviderClient.AdminRemoveUserFromGroupAsync(
                adminRemoveUserFromGroupRequest);
        }

        public async Task AdminDisableUserAsync(string username, string userPoolId)
        {
            var adminDisableUserRequest = new AdminDisableUserRequest
            {
                UserPoolId = userPoolId,
                Username = username
            };

            await _adminAmazonCognitoIdentityProviderClient.AdminDisableUserAsync(adminDisableUserRequest);
        }

        public async Task AdminEnableUserAsync(string username, string userPoolId)
        {
            var adminEnableUserRequest = new AdminEnableUserRequest()
            {
                UserPoolId = userPoolId,
                Username = username
            };

            await _adminAmazonCognitoIdentityProviderClient.AdminEnableUserAsync(adminEnableUserRequest);
        }

        public async Task AdminDeleteUserAsync(string username, string userPoolId)
        {
            var deleteUserRequest = new AdminDeleteUserRequest
            {
                Username = username,
                UserPoolId = userPoolId
            };

            await _adminAmazonCognitoIdentityProviderClient.AdminDeleteUserAsync(deleteUserRequest);
        }

        public async Task<ListUsersResponse> AdminListUsersAsync(string userPoolId, List<string> attributesToGet, string filter, int limit, string paginationToken)
        {
            var listUserRequest = new ListUsersRequest
            {
                UserPoolId = userPoolId,
                AttributesToGet = attributesToGet,
                Filter = filter,
                Limit = limit,
                PaginationToken = paginationToken
            };

            var listUSerResponse = await _adminAmazonCognitoIdentityProviderClient.ListUsersAsync(listUserRequest);
            
            return listUSerResponse;
        }
    }

    

}