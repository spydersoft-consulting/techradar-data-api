// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="ArcController.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Spydersoft.TechRadar.Api.Data;
using Spydersoft.TechRadar.Api.Services;
using Microsoft.AspNetCore.Mvc;
using RadarArc = Spydersoft.TechRadar.Api.Data.RadarArc;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class ArcController.
    /// Implements the <see cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    /// </summary>
    /// <seealso cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    public class ArcController : EditControllerBase<RadarArc>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcController" /> class.
        /// </summary>
        /// <param name="radarDataItemService">The radar data item service.</param>
        public ArcController(IRadarDataItemService radarDataItemService) : base(radarDataItemService)
        {
        }

        /// <summary>
        /// Deletes an Arc
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            RadarDataItemService.DeleteRadarDataItem<RadarArc>(id, User);
        }
    }
}