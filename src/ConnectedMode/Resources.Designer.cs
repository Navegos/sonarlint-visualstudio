﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SonarLint.VisualStudio.ConnectedMode {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SonarLint.VisualStudio.ConnectedMode.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to [ActionRunner] Cancelling current operation....
        /// </summary>
        internal static string ActionRunner_CancellingCurrentOperation {
            get {
                return ResourceManager.GetString("ActionRunner_CancellingCurrentOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Binding failed due to: {0}.
        /// </summary>
        internal static string Binding_Fails {
            get {
                return ResourceManager.GetString("Binding_Fails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Finished calculating closest Sonar server branch. Result: {0}.
        /// </summary>
        internal static string BranchMapper_CalculatingServerBranch_Finished {
            get {
                return ResourceManager.GetString("BranchMapper_CalculatingServerBranch_Finished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Calculating closest Sonar server branch....
        /// </summary>
        internal static string BranchMapper_CalculatingServerBranch_Started {
            get {
                return ResourceManager.GetString("BranchMapper_CalculatingServerBranch_Started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping]     Checking Sonar server branches....
        /// </summary>
        internal static string BranchMapper_CheckingSonarBranches {
            get {
                return ResourceManager.GetString("BranchMapper_CheckingSonarBranches", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping]     Match found: same Sonar branch name: {0}.
        /// </summary>
        internal static string BranchMapper_Match_SameSonarBranchName {
            get {
                return ResourceManager.GetString("BranchMapper_Match_SameSonarBranchName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping]     No head branch detected .
        /// </summary>
        internal static string BranchMapper_NoHead {
            get {
                return ResourceManager.GetString("BranchMapper_NoHead", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping]     No match found - using Sonar server &quot;main&quot; branch.
        /// </summary>
        internal static string BranchMapper_NoMatchingBranchFound {
            get {
                return ResourceManager.GetString("BranchMapper_NoMatchingBranchFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping]     Updating closest match: branch = {0}, distance = {1}.
        /// </summary>
        internal static string BranchMapper_UpdatingClosestMatch {
            get {
                return ResourceManager.GetString("BranchMapper_UpdatingClosestMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Could not detect a git repo for the current solution.
        /// </summary>
        internal static string BranchProvider_CouldNotDetectGitRepo {
            get {
                return ResourceManager.GetString("BranchProvider_CouldNotDetectGitRepo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Failed to calculate closest local git branch, defaulting to Sonar main branch..
        /// </summary>
        internal static string BranchProvider_FailedToCalculateMatchingBranch {
            get {
                return ResourceManager.GetString("BranchProvider_FailedToCalculateMatchingBranch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Matching Sonar server branch: {0}.
        /// </summary>
        internal static string BranchProvider_MatchingServerBranchName {
            get {
                return ResourceManager.GetString("BranchProvider_MatchingServerBranchName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Solution is not bound.
        /// </summary>
        internal static string BranchProvider_NotInConnectedMode {
            get {
                return ResourceManager.GetString("BranchProvider_NotInConnectedMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/DotnetAnalyzerIndicator] {0}.
        /// </summary>
        internal static string DotnetAnalyzerIndicatorTemplate {
            get {
                return ResourceManager.GetString("DotnetAnalyzerIndicatorTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] The search for projects for the connectionId {0} with the search term &quot;{1}&quot; failed: {2}.
        /// </summary>
        internal static string FuzzySearchProjects_Fails {
            get {
                return ResourceManager.GetString("FuzzySearchProjects_Fails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Getting all projects failed.
        /// </summary>
        internal static string GetAllProjects_Fails {
            get {
                return ResourceManager.GetString("GetAllProjects_Fails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Failed to get project name by key.
        /// </summary>
        internal static string GetServerProjectByKey_Fails {
            get {
                return ResourceManager.GetString("GetServerProjectByKey_Fails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Server did not return a project name for the specified project key {0}.
        /// </summary>
        internal static string GetServerProjectByKey_ProjectNotFound {
            get {
                return ResourceManager.GetString("GetServerProjectByKey_ProjectNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Error handling repo change notification: {0}.
        /// </summary>
        internal static string GitMonitor_EventError {
            get {
                return ResourceManager.GetString("GitMonitor_EventError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/GitMonitor] Git head branch changed.
        /// </summary>
        internal static string GitMonitor_GitBranchChanged {
            get {
                return ResourceManager.GetString("GitMonitor_GitBranchChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/GitMonitor] Monitoring git repo. Root path: {0}.
        /// </summary>
        internal static string GitMonitor_MonitoringRepoStarted {
            get {
                return ResourceManager.GetString("GitMonitor_MonitoringRepoStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/GitMonitor] Stopped monitoring repo.
        /// </summary>
        internal static string GitMonitor_MonitoringRepoStopped {
            get {
                return ResourceManager.GetString("GitMonitor_MonitoringRepoStopped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/GitMonitor] No repo detected - nothing to monitor.
        /// </summary>
        internal static string GitMonitor_NoRepo {
            get {
                return ResourceManager.GetString("GitMonitor_NoRepo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Hotpots] Fetch operation failed: {0}.
        /// </summary>
        internal static string Hotpots_FetchError_Short {
            get {
                return ResourceManager.GetString("Hotpots_FetchError_Short", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Hotspots] Fetching all hotspots....
        /// </summary>
        internal static string Hotspots_Fetch_AllHotspots {
            get {
                return ResourceManager.GetString("Hotspots_Fetch_AllHotspots", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Hotspots] Finished fetching all hotspots.
        /// </summary>
        internal static string Hotspots_Fetch_AllHotspots_Finished {
            get {
                return ResourceManager.GetString("Hotspots_Fetch_AllHotspots_Finished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Hotspots] Error fetching data: {0}.
        /// </summary>
        internal static string Hotspots_FetchError_Verbose {
            get {
                return ResourceManager.GetString("Hotspots_FetchError_Verbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Hotspots] Fetch operation cancelled.
        /// </summary>
        internal static string Hotspots_FetchOperationCancelled {
            get {
                return ResourceManager.GetString("Hotspots_FetchOperationCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Checking if file exists before importing targets. File: {0}.
        /// </summary>
        internal static string ImportBeforeFileGenerator_CheckingIfFileExists {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_CheckingIfFileExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Creating directory {0}.
        /// </summary>
        internal static string ImportBeforeFileGenerator_CreatingDirectory {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_CreatingDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Failed to write file to disk: {0}.
        /// </summary>
        internal static string ImportBeforeFileGenerator_FailedToWriteFile {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_FailedToWriteFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Failed to write file to disk with exception: {0}.
        /// </summary>
        internal static string ImportBeforeFileGenerator_FailedToWriteFile_Verbose {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_FailedToWriteFile_Verbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Skipped file writing as identical file was found..
        /// </summary>
        internal static string ImportBeforeFileGenerator_FileAlreadyExists {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_FileAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Loading embedded resource file.
        /// </summary>
        internal static string ImportBeforeFileGenerator_LoadingResourceFile {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_LoadingResourceFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/ImportsBeforeFileGenerator] Writing file to imports before directory.
        /// </summary>
        internal static string ImportBeforeFileGenerator_WritingTargetFileToDisk {
            get {
                return ResourceManager.GetString("ImportBeforeFileGenerator_WritingTargetFileToDisk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Listing user organizations failed.
        /// </summary>
        internal static string ListUserOrganizations_Fails {
            get {
                return ResourceManager.GetString("ListUserOrganizations_Fails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string MuteIssuesService_Error_Caption {
            get {
                return ResourceManager.GetString("MuteIssuesService_Error_Caption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Issue is resolved but an error occured while adding the comment, please refer to the logs for more information..
        /// </summary>
        internal static string MuteIssuesService_Error_CommentAdditionFailed {
            get {
                return ResourceManager.GetString("MuteIssuesService_Error_CommentAdditionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to resolve the issue, please refer to the logs for more information..
        /// </summary>
        internal static string MuteIssuesService_Error_FailedToTransition {
            get {
                return ResourceManager.GetString("MuteIssuesService_Error_FailedToTransition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Credentials you have provided do not have enough permission to resolve issues. It requires the permission &apos;Administer Issues&apos;..
        /// </summary>
        internal static string MuteIssuesService_Error_InsufficientPermissions {
            get {
                return ResourceManager.GetString("MuteIssuesService_Error_InsufficientPermissions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Transition]Issue muting is only supported in connected mode.
        /// </summary>
        internal static string MuteWindowService_NotInConnectedMode {
            get {
                return ResourceManager.GetString("MuteWindowService_NotInConnectedMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {null}.
        /// </summary>
        internal static string NullBranchName {
            get {
                return ResourceManager.GetString("NullBranchName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Package initialized.
        /// </summary>
        internal static string Package_Initialized {
            get {
                return ResourceManager.GetString("Package_Initialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Initializing package....
        /// </summary>
        internal static string Package_Initializing {
            get {
                return ResourceManager.GetString("Package_Initializing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [SharedBindingConfigProvider] There&apos;s no .sonarlint shared folder or solution is not under git.
        /// </summary>
        internal static string SharedBindingConfigProvider_SavePathNotFound {
            get {
                return ResourceManager.GetString("SharedBindingConfigProvider_SavePathNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [SharedBindingConfigProvider] The .sonarlint shared folder was not found.
        /// </summary>
        internal static string SharedBindingConfigProvider_SharedFolderNotFound {
            get {
                return ResourceManager.GetString("SharedBindingConfigProvider_SharedFolderNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Binding changed -&gt; cache cleared.
        /// </summary>
        internal static string StatefulBranchProvider_BindingChanged {
            get {
                return ResourceManager.GetString("StatefulBranchProvider_BindingChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Binding updated -&gt; cache cleared.
        /// </summary>
        internal static string StatefulBranchProvider_BindingUpdated {
            get {
                return ResourceManager.GetString("StatefulBranchProvider_BindingUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Cache hit.
        /// </summary>
        internal static string StatefulBranchProvider_CacheHit {
            get {
                return ResourceManager.GetString("StatefulBranchProvider_CacheHit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Cache miss. Recalculating server branch mapping....
        /// </summary>
        internal static string StatefulBranchProvider_CacheMiss {
            get {
                return ResourceManager.GetString("StatefulBranchProvider_CacheMiss", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/BranchMapping] Closest Sonar server branch: {0}.
        /// </summary>
        internal static string StatefulBranchProvider_ReturnValue {
            get {
                return ResourceManager.GetString("StatefulBranchProvider_ReturnValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ServerIssueStore] Updating issue: {0}. No matching issue in store. Event will be ignored..
        /// </summary>
        internal static string Store_UpdateIssue_NoMatch {
            get {
                return ResourceManager.GetString("Store_UpdateIssue_NoMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ServerIssueStore] Raising change event....
        /// </summary>
        internal static string Store_UpdateIssue_RaisingEvent {
            get {
                return ResourceManager.GetString("Store_UpdateIssue_RaisingEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ServerIssueStore] Updating issue: {0}. Match found, update not required.
        /// </summary>
        internal static string Store_UpdateIssue_UpdateNotRequired {
            get {
                return ResourceManager.GetString("Store_UpdateIssue_UpdateNotRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ServerIssueStore] Updating issue: {0}. Match found, update required. New IsResolved value: {1}.
        /// </summary>
        internal static string Store_UpdateIssue_UpdateRequired {
            get {
                return ResourceManager.GetString("Store_UpdateIssue_UpdateRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Finished fetching all suppressions.
        /// </summary>
        internal static string Suppression_Fetch_AllIssues_Finished {
            get {
                return ResourceManager.GetString("Suppression_Fetch_AllIssues_Finished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Error fetching data: {0}.
        /// </summary>
        internal static string Suppression_FetchError_Verbose {
            get {
                return ResourceManager.GetString("Suppression_FetchError_Verbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Handling IssueChangedEvent, isResolved: {0}, issue keys for current branch: {1}.
        /// </summary>
        internal static string Suppression_IssueChangedEvent {
            get {
                return ResourceManager.GetString("Suppression_IssueChangedEvent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] IssueChangedEvent finished.
        /// </summary>
        internal static string Suppression_IssueChangedEventFinished {
            get {
                return ResourceManager.GetString("Suppression_IssueChangedEventFinished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Error updating data: {0}.
        /// </summary>
        internal static string Suppression_UpdateError_Verbose {
            get {
                return ResourceManager.GetString("Suppression_UpdateError_Verbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Fetching all suppressions....
        /// </summary>
        internal static string Suppressions_Fetch_AllIssues {
            get {
                return ResourceManager.GetString("Suppressions_Fetch_AllIssues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Fetch operation failed: {0}.
        /// </summary>
        internal static string Suppressions_FetchError_Short {
            get {
                return ResourceManager.GetString("Suppressions_FetchError_Short", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Fetch operation cancelled.
        /// </summary>
        internal static string Suppressions_FetchOperationCancelled {
            get {
                return ResourceManager.GetString("Suppressions_FetchOperationCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Update operation failed: {0}.
        /// </summary>
        internal static string Suppressions_UpdateError_Short {
            get {
                return ResourceManager.GetString("Suppressions_UpdateError_Short", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Suppressions] Update operation cancelled.
        /// </summary>
        internal static string Suppressions_UpdateOperationCancelled {
            get {
                return ResourceManager.GetString("Suppressions_UpdateOperationCancelled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/TimedUpdate] Triggering refresh of server settings....
        /// </summary>
        internal static string TimedUpdateTriggered {
            get {
                return ResourceManager.GetString("TimedUpdateTriggered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected server connection type.
        /// </summary>
        internal static string UnexpectedConnectionType {
            get {
                return ResourceManager.GetString("UnexpectedConnectionType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/UseSharedBinding] Connection to server {0} could not be found.
        /// </summary>
        internal static string UseSharedBinding_ConnectionNotFound {
            get {
                return ResourceManager.GetString("UseSharedBinding_ConnectionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode/UseSharedBinding] The credentials for the connection {0} could not be found.
        /// </summary>
        internal static string UseSharedBinding_CredentiasNotFound {
            get {
                return ResourceManager.GetString("UseSharedBinding_CredentiasNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [ConnectedMode] Validating credentials failed.
        /// </summary>
        internal static string ValidateCredentials_Fails {
            get {
                return ResourceManager.GetString("ValidateCredentials_Fails", resourceCulture);
            }
        }
    }
}
