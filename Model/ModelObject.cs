using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
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
