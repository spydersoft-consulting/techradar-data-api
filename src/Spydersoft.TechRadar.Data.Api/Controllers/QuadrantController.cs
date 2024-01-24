using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Services;

namespace Spydersoft.TechRadar.Data.Api.Controllers
{
    /// <summary>
    /// Class QuadrantController.
    /// Implements the <see cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" /></summary>
    /// <seealso cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" />
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