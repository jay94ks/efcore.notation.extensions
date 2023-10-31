using Efcore.Notation.Extensions.Internals.Prenotations;
using Efcore.Notation.Extensions.Utilities;
using Efcore.Notation.Extensions.Utilities.Ecdsa;
using Efcore.Notation.Extensions.Utilities.Internals;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Efcore.Notation.Extensions.Internals
{
    internal class PrenotationProvider
    {
        private static readonly Dictionary<Type, Action<PropertyBuilder>> PRENOTATED
            = new Dictionary<Type, Action<PropertyBuilder>>()
            {
                { typeof(Guid), GuidAsString.Configure },
                { typeof(Color), ColorAsString.Configure },
                { typeof(Password), Password.Configure },

                { typeof(Hash256), EcdsaConversions.Configure },
                { typeof(Pub256), EcdsaConversions.Configure },
                { typeof(Key256), EcdsaConversions.Configure },
                { typeof(Sign256), EcdsaConversions.Configure },

                { typeof(Uri), UriAsString.Configure },
                { typeof(Type), TypeAsString.Configure },

                { typeof(IPAddress), IPAddressAsString.Configure },
                { typeof(IPEndPoint), IPEndPointAsString.Configure },
                { typeof(JObject), JsonAsString.Configure },
                { typeof(JArray), JsonAsString.Configure },
                { typeof(Encoding), EncodingAsString.Configure },

                { typeof(CultureInfo), CultureInfoAsString.Configure },
                { typeof(TimeZoneInfo), TimeZoneInfoAsString.Configure },

                { typeof(X509Certificate), X509CertificateAsString.Configure },
            };

        /// <summary>
        /// Get an action to configure property builder if exists.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static Action<PropertyBuilder> Get(Type Type)
        {
            PRENOTATED.TryGetValue(Type, out var Action);
            return Action;
        }
    }
}
