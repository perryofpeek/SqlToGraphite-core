﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SqlToGraphite.UnitTests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SqlToGraphite.UnitTests.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;
        ///&lt;SqlToGraphiteConfig xmlns=&quot;SqlToGraphite_0.0.0.1&quot;&gt;
        ///  &lt;Jobs&gt;
        ///    &lt;ArrayOfJob xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns=&quot;&quot;&gt;
        ///      &lt;Job xsi:type=&quot;WmiClient&quot;&gt;
        ///        &lt;Name&gt;Wmi_Memory_AvailableMBytes&lt;/Name&gt;
        ///        &lt;ClientName&gt;GraphiteUdpClient&lt;/ClientName&gt;
        ///        &lt;Type&gt;SqlToGraphite.Plugin.Wmi.WmiClient&lt;/Type&gt;
        ///        &lt;Username /&gt;
        ///        &lt;Hostname&gt;localhost&lt;/Hostname&gt;
        ///        &lt;Password /&gt;
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string knownPlugin {
            get {
                return ResourceManager.GetString("knownPlugin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-16&quot;?&gt;
        ///&lt;SqlToGraphiteConfig xmlns=&quot;SqlToGraphite_0.0.0.1&quot;&gt;
        ///  &lt;Jobs&gt;
        ///    &lt;ArrayOfJob xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot; xmlns=&quot;&quot;&gt;
        ///      &lt;Job xsi:type=&quot;WmiClient&quot;&gt;
        ///        &lt;Name&gt;Wmi_Memory_AvailableMBytes&lt;/Name&gt;
        ///        &lt;ClientName&gt;GraphiteUdpClient&lt;/ClientName&gt;
        ///        &lt;Type&gt;SqlToGraphite.Plugin.Wmi.WmiClient&lt;/Type&gt;
        ///        &lt;Username /&gt;
        ///        &lt;Hostname&gt;localhost&lt;/Hostname&gt;
        ///        &lt;Password /&gt;
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UnknownPlugin {
            get {
                return ResourceManager.GetString("UnknownPlugin", resourceCulture);
            }
        }
    }
}