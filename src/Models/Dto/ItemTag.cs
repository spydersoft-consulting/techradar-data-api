// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="SimpleTag.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Spydersoft.TechRadar.Api.Models.Dto
{
    /// <summary>
    /// Class SimpleTag.
    /// </summary>
    public class ItemTag
    {
        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>The tag identifier.</value>
        public int TagId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; } = string.Empty;
    }
}