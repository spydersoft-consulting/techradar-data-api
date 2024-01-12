// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="Quadrant.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.DataAnnotations;

namespace Spydersoft.TechRadar.Api.Data
{
    /// <summary>
    /// Class Quadrant.
    /// </summary>
    public class Quadrant : IRadarDataItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the radar identifier.
        /// </summary>
        /// <value>The radar identifier.</value>
        public int RadarId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [Required]
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [Required]
        public int Position { get; set; }
    }
}