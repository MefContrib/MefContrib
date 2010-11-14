namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a metadata key which is required by an import.
    /// </summary>
    public struct RequiredMetadataItem : IEquatable<RequiredMetadataItem>
    {
        /// <summary>
        /// Gets or sets the name of the required metadata item.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the required metadata item.</value>
        public readonly string Name;

        /// <summary>
        /// Gets or sets the type of the required metadata item.
        /// </summary>
        /// <value>A <see cref="Type"/> instance.</value>
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredMetadataItem"/> struct.
        /// </summary>
        /// <param name="name">The name of the required metadata item.</param>
        /// <param name="type">The type of the value of the required metadata item.</param>
        public RequiredMetadataItem(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RequiredMetadataItem"/> to <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="requiredMetadataItem">The required metadata item.</param>
        /// <returns>A <see cref="KeyValuePair{TKey,TValue}"/> instance containing the result of the conversion.</returns>
        public static implicit operator KeyValuePair<string, Type>(RequiredMetadataItem requiredMetadataItem)
        {
            return new KeyValuePair<string, Type>(requiredMetadataItem.Name, requiredMetadataItem.Type);
        }

        /// <summary>
        /// Indicates whether the current <see cref="RequiredMetadataItem"/> is equal to another <see cref="RequiredMetadataItem"/> of the same type.
        /// </summary>
        /// <returns><see langword="true"/> if the current <see cref="RequiredMetadataItem"/> is equal to the <paramref name="requiredMetadataItem"/> parameter; otherwise, <see langword="false"/>.</returns>
        /// <param name="requiredMetadataItem">An <see cref="RequiredMetadataItem"/> to compare with this <see cref="RequiredMetadataItem"/>.</param>
        public bool Equals(RequiredMetadataItem requiredMetadataItem)
        {
            return Equals(this.Name, requiredMetadataItem.Name) && Equals(this.Type, requiredMetadataItem.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is RequiredMetadataItem)
            {
                return Equals((RequiredMetadataItem)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hash1 = (this.Name == null) ? 0 : this.Name.GetHashCode();
            var hash2 = (this.Type == null) ? 0 : this.Type.GetHashCode();
            return hash1 ^ hash2;
        }

        public static bool operator ==(RequiredMetadataItem item1, RequiredMetadataItem item2)
        {
            return item1.Equals(item2);
        }

        public static bool operator !=(RequiredMetadataItem item1, RequiredMetadataItem item2)
        {
            return !item1.Equals(item2);
        }
    }
}
