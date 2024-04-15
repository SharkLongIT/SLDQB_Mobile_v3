using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Category.Geographies
{
	/// <summary>
    /// This interface is implemented entities those may have an <see cref="GeoUnit"/>.
    /// </summary>
    public interface IMayHaveGeoUnit
    {
        /// <summary>
        /// <see cref="GeoUnit"/>'s Id which this entity belongs to.
        /// Can be null if this entity is not related to any <see cref="GeoUnit"/>.
        /// </summary>
        long? GeoUnitId { get; set; }
    }
}
