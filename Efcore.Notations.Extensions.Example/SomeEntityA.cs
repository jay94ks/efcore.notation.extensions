using Efcore.Notation.Extensions.Notations;
using Efcore.Notation.Extensions.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Efcore.Notations.Extensions.Example
{
    [Table("TableForSomeEntityA")]
    public class SomeEntityA
    {
        /// <summary>
        /// GUID.
        /// </summary>
        [MultiKey(Order = 0)]
        public Guid Guid { get; set; }

        /// <summary>
        /// Number.
        /// </summary>
        [MultiKey(Order = 1)]
        public int Number { get; set; }

        /// <summary>
        /// Remote Address.
        /// </summary>
        public IPAddress RemoteAddress { get; set; }

        /// <summary>
        /// Text.
        /// </summary>
        [Column(TypeName = "LONGTEXT")]
        public string Text { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public Password Password { get; set; }
    }
}