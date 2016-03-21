﻿using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ptv.XServer.Controls.Map.Layers;
using Ptv.XServer.Controls.Map.Tools;

namespace Ptv.XServer.Controls.Map.Canvases
{
    /// <summary> The main canvas which holds paintable elements for a map. </summary>
    public abstract class MapCanvas : Canvas, IWeakEventListener
    {
        #region constructor
        /// <summary> Initializes a new instance of the <see cref="MapCanvas"/> class. Stores the parent map and adds
        /// listeners to the map viewport changed events. </summary>
        /// <param name="mapView"> The instance of the parent map. </param>
        protected MapCanvas(MapView mapView)
        {
            MapView = mapView;
            ViewportBeginChangedWeakEventManager.AddListener(mapView, this);
            ViewportEndChangedWeakEventManager.AddListener(mapView, this);
            ViewportWhileChangedWeakEventManager.AddListener(mapView, this);
        }
        #endregion

        #region properties
        /// <summary> Gets the parent map instance. </summary>
        /// <value> The parent map. </value>
        public MapView MapView { get; private set; }

        /// <summary> Gets or sets the <see cref="CanvasCategory"/> of the canvas. The canvas category defines the z
        /// order of the canvas in the map.</summary>
        /// <value> The canvas category. </value>
        public CanvasCategory CanvasCategory { get; set; }
        #endregion

        #region disposal
        /// <summary> Disposes the map canvas. During disposal the children of the canvas are removed and the viewport
        /// changed events are disconnected. </summary>
        public virtual void Dispose()
        {
            if (Parent is Canvas)
            {
                ((Canvas)Parent).Children.Remove(this);
            }

            ViewportBeginChangedWeakEventManager.RemoveListener(MapView, this);
            ViewportWhileChangedWeakEventManager.RemoveListener(MapView, this);
            ViewportEndChangedWeakEventManager.RemoveListener(MapView, this);
        }
        #endregion

        #region public methods

        /// <summary>
        /// Converts a geographic point to a canvas coordinate.
        /// </summary>
        /// <param name="geoPoint">The geographic point.</param>
        /// <param name="spatialReferenceId">The spatial reference identifier.</param>
        /// <returns> The transformed canvas coordinate. </returns>
        public Point GeoToCanvas(Point geoPoint, string spatialReferenceId)
        {
            return PtvMercatorToCanvas(GeoTransform.Transform(geoPoint, spatialReferenceId, "PTV_MERCATOR"));
        }

        /// <summary> Converts a geographic point to a canvas point. </summary>
        /// <param name="geoPoint"> The geographic point. </param>
        /// <returns> The canvas point. </returns>
        public Point GeoToCanvas(Point geoPoint)
        {
            return PtvMercatorToCanvas(GeoTransform.WGSToPtvMercator(geoPoint));
        }

        /// <summary> Converts a canvas point to a geographic point. </summary>
        /// <param name="canvasPoint"> The canvas point. </param>
        /// <returns> The geographic point. </returns>
        public Point CanvasToGeo(Point canvasPoint)
        {
            return GeoTransform.PtvMercatorToWGS(CanvasToPtvMercator(canvasPoint));
        }

        /// <summary> Updates the map content. The map content consists of all elements of the canvas. This method is
        /// for example triggered when the viewport changes. </summary>
        /// <param name="updateMode"> The update mode. This mode tells which kind of change is to be processed by the
        /// update call. </param>
        public abstract void Update(UpdateMode updateMode);
        #endregion

        #region IWeakEventListener Members
        /// <inheritdoc/>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(ViewportBeginChangedWeakEventManager))
            {
                Update(UpdateMode.BeginTransition);
                return true;
            }
            if (managerType == typeof(ViewportWhileChangedWeakEventManager))
            {
                Update(UpdateMode.WhileTransition);
                return true;
            }
            if (managerType == typeof(ViewportEndChangedWeakEventManager))
            {
                Update(UpdateMode.EndTransition);
                return true;
            }
            return false;
        }
        #endregion

        #region protected methods
        /// <summary> This method implements a transformation from canvas coordinates to logical coordinates. </summary>
        /// <param name="canvasPoint"> The canvas point. </param>
        /// <returns> The logical point. </returns>
        protected abstract Point CanvasToPtvMercator(Point canvasPoint);

