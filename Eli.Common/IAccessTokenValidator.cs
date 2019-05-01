namespace Eli.Common
{
    public interface IAccessTokenValidator
    {
        bool ValidateToken(string accessToken, string[] scope);
    }
}
