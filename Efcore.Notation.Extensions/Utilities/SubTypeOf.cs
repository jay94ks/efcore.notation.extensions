using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Efcore.Notation.Extensions.Utilities
{
    /// <summary>
    /// Subtype of.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct SubTypeOf<T> : ISubTypeOf, IEquatable<SubTypeOf<T>>, IEquatable<Type>, IEquatable<ISubTypeOf>
    {
        private readonly Type m_Type;

        /// <summary>
        /// None.
        /// </summary>
        public static readonly SubTypeOf<T> None = new();

        /// <summary>
        /// Initialize a new <see cref="SubTypeOf{T}"/> instance.
        /// </summary>
        public SubTypeOf() => m_Type = typeof(T);

        /// <summary>
        /// Initialize a new <see cref="SubTypeOf{T}"/> instance.
        /// </summary>
        /// <param name="Type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SubTypeOf(Type Type)
        {
            if (Type is null)
                throw new ArgumentNullException(nameof(Type));

            if (Type.IsAssignableTo(typeof(T)) == false)
                throw new ArgumentException($"the specified type is not assignable to {typeof(T).FullName}.");

            m_Type = Type;
        }

        /// <summary>
        /// Wrapping conversion operator.
        /// </summary>
        /// <param name="Value"></param>
        public static implicit operator SubTypeOf<T>(Type Value) => new SubTypeOf<T>(Value);

        /// <summary>
        /// Unwrapping conversion operator.
        /// </summary>
        /// <param name="Value"></param>
        public static implicit operator Type(SubTypeOf<T> Value) => Value.Type;

        /// <summary>
        /// Assigned Type.
        /// </summary>
        public Type Type => m_Type ?? typeof(T);

        /// <summary>
        /// Name of <see cref="Type"/>.
        /// </summary>
        public string Name => Type.Name;

        /// <summary>
        /// Full Name of <see cref="Type"/>.
        /// </summary>
        public string FullName => Type.FullName;

        /// <summary>
        /// Assembly of <see cref="Type"/>.
        /// </summary>
        public Assembly Assembly => Type.Assembly;

        /// <inheritdoc/>
        public bool Equals(SubTypeOf<T> Other) => FullName == Other.FullName;

        /// <inheritdoc/>
        public bool Equals(Type Other)
        {
            if (Other is null)
                return false;

            return FullName == Other.FullName;
        }

        /// <inheritdoc/>
        public bool Equals(ISubTypeOf Other) => Equals(Other.Type);

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SubTypeOf<T> a, SubTypeOf<T> b) => a.Equals(b) == true;

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SubTypeOf<T> a, ISubTypeOf b) => a.Equals(b) == true;

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(SubTypeOf<T> a, Type b) => a.Equals(b) == true;

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SubTypeOf<T> a, SubTypeOf<T> b) => a.Equals(b) == false;

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SubTypeOf<T> a, ISubTypeOf b) => a.Equals(b) == false;

        /// <summary>
        /// Compare two types.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(SubTypeOf<T> a, Type b) => a.Equals(b) == false;

        /// <inheritdoc/>
        public override int GetHashCode() => Type.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object Input)
        {
            switch(Input)
            {
                case SubTypeOf<T> SubType: return Equals(SubType);
                case Type Type: return Equals(Type);
                default:
                    break;
            }

            if (Input is ISubTypeOf Other)
                return Equals(Other);

            return false;
        }

        /// <inheritdoc/>
        public override string ToString() => FullName;
    }
}
