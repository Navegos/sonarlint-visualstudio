﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2025 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.Core.Helpers;
using SonarLint.VisualStudio.Core.Telemetry;
using SonarLint.VisualStudio.Core.WPF;
using SonarLint.VisualStudio.Infrastructure.VS;
using SonarLint.VisualStudio.Infrastructure.VS.DocumentEvents;
using SonarLint.VisualStudio.IssueVisualization.Editor;
using SonarLint.VisualStudio.IssueVisualization.Helpers;
using SonarLint.VisualStudio.IssueVisualization.IssueVisualizationControl.ViewModels.Commands;
using SonarLint.VisualStudio.IssueVisualization.Models;
using SonarLint.VisualStudio.IssueVisualization.Security.IssuesStore;
using SonarLint.VisualStudio.IssueVisualization.Security.Taint.Models;
using SonarLint.VisualStudio.IssueVisualization.Selection;
using SonarQube.Client;

namespace SonarLint.VisualStudio.IssueVisualization.Security.Taint.TaintList.ViewModels
{
    internal interface ITaintIssuesControlViewModel : INotifyPropertyChanged, IDisposable
    {
        ICommand NavigateCommand { get; }

        ICommand ShowInBrowserCommand { get; }

        ICommand ShowVisualizationPaneCommand { get; }

        ICommand ShowDocumentationCommand { get; }

        ICollectionView IssuesView { get; }

        ITaintIssueViewModel SelectedIssue { get; set; }

        INavigateToRuleDescriptionCommand NavigateToRuleDescriptionCommand { get; }

        bool HasServerIssues { get; }

        string WindowCaption { get; }

        string ServerTypeDisplayName { get; }
    }

    /// <summary>
    /// View model that surfaces the data in the TaintStore
    /// </summary>
    /// <remarks>Monitors the taint store for changes and updates the view model accordingly.</remarks>
    internal sealed class TaintIssuesControlViewModel : ViewModelBase, ITaintIssuesControlViewModel
    {
        private readonly IActiveDocumentTracker activeDocumentTracker;
        private readonly IShowInBrowserService showInBrowserService;
        private readonly ITelemetryManager telemetryManager;
        private readonly IIssueSelectionService selectionService;
        private readonly ITaintStore store;
        private readonly IMenuCommandService menuCommandService;
        private readonly ISonarQubeService sonarQubeService;
        private readonly IThreadHandling threadHandling;

        private readonly object Lock = new object();
        private string activeDocumentFilePath;
        private string windowCaption;
        private ITaintIssueViewModel selectedIssue;
        private string serverTypeDisplayName;

        private readonly ObservableCollection<ITaintIssueViewModel> unfilteredIssues;

        public ICollectionView IssuesView { get; }

        public ICommand NavigateCommand { get; private set; }

        public ICommand ShowInBrowserCommand { get; private set; }

        public ICommand ShowVisualizationPaneCommand { get; private set; }

        public ICommand ShowDocumentationCommand { get; }

        public INavigateToRuleDescriptionCommand NavigateToRuleDescriptionCommand { get; }

        public bool HasServerIssues => unfilteredIssues.Any();

