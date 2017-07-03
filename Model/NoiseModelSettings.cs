using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    public class NoiseModelSettings
    /// <summary>
    /// Class to store Noise Model Settings
    ///</summary>
    {
        public NoiseModelSettings()
        {
            GridHeight = 4;
            GridSize = 10;
            SpatialReference = null;
            MinimumValue = 35;
        }

        public int GridSize { get; set; }
        public int GridHeight { get; set; }
        public esriGeometry.SpatialReference SpatialReference{ get; set; }

        public int MinimumValue { get; set; }
    }
}
