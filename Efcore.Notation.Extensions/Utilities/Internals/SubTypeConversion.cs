using Efcore.Notation.Extensions.Internals.Prenotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efcore.Notation.Extensions.Utilities.Internals
{
    internal class SubTypeConversion
    {
        private class Proxy<T>
        {
            /// <summary>
            /// Calls <see cref="ConfigureInternal{T}(PropertyBuilder{T})"/> method.
            /// </summary>
            /// <param name="Property"></param>
            public Proxy(PropertyBuilder Property)
            {
                ConfigureInternal<T>(Property as PropertyBuilder<SubTypeOf<T>>);
            }
        }

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        internal static void Configure(Type SuperType, PropertyBuilder Property)
        {
            typeof(Proxy<>)
                .MakeGenericType(SuperType)
                .GetConstructors().First()
                .Invoke(new object[] { Property });
        }

        /// <summary>
        /// Configure the property builder.
        /// </summary>
        /// <param name="Property"></param>
        private static void ConfigureInternal<T>(PropertyBuilder<SubTypeOf<T>> Property)
        {
            Property.HasConversion(
                new ValueConverter<SubTypeOf<T>, string>(X => TypeAsString.ToString(X), X => SafeParse<T>(X)),
                new ValueComparer<SubTypeOf<T>>((A, B) => A == B, X => X.GetHashCode(), X => X));

            Property.HasMaxLength(255);
        }

        /// <summary>
        /// Parse the input string to <see cref="SubTypeOf{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static SubTypeOf<T> SafeParse<T>(string Input)
        {
            var Type = TypeAsString.SafeParse(Input);
            if (Type is null)
                return SubTypeOf<T>.None;

            if (Type.IsAssignableTo(typeof(T)) == false)
                return SubTypeOf<T>.None;

            return Type;
        }
    }
}
