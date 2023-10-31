using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Efcore.Notation.Extensions.Internals.Prenotations
{
    /// <summary>
    /// Store <see cref="X509Certificate"/> as string.
    /// </summary>
    internal class X509CertificateAsString
    {
        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        public static void Configure(PropertyBuilder Property)
        {
            if (Property is not PropertyBuilder<X509Certificate> Cert)
                return;

            Cert.HasConversion(
                new ValueConverter<X509Certificate, string>(X => ToString(X), X => SafeParse(X)),
                new ValueComparer<X509Certificate>((A, B) => ToString(A) == ToString(B),
                    X => ToString(X).GetHashCode(), X => SafeParse(ToString(X))));

            Cert.HasColumnType("LONGTEXT");
        }

        /// <summary>
        /// Convert <see cref="Uri"/> to string without exception.
        /// </summary>
        /// <param name="Cert"></param>
        /// <returns></returns>
        private static string ToString(X509Certificate Cert)
        {
            if (Cert is null)
                return string.Empty;

            return Convert.ToBase64String(
                Cert.Export(X509ContentType.Pkcs12), 
                Base64FormattingOptions.None);
        }

        /// <summary>
        /// Parse the input string without exception.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static X509Certificate SafeParse(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;

            try
            {
                var Data = Convert.FromBase64String(Input);
                return new X509Certificate2(
                    Data, null as string,
                    X509KeyStorageFlags.Exportable);
            }

            catch { }
            return null;
        }
    }
}
