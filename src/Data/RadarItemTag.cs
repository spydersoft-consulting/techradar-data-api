// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="RadarItemTag.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Spydersoft.TechRadar.Api.Data
{
    /// <summary>
    /// Class RadarItemTag.
    /// </summary>
    public class RadarItemTag
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the radar item identifier.
        /// </summary>
        /// <value>The radar item identifier.</value>
        public int RadarItemId { get; set; }

        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>The tag identifier.</value>
        public int TagId { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        [JsonIgnore]
        public Tag? Tag { get; set; }

        /// <summary>
        /// Gets or sets the radar item.
        /// </summary>
        /// <value>The radar item.</value>
        [JsonIgnore]
        public RadarItem? RadarItem { get; set; }
    }
}