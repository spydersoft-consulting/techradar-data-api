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