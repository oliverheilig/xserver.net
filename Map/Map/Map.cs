﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Reflection;

using Ptv.XServer.Controls.Map.Layers;
using Ptv.XServer.Controls.Map.Tools;
using Ptv.XServer.Controls.Map.Gadgets;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Layers.Untiled;
using Ptv.XServer.Controls.Map.TileProviders;

namespace Ptv.XServer.Controls.Map
{
    /// <summary>
    /// <para>
    /// The <see cref="Map"/> namespace contains the classes to visualize your data in an interactive map.
    /// </para>
    /// <para>
    /// The main type is the <see cref="WpfMap"/>, which can be added to a XAML user control. An alternative
    /// <see cref="FormsMap"/> can be used for easy WinForms integration. Both controls inherit from
    /// <see cref="Map"/> and implement <see cref="IMap"/>. The Map type can be used to build up a
    /// customized map control.
    /// </para>
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
        // NamespaceDoc is used for providing the root documentation for Ptv.Components.Projections
        // hint taken from http://stackoverflow.com/questions/156582/namespace-documentation-on-a-net-project-sandcastle
    }

    /// <summary> This class represents the basic map control without any gadgets. It is used as the WPFMap's base class,
    /// and as a proxy for the WinForms map control. So, it is possible to provide a common implementation for both
    /// environments, i.e. WPF and WinForms.</summary>
    public class Map : UserControl, IMap, IToolTipManagement, IDisposable
    {
        /// <summary> Initializes static members of the <see cref="Map"/> class. </summary>
        static Map()
        {
            try
            {
                // the evil hack to avoid induced GC.Collect(), see
                // http://stackoverflow.com/questions/7331735/gc-is-forced-when-working-with-small-images-4k-pixel-data
                if (GlobalOptions.MemoryPressureMode == MemoryPressureMode.Disable ||
                    GlobalOptions.MemoryPressureMode == MemoryPressureMode.Automatic && System.IntPtr.Size == 8)
                {
                    typeof (BitmapImage).Assembly.GetType("MS.Internal.MemoryPressure")
                        .GetField("_totalMemory", BindingFlags.NonPublic | BindingFlags.Static)
                        .SetValue(null, Int64.MinValue/2);
                }
            }
            catch { }
        }

        #region events
        /// <inheritdoc/>  
        public event EventHandler ViewportBeginChanged
        {
            add { mapView.ViewportBeginChanged += value; }
            remove { mapView.ViewportBeginChanged -= value; }
        }

        /// <inheritdoc/>  
        public event EventHandler ViewportEndChanged
        {
            add { mapView.ViewportEndChanged += value; }
            remove { mapView.ViewportEndChanged -= value; }
        }

        /// <inheritdoc/>  
        public event EventHandler ViewportWhileChanged
        {
            add { mapView.ViewportWhileChanged += value; }
            remove { mapView.ViewportWhileChanged -= value; }
        }
        #endregion //events

        #region private variables
        /// <summary> Credentials for xMapServer. </summary>
        private string xmapCredentials = "";
        /// <summary> Url pointing on the xMapServer in use. </summary>
        private string xmapUrl = "";
        /// <summary> The style profile of the xMapServer base map. </summary>
        private string xmapStyle = "";
        /// <summary> The textblock which displays the hint for the missing xMap url. </summary>
        protected TextBlock copyrightHintText;
        /// <summary> Documentation in progress... </summary>
        private string xMapCopyright;
        #endregion

        #region protected variables
        /// <summary> MapView to be displayed in the map. </summary>
        protected MapView mapView = new MapView { Name = "Map" };
        #endregion

        #region public variables
        /// <summary> Dictionary holding all existing gadgets like Scale gadget, zoom slider etc. </summary>
        public ObservableDictionary<GadgetType, IGadget> Gadgets = new ObservableDictionary<GadgetType, IGadget>();

        /// <summary> Gets or sets the default theme. The default theme is initialized in the WpfMap.xaml file and used
        /// by <see cref="UseDefaultTheme"/>. </summary>
        public ResourceDictionary DefaultThemeResources { get; set; }
        #endregion

        #region constructor
        /// <summary> Initializes a new instance of the <see cref="Map"/> class. By default, the map uses animation
        /// and the scale is shown in km. </summary>
        public Map()
        {
            Loaded += Map_Loaded;
            
            Layers = new LayerCollection();

            UseAnimation = true;
            InvertMouseWheel = false;
            MouseWheelSpeed = .5;
            UseMiles = false;
            
            MouseDoubleClickZoom = true;
            MouseDragMode = DragMode.SelectOnShift;
            CoordinateDiplayFormat = CoordinateDiplayFormat.Degree;

            DefaultThemeResources = new ResourceDictionary { Source = new Uri("Ptv.XServer.Controls.Map;component/Resources/Themes/PTVDefault.xaml", UriKind.Relative) };
            UseDefaultTheme = true;

            InitializeToolTipManagement();
        }
        #endregion

        #region event handling
        /// <summary> Event handler for a successful load of the map. Shows the map in a grid. </summary>
        /// <param name="sender"> Sender of the Loaded event. </param>
        /// <param name="e"> Event parameters. </param>
        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Map_Loaded;

            SetXMapUrlHint();

            ((Grid) Content).Children.Insert(0, mapView);

            foreach (var view in MapElementExtensions.FindChildren<MapView>(this))
                Layers.Register(view);
        }

        private void SetXMapUrlHint()
        {
            if (copyrightHintText != null)
                mapView.Layers.Children.Remove(copyrightHintText);

            if (!DesignTest.DesignMode || !string.IsNullOrEmpty(XMapUrl)) return;

            copyrightHintText = new TextBlock { Text = "Please insert a valid xMapServer URL to display a base map." };
            Panel.SetZIndex(copyrightHintText, 1024);
            copyrightHintText.TextWrapping = TextWrapping.Wrap;
            copyrightHintText.HorizontalAlignment = HorizontalAlignment.Center;
            copyrightHintText.VerticalAlignment = VerticalAlignment.Center;
            mapView.Layers.Children.Add(copyrightHintText);
        }

        /// <summary> Event indicating a change of the UseMiles property. It can be used to update the scale gadget. </summary>
        public event EventHandler UseMilesChanged;


        /// <summary> Active tool tip, may be null. </summary>
        private ToolTip toolTip;
        /// <summary> A timer for popping up the tool tips. </summary>
        private readonly DispatcherTimer toolTipTimer = new DispatcherTimer();
        // Strings which should be shown in the tooltip
        private readonly List<string> toolTipEntries = new List<string>();

        /// <inheritdoc/>
        bool IToolTipManagement.IsEnabled { get; set; }

        /// <inheritdoc/>
        public int ToolTipDelay { get; set; }

        /// <summary> 
        /// Stores the latest known position handled by method <see cref="StartToolTipTimer"/>. If the mouse position is outside the bounds
        ///  of the map control, its value is null. 
        /// </summary>
        private Point? LatestPosition { get; set; }

        /// <inheritdoc/>
        public double MaxPixelDistance { get; set; }

        private void InitializeToolTipManagement()
        {
            (this as IToolTipManagement).IsEnabled = true;

            ToolTipDelay = ToolTipService.GetInitialShowDelay(new Canvas());
            toolTipTimer.Tick += ShowToolTip;

            MaxPixelDistance = 10;

            MouseMove += (s, mouseEventArgs) => StartToolTipTimer(mouseEventArgs); // raised when the mouse pointer moves.
            MouseLeave += (s, mouseEventArgs) => StartToolTipTimer(); // raised when the mouse pointer leaves the bounds of the Map object.
        }

        /// <summary> Tests if the cursor position has changed. If so, stores the new position and triggers the tool tip timer. </summary>
        /// <param name="mouseEventArgs">Mouse event arguments, a null value represents an invalid position.</param>
        private void StartToolTipTimer(MouseEventArgs mouseEventArgs = null)
        {
            if (!(this as IToolTipManagement).IsEnabled)
                return;

            // test if position has changed
            var currentPosition = (mouseEventArgs == null) ? null : (Point?)mouseEventArgs.GetPosition(this);
            if ((LatestPosition.HasValue == currentPosition.HasValue) && (!currentPosition.HasValue || ((LatestPosition.Value - currentPosition.Value).Length <= 1e-4)))
                return;

            // clear previous tool tip
            ClearToolTip();

            // trigger update, if not any mouse button is pressed
            if (Mouse.LeftButton != MouseButtonState.Released || Mouse.MiddleButton != MouseButtonState.Released || Mouse.RightButton != MouseButtonState.Released) 
                return;

            LatestPosition = currentPosition;

            // flag indicating if a tool tip is to be shown. Initially, specified position must be valid.
            bool showToolTips = currentPosition.HasValue && (ToolTipDelay >= 0) && IsHitTestOK((Point)currentPosition);

            GetToolTipEntries();

            // disable or enable the tooltip timer, according to hasToolTip
            toolTipTimer.Stop();
            if (!showToolTips) return;

            toolTipTimer.Interval = TimeSpan.FromMilliseconds(ToolTipDelay);
            toolTipTimer.Start();
        }

        // Check if the mouse is not over a MapGadget control. Maybe this method needs to be extended for additional control types.
        private bool IsHitTestOK(Point currentPosition)
        {
            var test = VisualTreeHelper.HitTest(this, currentPosition);
            return ((test == null) || (test.VisualHit.FindAncestor<MapGadget>() == null));
        }


        /// <summary> Event handler: Tool tip timer has elapsed, tool tip is to be shown. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowToolTip(object sender, EventArgs e)
        {
            // stop the timer and clear any previous tool tips.
            toolTipTimer.Stop();
            ClearToolTip();

            // check if texts exist
            if (toolTipEntries.Count <= 0) return;

            StackPanel stackPanel = new StackPanel();

            bool isFirst = true;
            foreach (var toolTipEntry in toolTipEntries)
            {
                var label = new Label {Margin = new Thickness(1), Content = toolTipEntry};
                stackPanel.Children.Add(isFirst ? (UIElement) label : (new Border { BorderThickness = new Thickness(0, 1, 0, 0), BorderBrush = new SolidColorBrush(Colors.White), Child = label }));
                isFirst = false;
            }
            // create and show tool tip
            toolTip = new ToolTip
            {
                Content = stackPanel,
                IsOpen = true
            };
        }

        /// <summary> Removes the latest tool tip created by this layer. </summary>
        private void ClearToolTip()
        {
            if (toolTip == null) return;

            // close tool tip
            toolTip.IsOpen = false;
            toolTip = null;
        }
        
        private void GetToolTipEntries()
        {
            toolTipEntries.Clear();
            if (!LatestPosition.HasValue) return;

            foreach (var toolTipsEnumeration in Layers.OfType<IToolTips>().Select(layer => layer.Get((Point) LatestPosition, MaxPixelDistance)).Where(enumeration => enumeration != null)) 
                toolTipEntries.AddRange(toolTipsEnumeration);
        }




        /// <summary> Fires the UseMilesChanged event. </summary>
        private void FireUseMilesChanged()
        {
            if (UseMilesChanged != null)
                UseMilesChanged(this, EventArgs.Empty);
        }

        /// <summary> Flag to choose whether the metric or the mile based system is to be used for example for the scale display. </summary>
        private bool useMiles;
        /// <summary> Gets or sets a value indicating whether the scale is displayed in miles or it is displayed in km. </summary>
        public bool UseMiles
        {
            get { return useMiles; }
            set { useMiles = value; FireUseMilesChanged(); }
        }

        /// <summary> Flag indicating whether the default theme is to be used for gadget display. </summary>
        private bool bUseDefaultTheme;

        /// <summary><para> Gets or sets a value indicating whether the built-in PTV theme should be used, and thus
        /// overriding an optionally set style. The application theme can be set in the App.xaml. If no theme at all is
        /// set and the default PTV theme is not used either, the UI will look like the current Windows theme. </para>
        /// <para> See the <conceptualLink target="5e97e57f-ad50-4dda-af0b-e117af8c4fcd"/> topic for an example. </para></summary>
        public bool UseDefaultTheme
        {
            get { return bUseDefaultTheme; }
            set
            {
                bUseDefaultTheme = value;
                Resources.MergedDictionaries.Clear();

                if (bUseDefaultTheme)
                {
                    Resources.MergedDictionaries.Add(DefaultThemeResources);
                }
            }
        }

        /// <inheritdoc/>  
        public LayerCollection Layers { get; private set; }

        /// <inheritdoc/>  
        public string XMapCredentials
        {
            get { return xmapCredentials; }
            set
            {
                xmapCredentials = value;

                InitializeMapLayers();
            }
        }

        private void InitializeMapLayers()
        {
            Layers.RemoveXMapBaseLayers();

            if (string.IsNullOrEmpty(xmapUrl)) return;

            var xmapMetaInfo = new XMapMetaInfo(xmapUrl);
            if (!string.IsNullOrEmpty(xmapCredentials) && xmapCredentials.Contains(":"))
            {
                var usrpwd = xmapCredentials.Split(':');
                xmapMetaInfo.SetCredentials(usrpwd[0], usrpwd[1]);
            }
            Layers.InsertXMapBaseLayers(xmapMetaInfo);
        }

        /// <inheritdoc/>  
        public string XMapUrl
        {
            get { return xmapUrl; }
            set
            {        
                xmapUrl = value;
                InitializeMapLayers();

                SetXMapUrlHint();
            }
        }

        /// <inheritdoc/>  
        public string XMapStyle
        {
            get { return xmapStyle; }
            set
            {
                xmapStyle = value;

                if (Layers["Background"] != null)
                {
                    ((Layers["Background"] as TiledLayer).TiledProvider as XMapTiledProvider).CustomProfile =
                        xmapStyle != null ? xmapStyle + "-bg" : null;
                    (Layers["Background"] as TiledLayer).Refresh();
                }
                if (Layers["Labels"] != null)
                {
                    ((Layers["Labels"] as UntiledLayer).UntiledProvider as XMapTiledProvider).CustomProfile =
                        xmapStyle != null ? xmapStyle + "-fg" : null;
                    (Layers["Labels"] as UntiledLayer).Refresh();
                }
            }
        }

        /// <inheritdoc/>  
        public string XMapCopyright
        {
            get
            {
                if (string.IsNullOrEmpty(xMapCopyright) || xMapCopyright.Length < 3)
                    return "Please configure a valid copyright text!";
                return xMapCopyright;
            }
            set
            {
                xMapCopyright = value;
                Layers.UpdateXMapCoprightText(xMapCopyright);
                if (!Gadgets.ContainsKey(GadgetType.Copyright)) return;

                var mapGadget = Gadgets[GadgetType.Copyright] as MapGadget;
                if (mapGadget != null)
                    mapGadget.UpdateContent();
            }
        }

        /// <inheritdoc/>  
        public bool FitInWindow
        {
            get { return mapView.FitInWindow; }
            set { mapView.FitInWindow = value; }
        }

        /// <inheritdoc/>  
        public bool IsAnimating
        {
            get { return mapView.IsAnimating; }
        }

        /// <inheritdoc/>  
        public int MaxZoom
        {
            get { return mapView.MaxZoom; }
            set { mapView.MaxZoom = value; }
        }

        /// <inheritdoc/>  
        public int MinZoom
        {
            get { return mapView.MinZoom; }
            set { mapView.MinZoom = value; }
        }

        /// <inheritdoc/>  
        public double MetersPerPixel
        {
            get { return mapView.MetersPerPixel; }
        }

        /// <summary><para> Sets the current theme from a XAML file provided by the stream. The XAML file must contain a
        /// ResourceDictionary on the top level. See the attached XAML files in the demo project. </para>
        /// <para> See the <conceptualLink target="5e97e57f-ad50-4dda-af0b-e117af8c4fcd"/> topic for an example. </para></summary>
        /// <param name="stream"> The stream providing a XML file with a ResourceDictionary on the top level. </param>
        public void SetThemeFromXaml(System.IO.Stream stream)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add((ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream));
        }

        /// <summary> Set the center and zooming level of the map. </summary>
        /// <param name="point"> The center point. </param>
        /// <param name="zoom"> The zoom level. </param>
        public void SetMapLocation(Point point, double zoom)
        {
            SetMapLocation(point, zoom, "EPSG:4326");
        }

        /// <summary> Set the center and zooming level of the map. </summary>
        /// <param name="point"> The center point. </param>
        /// <param name="zoom"> The zoom level. </param>
        /// <param name="spatialReferenceId"> The spatial reference identifier. </param>
        public void SetMapLocation(Point point, double zoom, string spatialReferenceId)
        {
            var mercatorPoint = GeoTransform.Transform(point, spatialReferenceId, "PTV_MERCATOR");
            mapView.SetXYZ(mercatorPoint.X, mercatorPoint.Y, zoom, UseAnimation);
        }

        /// <summary> Gets or sets the zoom level of the map. </summary>
        public double Zoom
        {
            get { return mapView.FinalZoom; }
            set { mapView.SetZoom(value, UseAnimation); }
        }

        /// <inheritdoc/>  
        public void PrintMap(bool useScaling, string description)
        {
            // Open the print dialog.
            var print = new PrintDialog();
            if (print.ShowDialog() == false)
                return;

            // Initialize variables.
            var capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
            Transform oldTransform = null;

            if (useScaling)
            {
                // Set the transform object for scaling.
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / mapView.ActualWidth,
                                        capabilities.PageImageableArea.ExtentHeight / mapView.ActualHeight);
                oldTransform = mapView.LayoutTransform;
                mapView.LayoutTransform = new ScaleTransform(scale, scale);
            }

            // Set the size.
            var oldSize = new Size(mapView.ActualWidth, mapView.ActualHeight);
            var sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            mapView.Measure(sz);
            mapView.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

            // Print.
            mapView.Printing = true;
            mapView.UpdateLayout();
            print.PrintVisual(mapView, description);
            mapView.Printing = false;

            // Reset the old values.
            if (useScaling)
                mapView.LayoutTransform = oldTransform;
            mapView.Measure(oldSize);
            mapView.Arrange(new Rect(new Point(0, 0), oldSize));
        }
        #endregion

        #region IDisposable Members
        /// <summary> Disposal of the map. All layers are disposed. </summary>
        public void Dispose()
        {
            Layers.Dispose();
        }
        #endregion

        #region IMap Members
        /// <inheritdoc/>
        public Point MouseToGeo(MouseEventArgs e, string spatialReferenceId)
        {
            return RelToMapViewAsGeo(e.GetPosition(mapView), spatialReferenceId);
        }

        /// <inheritdoc/>
        public Point MouseToGeo(MouseEventArgs e)
        {
            return RelToMapViewAsGeo(e.GetPosition(mapView), "EPSG:4326");
        }

        /// <inheritdoc/>
        public Point RelToMapViewAsGeo(Point point, string spatialReferenceId)
        {
            var mercatorPoint = mapView.CanvasToPtvMercator(mapView, point);

            return mercatorPoint.GeoTransform("PTV_MERCATOR", spatialReferenceId);
        }

        /// <inheritdoc/>
        public Point RelToMapViewAsGeo(Point point)
        {
            return RelToMapViewAsGeo(point, "EPSG:4326");
        }

        /// <inheritdoc/>
        public Point GeoAsRelToMapView(Point point, string spatialReferenceId)
        {
            point.GeoTransform(spatialReferenceId, "PTV_MERCATOR");

            return mapView.PtvMercatorToCanvas(mapView, point);
        }

        /// <inheritdoc/>
        public Point GeoAsRelToMapView(Point point)
        {
            return GeoAsRelToMapView(point, "EPSG:4326");
        }

        /// <inheritdoc/>
        public MapRectangle GetEnvelope()
        {
            return GetEnvelope("EPSG:4326");
        }

        /// <inheritdoc/>
        public MapRectangle GetEnvelope(string spatialReferenceId)
        {
            MapRectangle rectangle = mapView.FinalEnvelope;
            return new MapRectangle(rectangle.SouthWest.GeoTransform("PTV_MERCATOR", spatialReferenceId), 
                                    rectangle.NorthEast.GeoTransform("PTV_MERCATOR", spatialReferenceId));
        }

        /// <inheritdoc/>
        public void SetEnvelope(MapRectangle rectangle)
        {
            SetEnvelope(rectangle, "EPSG:4326");
        }

        /// <inheritdoc/>
        public void SetEnvelope(MapRectangle rectangle, string spatialReferenceId)
        {
            var p1 = rectangle.SouthWest.GeoTransform(spatialReferenceId, "PTV_MERCATOR");
            var p2 = rectangle.NorthEast.GeoTransform(spatialReferenceId, "PTV_MERCATOR");

            mapView.SetEnvelope(new MapRectangle(p1, p2), UseAnimation);
        }

        /// <inheritdoc/>
        public MapRectangle GetCurrentEnvelope()
        {
            return GetCurrentEnvelope("EPSG:4326");
        }

        /// <inheritdoc/>
        public MapRectangle GetCurrentEnvelope(string spatialReferenceId)
        {
            MapRectangle rectangle = mapView.CurrentEnvelope;
            return new MapRectangle(rectangle.SouthWest.GeoTransform("PTV_MERCATOR", spatialReferenceId),
                                    rectangle.NorthEast.GeoTransform("PTV_MERCATOR", spatialReferenceId));
        }

        /// <inheritdoc/>  
        public bool UseAnimation { get; set; }

        /// <inheritdoc/>  
        public bool InvertMouseWheel { get; set; }

        /// <inheritdoc/>
        public double MouseWheelSpeed { get; set; }

        /// <inheritdoc/>
        public bool MouseDoubleClickZoom { get; set; }

        /// <inheritdoc/>
        public DragMode MouseDragMode { get; set; }

        /// <inheritdoc/>
        public CoordinateDiplayFormat CoordinateDiplayFormat { get; set; }

        /// <inheritdoc/>  
        public double ZoomLevel
        {
            get { return mapView.FinalZoom; }
            set { mapView.SetZoom(value, UseAnimation); }
        }

        /// <inheritdoc/>  
        public double Scale { get { return mapView.FinalScale; } }

        /// <inheritdoc/>  
        public Point Center
        {
            get { return GeoTransform.PtvMercatorToWGS(new Point(mapView.FinalX, mapView.FinalY)); }
            set
            {
                var p = GeoTransform.WGSToPtvMercator(value);
                mapView.SetXYZ(p.X, p.Y, mapView.FinalZoom, UseAnimation);
            }
        }

        /// <inheritdoc/>  
        public double CurrentZoomLevel { get { return mapView.CurrentZoom; } }

        /// <inheritdoc/>  
        public double CurrentScale { get { return mapView.CurrentScale; } }

        /// <inheritdoc/>  
        public Point CurrentCenter { get { return GeoTransform.PtvMercatorToWGS(new Point(mapView.CurrentX, mapView.CurrentY)); } }
        #endregion
    }

    /// <summary> Provides extensions for VisualTreeHelper. </summary>
    public static class VisualTreeHelperExtensions
    {
        /// <summary> Gets the parent of a depedency object </summary>
        /// <param name="obj">The dependency object to get the parent for</param>
        /// <returns>Parent of the dependency object</returns>
        public static DependencyObject GetParent(this DependencyObject obj)
        {
            if (obj == null) return null;

            var ce = obj as ContentElement;
            if (ce == null) return VisualTreeHelper.GetParent(obj);

            DependencyObject parent = ContentOperations.GetParent(ce);
            if (parent != null) return parent;

            var fce = ce as FrameworkContentElement;
            return fce != null ? fce.Parent : null;
        }

        /// <summary> Finds a specific ancestor of a dependency object. </summary>
        /// <typeparam name="T">Type to lookup</typeparam>
        /// <param name="obj">Dependency object to find the ancestor for.</param>
        /// <returns>Found ancestor or null.</returns>
        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            for (; obj != null; obj = GetParent(obj))
            {
                var objTest = obj as T;
                if (objTest != null) return objTest;
            }

            return null;
        }
    }

}
