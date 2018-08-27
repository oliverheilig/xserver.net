﻿// This source file is covered by the LICENSE.TXT file in the root folder of the SDK.

using Ptv.XServer.Controls.Map.Layers.Untiled;
using System.Text;
using System.IO;
using System.Net;
using TinyJson;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ptv.XServer.Controls.Map.TileProviders;
using System.Windows;

namespace Ptv.XServer.Controls.Map.Layers.Xmap2
{
    /// <summary> Handles additional map objects like Feature Layer information. Map objects are used to show
    /// textual information extending the geographical content.
    /// Commonly this interface is used by providers to inform their corresponding layer about a new
    /// set of map objects which was determined during the common rendering process as a side-effect.</summary>
    public interface IXmap2ObjectInfos
    {
        /// <summary> Signals the listener (commonly a layer) about a new constellation of map objects. </summary>
        Action<IEnumerable<IMapObject>, Size> Update { get; set; }
    }

    /// <summary> Loads untiled bitmaps from a given xServer 2 URL, like
    /// http://xserver-2:40000/services/rs/XMap/renderMap. 
    /// Its main purpose is to get labels from xServer 2, which shows a proper rendering of textual objects
    /// independent from the current scale.
    /// By means of tiled access some unpleasant artifacts may occur when fractional rendering is used. 
    /// With untiled rendering this issue can be avoided.</summary>
    public class UntiledProvider : IUntiledProvider, IXmap2ObjectInfos
    {
        /// <summary>URL of the service which provides untiled access of a map image via JSON request. </summary>
        public string RequestUriString { get; set; }

        /// <summary> Function which returns the xToken needed for authentication in cloud based environments.</summary>
        public Func<string> XTokenFunc { get; set; }

        /// <summary>Function which returns a set of themes for which a map should be rendered. Examples are <em>labels</em>,
        /// but also Feature Layer themes like <em>Truck Attributes</em>.</summary>
        public Func<IEnumerable<string>> ThemesForRenderingFunc { get; set; }

        /// <summary>Function which returns a set of themes for which map object information should be calculated during
        /// the renderMap service request. Commonly, this set is restricted to Feature Layer themes like <em>Truck Attributes</em>.</summary>
        public Func<IEnumerable<string>> ThemesWithMapObjectsFunc { get; set; }

        /// <summary>Function which returns the time consideration scenario which should be used when the map is rendered and
        /// map objects are retrieved. Currently supported scenarios are
        /// <em>OptimisticTimeConsideration</em>, <em>SnapshotTimeConsideration</em> and <em>TimeSpanConsideration</em>. 
        /// For all other return values (including null string), no scenario is used and all time dependent features are not relevant.
        /// </summary>
        public Func<string> TimeConsiderationScenarioFunc { get; set; }

        /// <summary>For <em>SnapshotTimeConsideration</em> and <em>TimeSpanConsideration</em> it is necessary to define a reference
        /// time to determine which time dependent features should be active or not. The function returns a reference time with the following format:
        /// <c>yyyy-MM-ddTHH:mm:ss[+-]HH:mm</c>, for example <c>2018-08-05T04:00:00+02:00</c>. </summary>
        public Func<string> ReferenceTimeFunc { get; set; }

        /// <summary>Function which defines the time span (in seconds) which is added to the reference time 
        /// and needed for the <em>TimeSpanConsideration</em> scenario. </summary>
        public Func<double> TimeSpanFunc { get; set; }

        /// <summary>Function which indicates if the non-relevant Features should be shown or not.</summary>
        public Func<bool> ShowOnlyRelevantByTimeFunc { get; set; }

        /// <summary>Function which returns the used language, commonly needed for the language of textual messages provided
        /// by the theme <em>traffic incidents</em>. The language code is defined in BCP47, 
        /// for example <em>en</em>, <em>fr</em> or <em>de</em>. </summary>
        public Func<string> UserLanguageFunc { get; set; }

        /// <inheritdoc/>
        public Action<IEnumerable<IMapObject>, Size> Update { get; set; }

