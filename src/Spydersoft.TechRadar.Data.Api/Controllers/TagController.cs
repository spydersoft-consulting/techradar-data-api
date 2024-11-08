using Microsoft.AspNetCore.Authorization;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Services;

namespace Spydersoft.TechRadar.Data.Api.Controllers
{
    /// <summary>
    /// Class TagController.
    /// Implements the <see cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" />
    /// </summary>
    /// <seealso cref="Spydersoft.TechRadar.Data.Api.Controllers.DataControllerBase" />
    [Authorize()]
    public class TagController : EditControllerBase<Tag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagController" /> class.
        /// </summary>
        /// <param name="radarDataItemService">The radar data item service.</param>
        public TagController(IRadarDataItemService radarDataItemService) : base(radarDataItemService)
        {
        }
    }
}