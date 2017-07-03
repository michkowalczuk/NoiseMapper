using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    /// <summary>
    /// Parent of all point-type model objects
    ///</summary>
    public abstract class PointModelObject : ModelObject
    {
        public PointModelObject(esriGeometry.MapPoint mapPoint)
            :base(mapPoint) {}

        #region PROPERTIES

        public esriGeometry.MapPoint Point
        {
            get
            {
                return base.Geometry as esriGeometry.MapPoint;
            }
        }

        public double X
        {
            get { return this.Point.X; }
        }

        public double Y
        {
            get { return this.Point.Y; }
        }

        public double Z
        {
            get { return this.Point.Z; }
        }

        #endregion
    }
}
