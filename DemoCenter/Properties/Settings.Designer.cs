﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ptv.XServer.Demo.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("api-eu-test")]
        public string XUrl {
            get {
                return ((string)(this["XUrl"]));
            }
            set {
                this["XUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string XToken {
            get {
                return ((string)(this["XToken"]));
            }
            set {
                this["XToken"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BingKey {
            get {
                return ((string)(this["BingKey"]));
            }
            set {
                this["BingKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string HereAppId {
            get {
                return ((string)(this["HereAppId"]));
            }
            set {
                this["HereAppId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string HereAppCode {
            get {
                return ((string)(this["HereAppCode"]));
            }
            set {
                this["HereAppCode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IntroSkipped {
            get {
                return ((bool)(this["IntroSkipped"]));
            }
            set {
                this["IntroSkipped"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DragDropIntroSkipped {
            get {
                return ((bool)(this["DragDropIntroSkipped"]));
            }
            set {
                this["DragDropIntroSkipped"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"        <Profile xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
          <FeatureLayer majorVersion=""1"" minorVersion=""0"">
            <GlobalSettings enableTimeDependency=""true""/>
            <Themes>
              <Theme id=""PTV_TrafficIncidents"" enabled=""false"" priorityLevel=""0""/>
              <Theme id=""PTV_TruckAttributes"" enabled=""false"" priorityLevel=""0""/>
              <Theme id=""PTV_RestrictionZones"" enabled=""false"" priorityLevel=""0""/>
              <Theme id=""PTV_SpeedPatterns"" enabled=""false"" priorityLevel=""0""/>
            </Themes>
          </FeatureLayer>
          <Routing majorVersion=""2"" minorVersion=""0"">
            <Course>
              <AdditionalDataRules enabled=""true"" />
              <DynamicRouting limitDynamicSpeedToStaticSpeed=""false"" />
            </Course>
            <Vehicle>
              <Load hazardousGoodsType=""HAZARDOUS"" />
            </Vehicle>
          </Routing>
        </Profile>
")]
        public string FeatureLayerEurope {
            get {
                return ((string)(this["FeatureLayerEurope"]));
            }
            set {
                this["FeatureLayerEurope"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<Profile
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <FeatureLayer majorVersion=""1"" minorVersion=""0"">
        <GlobalSettings enableTimeDependency=""true""/>
        <Themes>
            <Theme id=""PTV_PreferredRoutes"" enabled=""false"" priorityLevel=""0"">
                <PropertyValue id='preferredRouteType' value='1'/>
                <PropertyValue id='preferredRouteType' value='2'/>
                <PropertyValue id='preferredRouteType' value='3'/>
                <PropertyValue id='preferredRouteType' value='4'/>
                <PropertyValue id='preferredRouteType' value='5'/>
                <PropertyValue id='preferredRouteType' value='7'/>
                <PropertyValue id='preferredRouteType' value='18'/>
                <PropertyValue id='preferredRouteType' value='21'/>
                <PropertyValue id='preferredRouteType' value='22'/>
            </Theme>
            <Theme id=""PTV_SpeedPatterns"" enabled=""false"" priorityLevel=""0""/>
        </Themes>
    </FeatureLayer>
    <Routing majorVersion=""2"" minorVersion=""0"">
        <Course>
            <AdditionalDataRules enabled=""true"" />
            <DynamicRouting limitDynamicSpeedToStaticSpeed=""false"" />
        </Course>
        <Vehicle>
            <Load hazardousGoodsType=""HAZARDOUS"" />
        </Vehicle>
    </Routing>
</Profile>")]
        public string FeatureLayerNorthAmerica {
            get {
                return ((string)(this["FeatureLayerNorthAmerica"]));
            }
            set {
                this["FeatureLayerNorthAmerica"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<Profile xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
          <FeatureLayer majorVersion=""1"" minorVersion=""0"">
            <GlobalSettings enableTimeDependency=""true""/>
            <Themes>
              <Theme id=""PTV_PreferredRoutes"" enabled=""false"" priorityLevel=""0"">
                <PropertyValue id='preferredRouteType' value='9'/>
                <PropertyValue id='preferredRouteType' value='17'/>
                <PropertyValue id='preferredRouteType' value='23'/>
            </Theme>
              <Theme id=""PTV_TruckAttributes"" enabled=""false"" priorityLevel=""0""/>
              <Theme id=""PTV_SpeedPatterns"" enabled=""false"" priorityLevel=""0""/>
            </Themes>
          </FeatureLayer>
          <Routing majorVersion=""2"" minorVersion=""0"">
            <Course>
              <AdditionalDataRules enabled=""true"" />
              <DynamicRouting limitDynamicSpeedToStaticSpeed=""false"" />
            </Course>
            <Vehicle>
              <Load hazardousGoodsType=""HAZARDOUS"" />
			  <Physical>
			    <Dimension height='480'/>
			  </Physical>
            </Vehicle>
          </Routing>
        </Profile>")]
        public string FeatureLayerAustralia {
            get {
                return ((string)(this["FeatureLayerAustralia"]));
            }
            set {
                this["FeatureLayerAustralia"] = value;
            }
        }
    }
}
