﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.269
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=4.0.30319.1.
// 

namespace xserver
{
    /// <summary> Generic interface which encapsulates the xMapServer ws implementation. </summary>
    public interface IXMapWSBinding {

        /// <summary> See xServer documentation for details. </summary>
        /// <param name="MapSection_1"> See xServer documentation for details. </param>
        /// <param name="MapParams_2"> See xServer documentation for details. </param>
        /// <param name="ImageInfo_3"> See xServer documentation for details. </param>
        /// <param name="ArrayOfLayer_4"> See xServer documentation for details. </param>
        /// <param name="boolean_5"> See xServer documentation for details. </param>
        /// <param name="CallerContext_6"> See xServer documentation for details. </param>
        /// <returns> See xServer documentation for details. </returns>
        Map renderMap([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] MapSection MapSection_1, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] MapParams MapParams_2, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] ImageInfo ImageInfo_3, [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://xmap.xserver.ptvag.com")] Layer[] ArrayOfLayer_4, bool boolean_5, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] CallerContext CallerContext_6);

        /// <summary> See xServer documentation for details. </summary>
        /// <param name="BoundingBox_1"> See xServer documentation for details. </param>
        /// <param name="MapParams_2"> See xServer documentation for details. </param>
        /// <param name="ImageInfo_3"> See xServer documentation for details. </param>
        /// <param name="ArrayOfLayer_4"> See xServer documentation for details. </param>
        /// <param name="boolean_5"> See xServer documentation for details. </param>
        /// <param name="CallerContext_6"> See xServer documentation for details. </param>
        /// <returns> See xServer documentation for details. </returns>
        Map renderMapBoundingBox([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] BoundingBox BoundingBox_1, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] MapParams MapParams_2, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] ImageInfo ImageInfo_3, [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://xmap.xserver.ptvag.com")] Layer[] ArrayOfLayer_4, bool boolean_5, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] CallerContext CallerContext_6);

        /// <summary> See xServer documentation for details. </summary>
        /// <param name="MapSectionRot_1"> See xServer documentation for details. </param>
        /// <param name="MapParams_2"> See xServer documentation for details. </param>
        /// <param name="ImageInfo_3"> See xServer documentation for details. </param>
        /// <param name="ArrayOfLayer_4"> See xServer documentation for details. </param>
        /// <param name="boolean_5"> See xServer documentation for details. </param>
        /// <param name="CallerContext_6"> See xServer documentation for details. </param>
        /// <returns> See xServer documentation for details. </returns>
        Map renderMapRot([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] MapSectionRot MapSectionRot_1, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] MapParams MapParams_2, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] ImageInfo ImageInfo_3, [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://xmap.xserver.ptvag.com")] Layer[] ArrayOfLayer_4, bool boolean_5, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] CallerContext CallerContext_6);
    }

}