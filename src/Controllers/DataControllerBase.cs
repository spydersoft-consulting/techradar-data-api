// ***********************************************************************
// Assembly         : Spydersoft.TechRadar.Api
// Author           : MGerega
// Created          : 02-11-2019
//
// Last Modified By : MGerega
// Last Modified On : 08-21-2019
// ***********************************************************************
// <copyright file="RadarItemControllerBase.cs" company="Spydersoft.TechRadar.Api">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Spydersoft.TechRadar.Api.Data;
using Spydersoft.TechRadar.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class RadarItemControllerBase.
    /// Implements the <see cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    /// </summary>
    /// <seealso cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    [ApiController]
    [Route("api/[controller]")]
    public class DataControllerBase : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataControllerBase" /> class.
        /// </summary>
        /// <remarks>This class provides easy access to data and services for the TechRadar.</remarks>
        public DataControllerBase()
        {

        }



        /// <summary>
        /// Options implementation for all derivative classes.
        /// </summary>
        [HttpOptions("")]
        public void Options()
        {
        }
    }
}