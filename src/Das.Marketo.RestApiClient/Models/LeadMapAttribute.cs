using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Das.Marketo.RestApiClient.Models
{
    /// <summary>
    /// LeadMapAttribute
    /// </summary>
    [DataContract]
    public partial class LeadMapAttribute :  IEquatable<LeadMapAttribute>, IValidatableObject
    {
        /// <summary>
        /// Name of the attribute
        /// </summary>
        /// <value>Name of the attribute</value>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Whether the attribute is read only
        /// </summary>
        /// <value>Whether the attribute is read only</value>
        [DataMember(Name="readOnly", EmitDefaultValue=false)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LeadMapAttribute {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  ReadOnly: ").Append(ReadOnly).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as LeadMapAttribute);
        }

        /// <summary>
        /// Returns true if LeadMapAttribute instances are equal
        /// </summary>
        /// <param name="input">Instance of LeadMapAttribute to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LeadMapAttribute input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.ReadOnly == input.ReadOnly ||
                    this.ReadOnly.Equals(input.ReadOnly)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                hashCode = hashCode * 59 + this.ReadOnly.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
