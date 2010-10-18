namespace MefContrib.Hosting.Conventions
{
    using System;

    /// <summary>
    /// Defines a metadata value which is eposed by an parts and exports.
    /// </summary>
    public struct MetadataItem : IEquatable<MetadataItem>
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="MetadataItem"/>.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the <see cref="MetadataItem"/>.</value>
        public readonly string Name;

        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataItem"/>.
        /// </summary>
        /// <value>An <see cref="object"/> containing the value of the <see cref="MetadataItem"/>.</value>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataItem"/> struct.
        /// </summary>
        /// <param name="name">A <see cref="string"/> containing the name of the <see cref="MetadataItem"/>.</param>
        /// <param name="value">A <see cref="Func{T,TResult}"/> function used to retrieve the value of the <see cref="MetadataItem"/>.</param>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="name"/> or <see cref="value"/> parameters were <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The lenght of the <paramref name="name"/> parameter was zero.</exception>
        public MetadataItem(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "The name cannot be null.");
            }

            if (name.Length == 0)
            {
                throw new ArgumentOutOfRangeException("name", "The name cannot be empty.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value", "The value cannot be null.");
            }

            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Indicates whether the current <see cref="MetadataItem"/> is equal to another <see cref="MetadataItem"/> of the same type.
        /// </summary>
        /// <param name="item">An <see cref="MetadataItem"/> to compare with this <see cref="MetadataItem"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="MetadataItem"/> is equal to the <paramref name="item"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(MetadataItem item)
        {
            return Equals(this.Name, item.Name) &&
                   Equals(this.Value, item.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is MetadataItem)
            {
                return Equals((MetadataItem)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hash1 = (this.Name == null) ? 0 : this.Name.GetHashCode();
            var hash2 = (this.Value == null) ? 0 : this.Value.GetHashCode();
            return hash1 ^ hash2;
        }

        public static bool operator ==(MetadataItem item1, MetadataItem item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(MetadataItem item1, MetadataItem item2)
        {
            return !item1.Equals(item2);
        }
    }
}
