using Signals.Aspects.Auth.Properties;

namespace Signals.Aspects.Auth.Mvc5.Extensions
{
    /// <summary>
    /// Extension class for casting <see cref="AuthenticationProperties"/>
    /// </summary>
    public static class TypeCastingExtensions
    {
        /// <summary>
        /// Casts <see cref="AuthenticationProperties"/> to <see cref="Microsoft.Owin.Security.AuthenticationProperties"/>
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Microsoft.Owin.Security.AuthenticationProperties To(this AuthenticationProperties properties)
        {
            if (properties == null) return new Microsoft.Owin.Security.AuthenticationProperties();

            var newProperties = new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = properties.RedirectUri,
                AllowRefresh = properties.AllowRefresh,
                ExpiresUtc = properties.ExpiresUtc,
                IsPersistent = properties.IsPersistent,
                IssuedUtc = properties.IssuedUtc,
            };

            foreach (var pair in properties.Dictionary)
            {
                newProperties.Dictionary[pair.Key] = pair.Value;
            }

            return newProperties;
        }
    }
}
