namespace Signals.Aspects.Auth.NetCore.Extensions
{
    /// <summary>
    /// Extension class for casting <see cref="AuthenticationProperties"/>
    /// </summary>
    public static class TypeCastingExtensions
    {
        /// <summary>
        /// Casts <see cref="AuthenticationProperties"/> to <see cref="Microsoft.AspNetCore.Authentication.AuthenticationProperties"/>
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Microsoft.AspNetCore.Authentication.AuthenticationProperties To(this AuthenticationProperties properties)
        {
            if (properties == null) return new Microsoft.AspNetCore.Authentication.AuthenticationProperties();

            var newProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = properties.RedirectUri,
                AllowRefresh = properties.AllowRefresh,
                ExpiresUtc = properties.ExpiresUtc,
                IsPersistent = properties.IsPersistent,
                IssuedUtc = properties.IssuedUtc,
            };

            if (properties.Dictionary != null)
            {
                foreach (var pair in properties.Dictionary)
                {
                    newProperties.SetParameter(pair.Key, pair.Value);
                }
            }

            return newProperties;
        }
    }
}
