using Rotary.Core.Http.Enums;

namespace Rotary.Core.Http.Records
{
    public record AuthDefinition
    {
        public sealed record NoAuthDefinition : AuthDefinition { }

        public sealed record BasicAuthDefinition : AuthDefinition
        {
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
        }

        public sealed record BearerTokenAuthDefinition : AuthDefinition
        {
            public string Token { get; init; } = string.Empty;
        }

        public sealed record ApiKeyAuthDefinition : AuthDefinition
        {
            public string Key { get; init; } = string.Empty;
            public string Value { get; init; } = string.Empty;
            public ApiKeyAuthLocation Location { get; init; } = ApiKeyAuthLocation.Header; // or ApiKeyAuthLocation.Query, ApiKeyAuthLocation.Cookie
        }

        public sealed record AwsIamAuthDefinition : AuthDefinition
        {
            public string AccessKey { get; init; } = string.Empty;
            public string SecretKey { get; init; } = string.Empty;
            public string Region { get; init; } = string.Empty;
            public string Service { get; init; } = string.Empty;
            public string SessionToken { get; init; } = string.Empty;
        }

        public sealed record OauthOneAuthDefinition : AuthDefinition
        {
            public string ConsumerKey { get; init; } = string.Empty;
            public string ConsumerSecret { get; init; } = string.Empty;
            public string Token { get; init; } = string.Empty;
            public string TokenSecret { get; init; } = string.Empty;
        }

        public sealed record OauthTwoAuthDefinition : AuthDefinition
        {
            public string AccessToken { get; init; } = string.Empty;
            public string TokenType { get; init; } = "Bearer";
        }

        public sealed record DigestAuthDefinition : AuthDefinition
        {
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
        }

        public sealed record NtlmAuthDefinition : AuthDefinition
        {
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
            public string Domain { get; init; } = string.Empty;
        }

        public sealed record HawkAuthDefinition : AuthDefinition
        {
            public string Id { get; init; } = string.Empty;
            public string Key { get; init; } = string.Empty;
            public string Algorithm { get; init; } = "sha256";
            public string Ext { get; init; } = string.Empty;
        }
    }
}