        /// <inheritdoc/>
        public Stream GetImageStream(double left, double top, double right, double bottom, int width, int height)
        {
            var request = WebRequest.Create(RequestUriString);
            request.Method = "POST";
            request.ContentType = "application/json";

            string xToken = XTokenFunc?.Invoke();
            if (!string.IsNullOrEmpty(xToken))
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("xtok:" + xToken));

            using (var stream = request.GetRequestStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(GetJsonRequest(left, top, right, bottom, width, height));
            }

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return null;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var responseObject = reader.ReadToEnd().FromJson<ResponseObject>();

                    IEnumerable<IMapObject> mapObjects = responseObject?.features?.Select(feature =>
                        (IMapObject) new MapObject(
                            feature.id,
                            feature.themeId,
                            new Point(feature.referencePixelPoint.x, feature.referencePixelPoint.y),
                            new Point(feature.referenceCoordinate.x, feature.referenceCoordinate.y),
                            () => feature.attributes?.Select(attribute =>
                                new KeyValuePair<string, string>(attribute.key, attribute.value))
                        ));
                    Update?.Invoke(mapObjects, new Size(width, height));

                    return (responseObject?.image != null)
                        ? new MemoryStream(Convert.FromBase64String(responseObject.image))
                        : null;
                }
            }
        }

        private string GetJsonRequest(double left, double top, double right, double bottom, int width, int height)
        {
            var mapRequest = new
            {
                requestProfile = new
                {
                    userLanguage = UserLanguageFunc?.Invoke() ?? "en"
                },
                mapSection = new
                {
                    _type = "MapSectionByBounds",
                    bounds = new { minX = left, maxX = right, minY = top, maxY = bottom }
                },
                imageOptions = new
                {
                    width,
                    height
                },
                mapOptions = new
                {
                    layers = ThemesForRenderingFunc?.Invoke().ToArray(),
                    timeConsideration = GetTimeConsideration(),
                    showOnlyRelevantByTime = ShowOnlyRelevantByTimeFunc?.Invoke() ?? false
                },
                resultFields = new
                {
                    image = true,
                    featureThemeIds = ThemesWithMapObjectsFunc?.Invoke().ToArray()
                },
                coordinateFormat = "EPSG:76131"
            };

            return mapRequest.ToJson();
        }

        private object GetTimeConsideration()
        {
            try
            {
                string timeConsiderationScenario = TimeConsiderationScenarioFunc?.Invoke();
                string referenceTime = ReferenceTimeFunc?.Invoke();
                switch (timeConsiderationScenario)
                {
                    case "OptimisticTimeConsideration":
                        return new
                        {
                            _type = timeConsiderationScenario
                        };

                    case "SnapshotTimeConsideration":
                        return (referenceTime == null) ? null : new
                        {
                            _type = timeConsiderationScenario,
                            referenceTime
                        };

                    case "TimeSpanConsideration":
                        double? timeSpan = TimeSpanFunc?.Invoke();
                        return ((referenceTime == null) || !timeSpan.HasValue) ? null : new
                        {
                            _type = timeConsiderationScenario,
                            referenceTime,
                            timeSpan = timeSpan.Value
                        };

                    default: return null;
                }
            }
            catch (Exception) { return null; }
        }

        // Helper class for conversion of JSON response
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private class ResponseObject
        {
            public string image { get; set; }
            public List<Feature> features { get; set; }

            public class Feature
            {
                public string id { get; set; }
                public ReferenceCoordinate referenceCoordinate { get; set; }
                public ReferencePixelPoint referencePixelPoint { get; set; }
                public string themeId { get; set; }
                public List<Attribute> attributes { get; set; }
            }

            public class ReferenceCoordinate
            {
                public double x { get; set; }
                public double y { get; set; }
            }

            public class ReferencePixelPoint
            {
                public int x { get; set; }
                public int y { get; set; }
            }

            public class Attribute
            {
                public string key { get; set; }
                public string value { get; set; }
            }
        }
    }
}
