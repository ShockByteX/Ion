﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ion.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ion.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Failed to find function: {0} (ProcessId: {1}, ProcessName: {2}, ModulePath: {3}).
        /// </summary>
        internal static string ErrorFailedToFindFunction {
            get {
                return ResourceManager.GetString("ErrorFailedToFindFunction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find process-module: {0} (ProcessId: {1}, ProcessName: {2}).
        /// </summary>
        internal static string ErrorFailedToFindModule {
            get {
                return ResourceManager.GetString("ErrorFailedToFindModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find process: {0}.
        /// </summary>
        internal static string ErrorFailedToFindProcess {
            get {
                return ResourceManager.GetString("ErrorFailedToFindProcess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to load process-module: {0} (ProcessId: {1}, ProcessName: {2}).
        /// </summary>
        internal static string ErrorFailedToLoadModule {
            get {
                return ResourceManager.GetString("ErrorFailedToLoadModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to read from: 0x{0}.
        /// </summary>
        internal static string ErrorFailedToReadFrom {
            get {
                return ResourceManager.GetString("ErrorFailedToReadFrom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to release memory page: 0x{0}.
        /// </summary>
        internal static string ErrorFailedToReleaseMemoryPage {
            get {
                return ResourceManager.GetString("ErrorFailedToReleaseMemoryPage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to write to: 0x{0}.
        /// </summary>
        internal static string ErrorFailedToWriteTo {
            get {
                return ResourceManager.GetString("ErrorFailedToWriteTo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value cannot be null ({0}).
        /// </summary>
        internal static string ErrorValueCannotBeNull {
            get {
                return ResourceManager.GetString("ErrorValueCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value cannot be null or empty ({0}).
        /// </summary>
        internal static string ErrorValueCannotBeNullOrEmpty {
            get {
                return ResourceManager.GetString("ErrorValueCannotBeNullOrEmpty", resourceCulture);
            }
        }
    }
}
