/* 
 * Employer-Favourites-Api
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = EmployerFavouritesApi.Client.Client.OpenAPIDateConverter;

namespace EmployerFavouritesApi.Client.Model
{
    /// <summary>
    /// Favourite
    /// </summary>
    [DataContract]
    public partial class Favourite :  IEquatable<Favourite>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Favourite" /> class.
        /// </summary>
        /// <param name="apprenticeshipId">apprenticeshipId.</param>
        /// <param name="ukprns">ukprns.</param>
        public Favourite(string apprenticeshipId = default(string), List<int> ukprns = default(List<int>))
        {
            this.ApprenticeshipId = apprenticeshipId;
            this.Ukprns = ukprns;
        }
        
        /// <summary>
        /// Gets or Sets ApprenticeshipId
        /// </summary>
        [DataMember(Name="apprenticeshipId", EmitDefaultValue=false)]
        public string ApprenticeshipId { get; set; }

        /// <summary>
        /// Gets or Sets Ukprns
        /// </summary>
        [DataMember(Name="ukprns", EmitDefaultValue=false)]
        public List<int> Ukprns { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Favourite {\n");
            sb.Append("  ApprenticeshipId: ").Append(ApprenticeshipId).Append("\n");
            sb.Append("  Ukprns: ").Append(Ukprns).Append("\n");
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
            return this.Equals(input as Favourite);
        }

        /// <summary>
        /// Returns true if Favourite instances are equal
        /// </summary>
        /// <param name="input">Instance of Favourite to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Favourite input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.ApprenticeshipId == input.ApprenticeshipId ||
                    (this.ApprenticeshipId != null &&
                    this.ApprenticeshipId.Equals(input.ApprenticeshipId))
                ) && 
                (
                    this.Ukprns == input.Ukprns ||
                    this.Ukprns != null &&
                    input.Ukprns != null &&
                    this.Ukprns.SequenceEqual(input.Ukprns)
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
                if (this.ApprenticeshipId != null)
                    hashCode = hashCode * 59 + this.ApprenticeshipId.GetHashCode();
                if (this.Ukprns != null)
                    hashCode = hashCode * 59 + this.Ukprns.GetHashCode();
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