        public string WindowCaption
        {
            get => windowCaption;
            set
            {
                if (windowCaption != value)
                {
                    windowCaption = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ITaintIssueViewModel SelectedIssue
        {
            get => selectedIssue;
            set
            {
                if (selectedIssue != value)
                {
                    selectedIssue = value;
                    selectionService.SelectedIssue = selectedIssue?.TaintIssueViz;
                }
            }
        }

        public string ServerTypeDisplayName
        {
            get => serverTypeDisplayName;
            set
            {
                if (serverTypeDisplayName != value)
                {
                    serverTypeDisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TaintIssuesControlViewModel(
            ITaintStore store,
            ILocationNavigator locationNavigator,
            IActiveDocumentTracker activeDocumentTracker,
            IActiveDocumentLocator activeDocumentLocator,
            IShowInBrowserService showInBrowserService,
            ITelemetryManager telemetryManager,
            IIssueSelectionService selectionService,
            ICommand navigateToDocumentationCommand,
            IMenuCommandService menuCommandService,
            ISonarQubeService sonarQubeService,
            INavigateToRuleDescriptionCommand navigateToRuleDescriptionCommand
        )
            : this(store, locationNavigator, activeDocumentTracker, activeDocumentLocator, showInBrowserService, telemetryManager, selectionService,
                navigateToDocumentationCommand, menuCommandService, sonarQubeService, navigateToRuleDescriptionCommand, ThreadHandling.Instance)
        {
        }

        internal /* for testing */ TaintIssuesControlViewModel(
            ITaintStore store,
            ILocationNavigator locationNavigator,
            IActiveDocumentTracker activeDocumentTracker,
            IActiveDocumentLocator activeDocumentLocator,
            IShowInBrowserService showInBrowserService,
            ITelemetryManager telemetryManager,
            IIssueSelectionService selectionService,
            ICommand navigateToDocumentationCommand,
            IMenuCommandService menuCommandService,
            ISonarQubeService sonarQubeService,
            INavigateToRuleDescriptionCommand navigateToRuleDescriptionCommand,
            IThreadHandling threadHandling)
        {
            this.threadHandling = threadHandling;
            unfilteredIssues = new ObservableCollection<ITaintIssueViewModel>();
            AllowMultiThreadedAccessToIssuesCollection();

            this.menuCommandService = menuCommandService;
            this.sonarQubeService = sonarQubeService;
            activeDocumentFilePath = activeDocumentLocator.FindActiveDocument()?.FilePath;
            this.activeDocumentTracker = activeDocumentTracker;
            activeDocumentTracker.ActiveDocumentChanged += ActiveDocumentTracker_OnDocumentFocused;

            this.showInBrowserService = showInBrowserService;
            this.telemetryManager = telemetryManager;
            NavigateToRuleDescriptionCommand = navigateToRuleDescriptionCommand;

            this.selectionService = selectionService;
            this.selectionService.SelectedIssueChanged += SelectionService_SelectionChanged;

            this.store = store;
            this.store.IssuesChanged += Store_IssuesChanged;
            UpdateIssues();

            IssuesView = new ListCollectionView(unfilteredIssues);

            ShowDocumentationCommand = navigateToDocumentationCommand;

            SetCommands(locationNavigator);
            UpdateServerType();
            SetDefaultSortOrder();
            UpdateCaptionAndListFilter();
        }

        private void ActiveDocumentTracker_OnDocumentFocused(object sender, ActiveDocumentChangedEventArgs e)
        {
            activeDocumentFilePath = e.ActiveTextDocument?.FilePath;
            UpdateServerType();
            UpdateCaptionAndListFilter();
        }

        private void UpdateServerType()
        {
            var newServerType = sonarQubeService.GetServerInfo()?.ServerType;
            ServerTypeDisplayName = newServerType?.ToProductName();
        }

        private void ApplyViewFilter(Predicate<object> filter) => IssuesView.Filter = filter;

        private bool NotSuppressedIssuesInCurrentDocumentFilter(object viewModel)
        {
            if (string.IsNullOrEmpty(activeDocumentFilePath))
            {
                return false;
            }

            var issueViz = ((ITaintIssueViewModel)viewModel).TaintIssueViz;

            if (issueViz.IsResolved)
            {
                return false;
            }

            var allFilePaths = issueViz.GetAllLocations()
                .Select(x => x.CurrentFilePath)
                .Where(x => !string.IsNullOrEmpty(x));

            return allFilePaths.Any(x => PathHelper.IsMatchingPath(x, activeDocumentFilePath));
        }

        private void SetDefaultSortOrder() =>
            IssuesView.SortDescriptions.Add(
                new SortDescription("TaintIssueViz.Issue.CreationTimestamp", ListSortDirection.Descending));

        /// <summary>
        /// Allow the observable collection <see cref="unfilteredIssues"/> to be modified from a non-UI thread.
        /// </summary>
        private void AllowMultiThreadedAccessToIssuesCollection()
        {
            Debug.Assert(unfilteredIssues != null, "unfiltered issues must be set before calling AllowMultiThreadedAccessToIssuesCollection");

            threadHandling.RunOnUIThread(() => { BindingOperations.EnableCollectionSynchronization(unfilteredIssues, Lock); });
        }

        private void SetCommands(ILocationNavigator locationNavigator)
        {
            NavigateCommand = new DelegateCommand(
                parameter =>
                {
                    telemetryManager.TaintIssueInvestigatedLocally();

                    var selected = (ITaintIssueViewModel)parameter;
                    locationNavigator.TryNavigate(selected.TaintIssueViz);
                },
                parameter => parameter is ITaintIssueViewModel);

            ShowInBrowserCommand = new DelegateCommand(
                parameter =>
                {
                    telemetryManager.TaintIssueInvestigatedRemotely();

                    var selected = (ITaintIssueViewModel)parameter;
                    var taintIssue = (ITaintIssue)selected.TaintIssueViz.Issue;
                    showInBrowserService.ShowIssue(taintIssue.IssueServerKey);
                },
                parameter => parameter is ITaintIssueViewModel);

            ShowVisualizationPaneCommand = new DelegateCommand(
                parameter =>
                {
                    telemetryManager.TaintIssueInvestigatedLocally();
                    var commandId = new CommandID(IssueVisualization.Commands.Constants.CommandSetGuid, IssueVisualization.Commands.Constants.ViewToolWindowCommandId);

                    menuCommandService.GlobalInvoke(commandId);
                }, parameter => parameter is ITaintIssueViewModel);
        }

        private void UpdateIssues()
        {
            unfilteredIssues.Clear();

            foreach (var issueViz in store.GetAll())
            {
                var taintIssueViewModel = new TaintIssueViewModel(issueViz);
                unfilteredIssues.Add(taintIssueViewModel);
            }

            RaisePropertyChanged(nameof(HasServerIssues));
        }

        private void UpdateCaptionAndListFilter()
        {
            threadHandling.RunOnUIThread(() =>
            {
                // WPF is not automatically re-applying the filter when the underlying list
                // of issues changes, so we're manually applying the filtering every time.
                ApplyViewFilter(NotSuppressedIssuesInCurrentDocumentFilter);

                // We'll show the default caption if:
                // * there are no underlying issues, or
                // * there is not an active document.
                // Otherwise, we'll add a suffix showing the number of issues in the active document.
                string suffix = null;

                if (unfilteredIssues.Count != 0 && activeDocumentFilePath != null)
                {
                    suffix = $" ({GetFilteredIssuesCount()})";
                }

                WindowCaption = Resources.TaintToolWindowCaption + suffix;

                // Must run on the UI thread.
                int GetFilteredIssuesCount() => IssuesView.OfType<object>().Count();
            });
        }

        private void Store_IssuesChanged(object sender, IssuesChangedEventArgs e)
        {
            UpdateIssues();
            UpdateCaptionAndListFilter();
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            selectedIssue = unfilteredIssues.FirstOrDefault(x => x.TaintIssueViz == selectionService.SelectedIssue);
            RaisePropertyChanged(nameof(SelectedIssue));
        }

        public void Dispose()
        {
            selectionService.SelectedIssueChanged -= SelectionService_SelectionChanged;
            store.IssuesChanged -= Store_IssuesChanged;
            activeDocumentTracker.ActiveDocumentChanged -= ActiveDocumentTracker_OnDocumentFocused;
        }
    }
}
