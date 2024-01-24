using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Spydersoft.TechRadar.Data.Api.Models.Dto;
using Spydersoft.TechRadar.Data.Api.Models.RadarViewObjects;
using Spydersoft.TechRadar.Data.Api.Services;

namespace Spydersoft.TechRadar.Data.Api.Controllers
{
    /// <summary>
    /// Class RadarDataController.
    /// Implements the <see cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" />
    /// </summary>
    /// <seealso cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" />
    public class RadarDataController : DataControllerBase
    {
        /// <summary>
        /// The radar service
        /// </summary>
        private readonly IRadarService _radarService;

        /// <summary>
        /// The tag service
        /// </summary>
        private readonly ITagService _tagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadarDataController" /> class.
        /// </summary>
        /// <param name="radarService">The radar service.</param>
        /// <param name="tagService">The tag service.</param>
        public RadarDataController(IRadarService radarService, ITagService tagService) : base()
        {
            _radarService = radarService;
            _tagService = tagService;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>ActionResult&lt;List&lt;Radar&gt;&gt;.</returns>
        [HttpGet("")]
        public ActionResult<List<Radar>> Get()
        {
            return _radarService.GetRadarList();
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <returns>ActionResult&lt;RadarData&gt;.</returns>
        [HttpGet("{id}")]
        public ActionResult<RadarData?> Get(int id, [FromQuery] FilterParameters filterParameters)
        {
            return _radarService.LoadRadarData(id, filterParameters);
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult&lt;List&lt;SimpleTag&gt;&gt;.</returns>
        [HttpGet("{id}/tags")]
        public ActionResult<List<ItemTag>> GetTags(int id)
        {
            return _tagService.GetAllRadarTags(id);
        }
    }
}