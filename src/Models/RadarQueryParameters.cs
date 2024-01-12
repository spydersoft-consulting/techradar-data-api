﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spydersoft.TechRadar.Api.Models
{
    /// <summary>
    /// Class RadarQueryParameters.
    /// </summary>
    public class RadarQueryParameters
    {
        /// <summary>
        /// Gets or sets the arc identifier.
        /// </summary>
        /// <value>The arc identifier.</value>
        public int? ArcId { get; set; }
        /// <summary>
        /// Gets or sets the quadrant identifier.
        /// </summary>
        /// <value>The quadrant identifier.</value>
        public int? QuadrantId { get; set; }
        /// <summary>
        /// Gets or sets the tag identifier.
        /// </summary>
        /// <value>The tag identifier.</value>
        public int? TagId { get; set; }
    }
}