        /// <summary> This method implements a transformation from logical coordinates to canvas coordinates. </summary>
        /// <param name="mercatorPoint"> The Mercator point. </param>
        /// <returns> The canvas point. </returns>
        protected abstract Point PtvMercatorToCanvas(Point mercatorPoint);
        #endregion
    }
    
    /// <summary> Indicates the mode of the update. </summary>
    public enum UpdateMode
    {
        /// <summary> Called for initial insert or refresh on the layer. </summary>
        Refresh,

        /// <summary> Called when a transition begins. </summary>
        BeginTransition,

        /// <summary> Called during a transition. </summary>
        WhileTransition,

        /// <summary> Called after a transition (Current-xxx-Values == Final-xxx-values). </summary>
        EndTransition
    }

    /// <summary>
    /// The enumeration defines the category of map canvases. The categories specify the primary z-order of canvases on
    /// the map. All canvases within the same category are grouped according to their index in the layer collection.
    /// </summary>
    public enum CanvasCategory
    {
        /// <summary> Category for the base map (i.e. xServer/Bing) content. </summary>
        BaseMap,

        /// <summary> Category for the user content. </summary>
        Content,

        /// <summary> Category for the labels of the user content. </summary>
        ContentLabels,

        /// <summary> Category for the selected objects. </summary>
        SelectedObjects,

        /// <summary> Category for top-most objects. </summary>
        TopMost
    }

    /// <summary>
    /// A canvas holds the graphic items of a map. One or more canvases are used for a layer. There are two main types
    /// of canvases: A <see cref="WorldCanvas"/>, whose elements have positions and dimensions in world Mercator units
    /// and a <see cref="ScreenCanvas"/>, whose elements have positions and dimensions in screen (pixel) units. By
    /// using multiple canvases, the elements for different layers can interleave for different
    /// <see cref="CanvasCategory"/> types. 
    /// </summary>
    [CompilerGenerated]
    public class NamespaceDoc
    {
        // NamespaceDoc is used for providing the root documentation for Ptv.Components.Projections
        // hint taken from http://stackoverflow.com/questions/156582/namespace-documentation-on-a-net-project-sandcastle
    }
    
    /// <summary>
    /// Canvas with screen coordinates. Elements of the canvas have an absolute dimension (= size in pixels) but have
    /// to be repositioned whenever the viewport changes.
    /// </summary>
    public abstract class ScreenCanvas : MapCanvas
    {
        #region constructor
        /// <summary> Initializes a new instance of the <see cref="ScreenCanvas"/> class. </summary>
        /// <param name="mapView"> The instance of the parent map. </param>
        protected ScreenCanvas(MapView mapView) : this(mapView, true)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="ScreenCanvas"/> class. If the second parameter
        /// <paramref name="addToMap"/> is set to true, the new screen canvas is added to the parent map.</summary>
        /// <param name="mapView"> The instance of the parent map. </param>
        /// <param name="addToMap"> Indicates that the canvas should be inserted to the parent map immediately. </param>
        protected ScreenCanvas(MapView mapView, bool addToMap)
            : base(mapView)
        {
            if (addToMap)
            {
                mapView.TopPaneCanvas.Children.Add(this);
            }
        }
        #endregion

        #region public methods
        /// <inheritdoc/>  
        protected override Point CanvasToPtvMercator(Point canvasPoint)
        {
            return MapView.CanvasToPtvMercator(this, canvasPoint);
        }

        /// <inheritdoc/>  
        protected override Point PtvMercatorToCanvas(Point mercatorPoint)
        {
            var geoCanvasPoint = new Point(
                (mercatorPoint.X + ((1.0 / MapView.ZoomAdjust) * (MapView.LogicalSize / 2))) * (MapView.ZoomAdjust / MapView.LogicalSize) * MapView.ReferenceSize,
                (-mercatorPoint.Y + ((1.0 / MapView.ZoomAdjust) * (MapView.LogicalSize / 2))) * (MapView.ZoomAdjust / MapView.LogicalSize) * MapView.ReferenceSize);

            return MapView.GeoCanvas.RenderTransform.Transform(geoCanvasPoint);
        }
        #endregion
    }

    /// <summary>
    /// Canvas with world coordinates. Elements of the canvas do not have to be repositioned when the viewport changes,
    /// but have world-dimension (= size in Mercator units).
    /// </summary>
    public abstract class WorldCanvas : MapCanvas
    {
        #region private variables
        /// <summary> Transformation instance of the canvas. This variable is used to transform coordinates from one
        /// format to another. </summary>
        private readonly Transform canvasTransform;
        #endregion

        #region constructor
        /// <summary> Initializes a new instance of the <see cref="WorldCanvas"/> class. </summary>
        /// <param name="mapView"> The instance of the parent map. </param>
        protected WorldCanvas(MapView mapView) : this(mapView, true)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="WorldCanvas"/> class. If the parameter
        /// <paramref name="addToMap"/> is set to true, the canvas is added to the parent map. </summary>
        /// <param name="mapView"> The instance of the parent map. </param>
        /// <param name="addToMap"> Indicates that the canvas should inserted to the parent map immediately. </param>
        protected WorldCanvas(MapView mapView, bool addToMap)
            : base(mapView)
        {
            canvasTransform = TransformFactory.CreateTransform(SpatialReference.PtvMercatorInvertedY);
            InitializeTransform();

            if (addToMap)
            {
                mapView.GeoCanvas.Children.Add(this);
            }
        }
        #endregion

        #region public methods
        /// <summary> Initializes the transformation instance which is needed to transform coordinates from one format
        /// to another one. </summary>
        public virtual void InitializeTransform()
        {
            RenderTransform = canvasTransform;
        }
        #endregion

        #region protected methods
        /// <inheritdoc/>  
        protected override Point CanvasToPtvMercator(Point canvasPoint)
        {
            return MapView.CanvasToPtvMercator(this, canvasPoint);
        }

        /// <inheritdoc/>  
        protected override Point PtvMercatorToCanvas(Point mercatorPoint)
        {
            return (RenderTransform == canvasTransform) ? 
                new Point(mercatorPoint.X + MapView.OriginOffset.X, -mercatorPoint.Y + MapView.OriginOffset.Y) :
                RenderTransform.Transform(canvasTransform.Inverse.Transform(
                    new Point(mercatorPoint.X + MapView.OriginOffset.X, -mercatorPoint.Y + MapView.OriginOffset.Y)));
        }

        #endregion
    }
}
