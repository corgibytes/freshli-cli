﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Corgibytes.Freshli.Cli.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CliOutput {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CliOutput() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Corgibytes.Freshli.Cli.Resources.CliOutput", typeof(CliOutput).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string Output_Date {
            get {
                return ResourceManager.GetString("Output_Date", resourceCulture);
            }
        }
        
        internal static string Output_LibYear {
            get {
                return ResourceManager.GetString("Output_LibYear", resourceCulture);
            }
        }
        
        internal static string Output_UpgradesAvailable {
            get {
                return ResourceManager.GetString("Output_UpgradesAvailable", resourceCulture);
            }
        }
        
        internal static string Output_Skipped {
            get {
                return ResourceManager.GetString("Output_Skipped", resourceCulture);
            }
        }
        
        internal static string ScanCommand_ScanCommand_Executing_scan_command_handler {
            get {
                return ResourceManager.GetString("ScanCommand_ScanCommand_Executing_scan_command_handler", resourceCulture);
            }
        }
        
        internal static string MainCommand_MainCommand_Root_Command_Executed {
            get {
                return ResourceManager.GetString("MainCommand_MainCommand_Root_Command_Executed", resourceCulture);
            }
        }
        
        internal static string AuthCommand_AuthCommand_Executing_auth_command_handler {
            get {
                return ResourceManager.GetString("AuthCommand_AuthCommand_Executing_auth_command_handler", resourceCulture);
            }
        }
        
        internal static string ScanCommandRunner_Run_Path_should_not_be_null_or_empty {
            get {
                return ResourceManager.GetString("ScanCommandRunner_Run_Path_should_not_be_null_or_empty", resourceCulture);
            }
        }
        
        internal static string ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty {
            get {
                return ResourceManager.GetString("ComputeLibYearCommandRunner_Run_FilePath_should_not_be_null_or_empty", resourceCulture);
            }
        }
        
        internal static string AgentsDetectCommandRunner_Run_No_detected_agents_found {
            get {
                return ResourceManager.GetString("AgentsDetectCommandRunner_Run_No_detected_agents_found", resourceCulture);
            }
        }
        
        internal static string ComputeLibYearCommandRunner_Table_Header_Package {
            get {
                return ResourceManager.GetString("ComputeLibYearCommandRunner_Table_Header_Package", resourceCulture);
            }
        }
        
        internal static string ComputeLibYearCommandRunner_Table_Header_Currently_Installed {
            get {
                return ResourceManager.GetString("ComputeLibYearCommandRunner_Table_Header_Currently_Installed", resourceCulture);
            }
        }
        
        internal static string ComputeLibYearCommandRunner_Table_Header_Released_at {
            get {
                return ResourceManager.GetString("ComputeLibYearCommandRunner_Table_Header_Released_at", resourceCulture);
            }
        }
        
        internal static string ComputeLibYearCommandRunner_Table_Header_Latest_Available {
            get {
                return ResourceManager.GetString("ComputeLibYearCommandRunner_Table_Header_Latest_Available", resourceCulture);
            }
        }
        
        internal static string ReadCycloneDxFile_Exception_Can_Not_Read_File {
            get {
                return ResourceManager.GetString("ReadCycloneDxFile_Exception_Can_Not_Read_File", resourceCulture);
            }
        }
    }
}
