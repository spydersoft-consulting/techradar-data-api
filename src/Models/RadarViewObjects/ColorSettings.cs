// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="ColorSettings.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Spydersoft.TechRadar.Api.Models.RadarViewObjects
{
    /// <summary>
    /// Class ColorSettings.
    /// </summary>
    public class ColorSettings
    {
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public string? Background { get; set; }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public string? Grid { get; set; }

        /// <summary>
        /// Gets or sets the inactive.
        /// </summary>
        /// <value>The inactive.</value>
        public string? Inactive { get; set; }
    }
}