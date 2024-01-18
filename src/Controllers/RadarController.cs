using Spydersoft.TechRadar.Api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Spydersoft.TechRadar.Api.Models;
using Spydersoft.TechRadar.Api.Models.Dto;
using Spydersoft.TechRadar.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class RadarController.
    /// Implements the <see cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    /// </summary>
    public class RadarController : EditControllerBase<Radar>
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
        /// Initializes a new instance of the <see cref="RadarController" /> class.
        /// </summary>
        /// <param name="radarService">The radar service.</param>
        /// <param name="tagService">The tag service.</param>
        /// <param name="radarDataItemService">The radar data item service.</param>
        public RadarController(IRadarService radarService, ITagService tagService, IRadarDataItemService radarDataItemService) : base(radarDataItemService)
        {
            _radarService = radarService;
            _tagService = tagService;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>ActionResult&lt;List&lt;Radar&gt;&gt;.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Radar>> Get()
        {
            return _radarService.GetRadarList();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ActionResult&lt;List&lt;RadarItem&gt;&gt;.</returns>
        [HttpGet("{id}/items")]
        public ActionResult<List<RadarItem>> GetItems(int id, [FromQuery] RadarQueryParameters parameters)
        {
            return _radarService.GetItems(id, parameters);
        }

        /// <summary>
        /// Gets the arcs.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult&lt;List&lt;RadarArc&gt;&gt;.</returns>
        [HttpGet("{id}/arcs")]
        public ActionResult<List<RadarArc>> GetArcs(int id)
        {
            return _radarService.GetArcs(id);
        }

        /// <summary>
        /// Gets the quadrants.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult&lt;List&lt;Quadrant&gt;&gt;.</returns>
        [HttpGet("{id}/quadrants")]
        public ActionResult<List<Quadrant>> GetQuadrants(int id)
        {
            return _radarService.GetQuadrants(id);
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult&lt;List&lt;Tag&gt;&gt;.</returns>
        [HttpGet("{id}/tags")]
        public ActionResult<List<ItemTag>> GetTags(int id)
        {
            return _tagService.GetAllRadarTags(id);
        }
    }
}