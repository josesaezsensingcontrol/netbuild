namespace NetBuild.App.Core.ApiModel.Responses
{
    public class TokensData
    {
        public string AccessToken { get; set; }
        public long AccessTokenExpirationDate { get; set; }
        public string RefreshToken { get; set; }

        public TokensData(string accessToken, long accessTokenExpirationDate, string refreshToken)
        {
            this.AccessToken = accessToken;
            this.AccessTokenExpirationDate = accessTokenExpirationDate;
            this.RefreshToken = refreshToken;
        }

        protected bool Equals(TokensData other)
        {
            return string.Equals(this.AccessToken, other.AccessToken) && this.AccessTokenExpirationDate == other.AccessTokenExpirationDate && string.Equals(this.RefreshToken, other.RefreshToken);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((TokensData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.AccessToken != null ? this.AccessToken.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ this.AccessTokenExpirationDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.RefreshToken != null ? this.RefreshToken.GetHashCode() : 0);
                return hashCode;
            }
        }

    }
}
