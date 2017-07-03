using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ESRI.ArcGIS.Client;
using esriGeometry = ESRI.ArcGIS.Client.Geometry;
using esriSymbols = ESRI.ArcGIS.Client.Symbols;

using NoiseMapper.Model;

namespace NoiseMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graphic _lastGraphic;

        public MainWindow()
        {
            InitializeComponent();
            this.CreateStandardRenderer();
        }

        private void map_Loaded(object sender, RoutedEventArgs e)
        {
            Editor editor = LayoutRoot.Resources["editor"] as Editor;
            editor.Map = map;

            featureDataGrid.Map = map;

            GraphicsLayer sourcesGraphicsLayer = map.Layers["sources"] as GraphicsLayer;
            featureDataGrid.GraphicsLayer = sourcesGraphicsLayer;

        }

        private void CreateStandardRenderer()
        {
            ClassBreaksRenderer resultsClassBreaksRenderer = new ClassBreaksRenderer();
            resultsClassBreaksRenderer.Field = "LDW";

            Color[] standardColors =  
            {
                Color.FromRgb(159,251,136),
                Color.FromRgb(0,128,0),
                Color.FromRgb(0,102,51),
                Color.FromRgb(255,255,0),
                Color.FromRgb(204,119,34),
                Color.FromRgb(255,165,0),
                Color.FromRgb(227,66,52),
                Color.FromRgb(132,24,57),
                Color.FromRgb(222,180,203),
                Color.FromRgb(0,0,255),
                Color.FromRgb(0,49,83)
            };

            // standard range from 35 do 85 dB
            double minValue = 30;
            for (int iClass = 0; iClass < 11; iClass++)
            {
                ClassBreakInfo classBreakInfo = new ClassBreakInfo();

                // Define the minimum and maximum values for the numeric grouping of the Field defined as the
                classBreakInfo.MinimumValue = minValue + iClass * 5;
                if (iClass != 10)
                {
                    classBreakInfo.MaximumValue = minValue + (iClass + 1) * 5;
                    // Add an informational description and label for the group.
                    classBreakInfo.Description = string.Format("{0} - {1}",
                        classBreakInfo.MinimumValue, classBreakInfo.MaximumValue);
                }
                else
                {
                    classBreakInfo.MaximumValue = 999;
                    classBreakInfo.Description = string.Format("> {0}",
                        classBreakInfo.MinimumValue);
                }

                classBreakInfo.Label = classBreakInfo.Description;


                // Each ClassBreakInfo grouping needs a symbol to display
                esriSymbols.SimpleFillSymbol simpleFillSymbol = new esriSymbols.SimpleFillSymbol()
                {
                    BorderThickness = 0,
                    Fill = new SolidColorBrush(standardColors[iClass])
                };

                classBreakInfo.Symbol = simpleFillSymbol;

                // Add the ClassBreaksInfo information to the Classes (grouping) of the ClassBreaksRenderer
                resultsClassBreaksRenderer.Classes.Add(classBreakInfo);
            }
            GraphicsLayer graphicsLayer = map.Layers["results"] as GraphicsLayer;
            graphicsLayer.Renderer = resultsClassBreaksRenderer;
        }


        #region CHANGING BASEMAP
        private void basemapComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem.Tag == null)
                return;

            // Find and remove the current basemap layer from the map
            if (map == null)
                return;

            // deleting existing basemap(s)
            while (map.Layers.Contains(map.Layers["basemap"]))
            {
                map.Layers.Remove(map.Layers["basemap"]);
            }

            // adding new basemap(s)
            foreach (var tag in selectedItem.Tag.ToString().Split(';'))
            {
                // Create a new basemap layer
                var newBasemap = new ArcGISTiledMapServiceLayer();

                // Set the ServiceUri with the url defined for the ComboBoxItem's Tag
                newBasemap.Url = tag;

                // Give the layer the same ID so it can still be found with the code above
                newBasemap.ID = "basemap";

                // Insert the new basemap layer as the first (bottom) layer in the map
                map.Layers.Insert(0, newBasemap);
            }
        }
        #endregion


        #region EVENTS
        // update from different thread
        private void OnProgressIncremented(object sender, int progress)
        {
        }

        private void cancelEditsButton_Click(object sender, RoutedEventArgs e)
        {
            Editor editor = LayoutRoot.Resources["editor"] as Editor;
            foreach (GraphicsLayer graphicsLayer in editor.GraphicsLayers)
            {
                if (graphicsLayer is FeatureLayer)
                {
                    FeatureLayer featureLayer = graphicsLayer as FeatureLayer;
                    if (featureLayer.HasEdits)
                        featureLayer.Update();
                }
            }
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {

            this.ClearResults();

            var _ = RunCalcAsync();
        }

        private void FeatureLayer_MouseLeftButtonUp(object sender, GraphicMouseButtonEventArgs e)
        {
            if (_lastGraphic != null)
                _lastGraphic.UnSelect();

            e.Graphic.Select();
            if (e.Graphic.Selected)
                featureDataGrid.ScrollIntoView(e.Graphic, null);

            _lastGraphic = e.Graphic;

        }
        #endregion


        #region METHODS
        private void ClearResults()
        {
            GraphicsLayer resultsGraphicsLayer = map.Layers["results"] as GraphicsLayer;
            resultsGraphicsLayer.Graphics.Clear();
        }

        private async Task RunCalcAsync()
        {
            this.IndeterminatedProgressBar();

            int gridSize = CalcGridSize();

            NoiseModelSettings noiseModelSettings = new NoiseModelSettings();
            noiseModelSettings.GridSize = gridSize;
            noiseModelSettings.GridHeight = int.Parse(gridHeightTextBox.Text);
            noiseModelSettings.SpatialReference = map.SpatialReference;
            noiseModelSettings.MinimumValue = 30;

            NoiseModel noiseModel = new NoiseModel(noiseModelSettings);

            noiseModel.SourceCollection = ImportSources("sources");
            noiseModel.CalcAreaCollection = ImportCalcAreas("calcAreas");

            await Task.Run(() =>
            {
                noiseModel.RunCalc();
            });

            List<Graphic> graphicCollection = new List<Graphic>(noiseModel.CalcPointCollection.Count);

            foreach (var calcPoint in noiseModel.CalcPointCollection)
            {
                if (calcPoint.LDW >= noiseModelSettings.MinimumValue)
                {
                    Graphic graphic = new Graphic()
                    {
                        Geometry = calcPoint.Tile,
                        Attributes = { { "LDW", calcPoint.LDW } }
                    };
                    graphicCollection.Add(graphic);
                }
            }

            GraphicsLayer resultsGraphicsLayer = map.Layers["results"] as GraphicsLayer;
            resultsGraphicsLayer.Graphics.AddRange(graphicCollection);

            this.HideProgressBar();
        }

        private int CalcGridSize()
        {
            // grid points to grid size converter
            int gridCount = int.Parse(gridCountComboBox.Text.ToString().Replace(" ", string.Empty));

            var calcAreaFeatureLayer = map.Layers["calcAreas"] as FeatureLayer;
            double gridArea = 0;
            foreach (var feature in calcAreaFeatureLayer)
            {
                gridArea += esriGeometry.Euclidian.Area(feature.Geometry as esriGeometry.Polygon);
            }

            int gridSize = (int)Math.Round(Math.Sqrt(gridArea / gridCount));

            return gridSize;
        }

        private void HideProgressBar()
        {
            progressBar.Opacity = 0;
            progressBar.IsIndeterminate = false;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ShowProgressBar()
        {
            progressBar.Visibility = System.Windows.Visibility.Visible;
        }

        private void IndeterminatedProgressBar()
        {
            progressBar.Opacity = 1;
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = System.Windows.Visibility.Visible;
        }


        // IMPORT MODEL OBJECTS
        // --------------------
        // adding calculation areas
        private List<CalcArea> ImportCalcAreas(string featureLayerID)
        {
            var calcAreaFeatureLayer = map.Layers["calcAreas"] as FeatureLayer;
            List<CalcArea> calcAreaCollection = new List<CalcArea>(calcAreaFeatureLayer.Count());
            CalcArea calcArea;

            foreach (var feature in calcAreaFeatureLayer)
            {
                calcArea = new CalcArea(feature.Geometry as esriGeometry.Polygon);

                if (feature.Attributes.Keys.Contains("Id") && feature.Attributes["Id"] != null)
                    calcArea.Id = (int)feature.Attributes["Id"];

                if (feature.Attributes.Keys.Contains("Name") && feature.Attributes["Name"] != null)
                    calcArea.Name = (string)feature.Attributes["Name"];

                calcAreaCollection.Add(calcArea);
            }
            return calcAreaCollection;
        }

        // adding sources
        private List<PointSource> ImportSources(string featureLayerID)
        {
            var sourceFeatureLayer = map.Layers["sources"] as FeatureLayer;
            List<PointSource> sourceCollection = new List<PointSource>(sourceFeatureLayer.Count());
            PointSource source;

            foreach (var feature in sourceFeatureLayer)
            {
                source = new PointSource(feature.Geometry as esriGeometry.MapPoint);

                if (feature.Attributes.Keys.Contains("Id") && feature.Attributes["Id"] != null)
                    source.Id = (int)feature.Attributes["Id"];

                if (feature.Attributes.Keys.Contains("Name") && feature.Attributes["Name"] != null)
                    source.Name = (string)feature.Attributes["Name"];

                if (feature.Attributes.Keys.Contains("Height") && feature.Attributes["Height"] != null)
                {
                    //source.Height = (double)feature.Attributes["Height"];
                    source.Point.Z = (double)feature.Attributes["Height"];
                }

                if (feature.Attributes.Keys.Contains("LWA") && feature.Attributes["LWA"] != null)
                    source.LWA = (double)feature.Attributes["LWA"];

                sourceCollection.Add(source);
            }
            return sourceCollection;
        }
        #endregion

    }
}
