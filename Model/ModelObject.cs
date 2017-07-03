using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    /// <summary>
    /// Parent of all geometry model objects
    ///</summary>
    public abstract class ModelObject
    {
        public ModelObject(esriGeometry.Geometry geometry)
        {
            Geometry = geometry;
        }

        #region PROPERTIES

        public esriGeometry.Geometry Geometry { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        #endregion   
    }
}
