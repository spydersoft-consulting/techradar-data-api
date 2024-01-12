using Spydersoft.TechRadar.Api.Data;
using Spydersoft.TechRadar.Api.Services;

namespace Spydersoft.TechRadar.Api.Controllers
{
    /// <summary>
    /// Class QuadrantController.
    /// Implements the <see cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" /></summary>
    /// <seealso cref="Spydersoft.TechRadar.Api.Controllers.DataControllerBase" />
    public class QuadrantController : EditControllerBase<Quadrant>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadrantController" /> class.
        /// </summary>
        /// <param name="radarDataItemService">The radar data item service.</param>
        public QuadrantController(IRadarDataItemService radarDataItemService) : base(radarDataItemService)
        {
        }
    }
}