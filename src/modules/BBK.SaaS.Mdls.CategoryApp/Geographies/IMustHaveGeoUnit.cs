using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Category.GeoGeographiesUnits
{
	/// <summary>
    /// This interface is implemented entities those must have an <see cref="GeoUnit"/>.
    /// </summary>
    public interface IMustHaveGeoUnit
    {
        /// <summary>
        /// <see cref="GeoUnit"/>'s Id which this entity belongs to.
        /// </summary>
        long GeoUnitId { get; set; }
    }
}
