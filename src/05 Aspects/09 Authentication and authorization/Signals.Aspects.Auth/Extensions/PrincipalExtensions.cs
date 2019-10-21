using Signals.Aspects.Auth.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Signals.Aspects.Auth.Extensions
{
    /// <summary>
    /// Extensions for claims principal manipulation
    /// </summary>
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Claim type for authentication properties
        /// </summary>
        public static readonly string AuthenticationPropertiesClaimName = "AuthenticationProperties";

        /// <summary>
        /// Adds claim to a principal with serialized value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="principal"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public static void AddClaim<T>(this ClaimsPrincipal principal, string name, T obj)
        {
	        (principal.Identity as ClaimsIdentity)?.AddClaim(new Claim(name, JsonSerializer.Serialize(obj)));
        }

        /// <summary>
        /// Extract claims using <see cref="ClaimTypeAttribute"/> from objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> ExtractClaims<T>(this T obj) where T : class
        {
            var type = obj.GetType();
            var claims = new List<Claim>();

            foreach (var prop in type.GetProperties())
            {
                var attributes = prop.GetCustomAttributes(typeof(ClaimTypeAttribute), true);

                foreach (var attribute in attributes)
                {
                    var claimType = (attribute as ClaimTypeAttribute)?.Type;
                    var value = prop.GetValue(obj)?.ToString();

					if (value != null)
					{
						claims.Add(new Claim(claimType, value));
					}
                }
            }

            return claims;
        }

        /// <summary>
        /// Gets claim from principal with deserialized value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="principal"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetClaim<T>(this ClaimsPrincipal principal, string name) where T : class
        {
            var claim = principal.Claims.FirstOrDefault(x => x.Type == name);
            if (claim == null) return default(T);
            return JsonSerializer.Deserialize<T>(claim.Value);
        }

        /// <summary>
        /// Gets list of roles from the principal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string[] GetRoles(this ClaimsPrincipal principal)
        {
            return principal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
        }
    }
}
