using Spydersoft.TechRadar.Api.Data;
using Spydersoft.TechRadar.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class RadarItemControllerBase.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize(Policy = "Writers")]
    public class EditControllerBase<TRadarDataItem> : DataControllerBase where TRadarDataItem : class, IRadarDataItem
    {
        /// <summary>
        /// Gets the radar data item service.
        /// </summary>
        /// <value>The radar data item service.</value>
        protected IRadarDataItemService RadarDataItemService { get;  }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataControllerBase" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <remarks>This class provides easy access to data and services for the TechRadar.</remarks>
        public EditControllerBase(IRadarDataItemService service) : base()
        {
            RadarDataItemService = service;
        }

        /// <summary>
        /// Gets the item for the given <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id value.</param>
        /// <returns>ActionResult&lt;RadarArc&gt;.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public ActionResult<TRadarDataItem?> Get(int id)
        {
            return RadarDataItemService.GetRadarDataItem<TRadarDataItem>(id);
        }

        /// <summary>
        /// Create a new Arc
        /// </summary>
        /// <param name="value">The value.</param>
        [HttpPost("")]
        public void Post([FromBody] TRadarDataItem value)
        {
            RadarDataItemService.SaveRadarDataItem(value, User);
        }

        /// <summary>
        /// Update an existing arc
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] TRadarDataItem value)
        {
            value.Id = id;
            RadarDataItemService.SaveRadarDataItem(value, User);
        }
    }
}