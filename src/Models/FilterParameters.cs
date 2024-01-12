// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="FilterParameters.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Spydersoft.TechRadar.Api.Models
{
    /// <summary>
    /// Class FilterParameters.
    /// </summary>
    public class FilterParameters
    {
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public int[] Tags { get; set; } = new int[0];

        /// <summary>
        /// Gets or sets the updated within days.
        /// </summary>
        /// <value>The updated within days.</value>
        public int UpdatedWithinDays { get; set; }
    }
}