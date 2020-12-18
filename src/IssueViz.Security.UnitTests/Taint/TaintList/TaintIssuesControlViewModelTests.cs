﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2020 SonarSource SA
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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Moq;
using SonarLint.VisualStudio.Infrastructure.VS;
using SonarLint.VisualStudio.Infrastructure.VS.DocumentEvents;
using SonarLint.VisualStudio.Integration.UnitTests;
using SonarLint.VisualStudio.IssueVisualization.Editor;
using SonarLint.VisualStudio.IssueVisualization.Models;
using SonarLint.VisualStudio.IssueVisualization.Security.SharedUI;
using SonarLint.VisualStudio.IssueVisualization.Security.Taint;
using SonarLint.VisualStudio.IssueVisualization.Security.Taint.Models;
using SonarLint.VisualStudio.IssueVisualization.Security.Taint.TaintList.ViewModels;

namespace SonarLint.VisualStudio.IssueVisualization.Security.UnitTests.Taint.TaintList
{
    [TestClass]
    public class TaintIssuesControlViewModelTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            // The ViewModel needs to be created on the UI thread
            ThreadHelper.SetCurrentThreadAsUIThread();
        }

        [TestMethod]
        public void Ctor_RegisterToSourceCollectionChanges()
        {
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization>();
            var testSubject = CreateTestSubject(storeCollection);

            var issueViz1 = CreateIssueViz();
            var issueViz2 = CreateIssueViz();
            var issueViz3 = CreateIssueViz();

            storeCollection.Add(issueViz1);

            testSubject.Issues.Count.Should().Be(1);
            testSubject.Issues[0].TaintIssueViz.Should().Be(issueViz1);

            storeCollection.Add(issueViz2);
            storeCollection.Add(issueViz3);

            testSubject.Issues.Count.Should().Be(3);
            testSubject.Issues[0].TaintIssueViz.Should().Be(issueViz1);
            testSubject.Issues[1].TaintIssueViz.Should().Be(issueViz2);
            testSubject.Issues[2].TaintIssueViz.Should().Be(issueViz3);

            storeCollection.Remove(issueViz2);

            testSubject.Issues.Count.Should().Be(2);
            testSubject.Issues[0].TaintIssueViz.Should().Be(issueViz1);
            testSubject.Issues[1].TaintIssueViz.Should().Be(issueViz3);
        }

        [TestMethod]
        public void Ctor_InitializeListWithStoreCollection()
        {
            var issueViz1 = CreateIssueViz();
            var issueViz2 = CreateIssueViz();
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization> {issueViz1, issueViz2};

            var testSubject = CreateTestSubject(storeCollection);

            testSubject.Issues.Count.Should().Be(2);
            testSubject.Issues.First().TaintIssueViz.Should().Be(issueViz1);
            testSubject.Issues.Last().TaintIssueViz.Should().Be(issueViz2);
        }

        [TestMethod]
        public void Ctor_NoActiveDocument_NoIssuesDisplayed()
        {
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization> { CreateIssueViz() };

            var activeDocumentLocator = new Mock<IActiveDocumentLocator>();
            activeDocumentLocator.Setup(x => x.FindActiveDocument()).Returns((ITextDocument) null);

            var testSubject = CreateTestSubject(storeCollection, activeDocumentLocator: activeDocumentLocator.Object);
            testSubject.Issues.Count.Should().Be(1);

            var view = CollectionViewSource.GetDefaultView(testSubject.Issues);
            view.Filter.Should().NotBeNull();

            var filteredItems = testSubject.Issues.Where(x => view.Filter(x));
            filteredItems.Count().Should().Be(0);
        }

        [TestMethod]
        public void Ctor_ActiveDocumentExists_IssuesFilteredForActiveFilePath()
        {
            var issueViz1 = CreateIssueViz("test1.cpp");
            var issueViz2 = CreateIssueViz("test2.cpp");
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization> { issueViz1, issueViz2 };

            var activeDocument = new Mock<ITextDocument>();
            activeDocument.Setup(x => x.FilePath).Returns("test2.cpp");

            var activeDocumentLocator = new Mock<IActiveDocumentLocator>();
            activeDocumentLocator.Setup(x => x.FindActiveDocument()).Returns(activeDocument.Object);

            var testSubject = CreateTestSubject(storeCollection, activeDocumentLocator: activeDocumentLocator.Object);

            testSubject.Issues.Count.Should().Be(2);

            var view = CollectionViewSource.GetDefaultView(testSubject.Issues);
            view.Filter.Should().NotBeNull();

            var filteredItems = testSubject.Issues.Where(x => view.Filter(x));
            filteredItems.Count().Should().Be(1);
            filteredItems.First().TaintIssueViz.Should().Be(issueViz2);
        }

        [TestMethod]
        public void Ctor_RegisterToActiveDocumentChanges()
        {
            var activeDocumentTracker = new Mock<IActiveDocumentTracker>();
            activeDocumentTracker.SetupAdd(x => x.OnDocumentFocused += null);

            CreateTestSubject(activeDocumentTracker: activeDocumentTracker.Object);

            activeDocumentTracker.VerifyAdd(x=> x.OnDocumentFocused += It.IsAny<EventHandler<DocumentFocusedEventArgs>>(), Times.Once);
            activeDocumentTracker.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Dispose_UnregisterFromActiveDocumentChanges()
        {
            var activeDocumentTracker = new Mock<IActiveDocumentTracker>();
            activeDocumentTracker.SetupRemove(x => x.OnDocumentFocused -= null);

            var testSubject = CreateTestSubject(activeDocumentTracker: activeDocumentTracker.Object);
            activeDocumentTracker.Invocations.Clear();

            testSubject.Dispose();

            activeDocumentTracker.VerifyRemove(x => x.OnDocumentFocused -= It.IsAny<EventHandler<DocumentFocusedEventArgs>>(), Times.Once);
            activeDocumentTracker.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void Dispose_UnregisterFromStoreCollectionChanges()
        {
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization>();
            var testSubject = CreateTestSubject(storeCollection);

            testSubject.Dispose();

            var issueViz = CreateIssueViz();
            storeCollection.Add(issueViz);

            testSubject.Issues.Count.Should().Be(0);
        }

        [TestMethod]
        public void NavigateCommand_CanExecute_NullParameter_False()
        {
            var locationNavigator = new Mock<ILocationNavigator>();
            var testSubject = CreateTestSubject(locationNavigator: locationNavigator.Object);

            VerifyCommandExecution(testSubject.NavigateCommand, null, false);

            locationNavigator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void NavigateCommand_CanExecute_ParameterIsNotTaintViewModel_False()
        {
            var locationNavigator = new Mock<ILocationNavigator>();
            var testSubject = CreateTestSubject(locationNavigator: locationNavigator.Object);

            VerifyCommandExecution(testSubject.NavigateCommand, "something", false);

            locationNavigator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void NavigateCommand_CanExecute_ParameterIsTaintViewModel_True()
        {
            var locationNavigator = new Mock<ILocationNavigator>();
            var testSubject = CreateTestSubject(locationNavigator: locationNavigator.Object);

            VerifyCommandExecution(testSubject.NavigateCommand, Mock.Of<ITaintIssueViewModel>(), true);

            locationNavigator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void NavigateCommand_Execute_LocationNavigated()
        {
            var locationNavigator = new Mock<ILocationNavigator>();

            var issueViz = CreateIssueViz();
            var viewModel = new Mock<ITaintIssueViewModel>();
            viewModel.Setup(x => x.TaintIssueViz).Returns(issueViz);

            var testSubject = CreateTestSubject(locationNavigator: locationNavigator.Object);
            testSubject.NavigateCommand.Execute(viewModel.Object);

            locationNavigator.Verify(x => x.TryNavigate(issueViz), Times.Once);
            locationNavigator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShowInBrowserCommand_CanExecute_NullParameter_False()
        {
            var showInBrowserService = new Mock<IShowInBrowserService>();
            var testSubject = CreateTestSubject(showInBrowserService: showInBrowserService.Object);

            VerifyCommandExecution(testSubject.ShowInBrowserCommand, null, false);

            showInBrowserService.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShowInBrowserCommand_CanExecute_ParameterIsNotTaintViewModel_False()
        {
            var showInBrowserService = new Mock<IShowInBrowserService>();
            var testSubject = CreateTestSubject(showInBrowserService: showInBrowserService.Object);

            VerifyCommandExecution(testSubject.ShowInBrowserCommand, "something", false);

            showInBrowserService.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShowInBrowserCommand_CanExecute_ParameterIsTaintViewModel_True()
        {
            var showInBrowserService = new Mock<IShowInBrowserService>();
            var testSubject = CreateTestSubject(showInBrowserService: showInBrowserService.Object);

            VerifyCommandExecution(testSubject.ShowInBrowserCommand, Mock.Of<ITaintIssueViewModel>(), true);

            showInBrowserService.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ShowInBrowserCommand_Execute_BrowserServiceIsCalled()
        {
            var issueViz = CreateIssueViz(issueKey: "issue key 123");
            var viewModel = new Mock<ITaintIssueViewModel>();
            viewModel.Setup(x => x.TaintIssueViz).Returns(issueViz);

            var showInBrowserService = new Mock<IShowInBrowserService>();
            var testSubject = CreateTestSubject(showInBrowserService: showInBrowserService.Object);

            testSubject.ShowInBrowserCommand.Execute(viewModel.Object);

            showInBrowserService.Verify(x => x.ShowIssue("issue key 123"), Times.Once);
            showInBrowserService.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ActiveDocumentChanged_NoActiveDocument_NoIssuesDisplayed()
        {
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization> { CreateIssueViz() };

            var activeDocumentTracker = new Mock<IActiveDocumentTracker>();

            var testSubject = CreateTestSubject(storeCollection, activeDocumentTracker: activeDocumentTracker.Object);

            activeDocumentTracker.Raise(x=> x.OnDocumentFocused += null, new DocumentFocusedEventArgs(null));

            testSubject.Issues.Count.Should().Be(1);

            var view = CollectionViewSource.GetDefaultView(testSubject.Issues);
            view.Filter.Should().NotBeNull();

            var filteredItems = testSubject.Issues.Where(x => view.Filter(x));
            filteredItems.Count().Should().Be(0);
        }

        [TestMethod]
        public void ActiveDocumentChanged_ActiveDocumentExists_IssuesFilteredForActiveFilePath()
        {
            var issueViz1 = CreateIssueViz("test1.cpp");
            var issueViz2 = CreateIssueViz("test2.cpp");
            var storeCollection = new ObservableCollection<IAnalysisIssueVisualization> { issueViz1, issueViz2 };

            var activeDocumentTracker = new Mock<IActiveDocumentTracker>();

            var testSubject = CreateTestSubject(storeCollection, activeDocumentTracker: activeDocumentTracker.Object);

            var activeDocument = new Mock<ITextDocument>();
            activeDocument.Setup(x => x.FilePath).Returns("test2.cpp");

            activeDocumentTracker.Raise(x => x.OnDocumentFocused += null, new DocumentFocusedEventArgs(activeDocument.Object));

            testSubject.Issues.Count.Should().Be(2);

            var view = CollectionViewSource.GetDefaultView(testSubject.Issues);
            view.Filter.Should().NotBeNull();

            var filteredItems = testSubject.Issues.Where(x => view.Filter(x));
            filteredItems.Count().Should().Be(1);
            filteredItems.First().TaintIssueViz.Should().Be(issueViz2);
        }

        private static TaintIssuesControlViewModel CreateTestSubject(
            ObservableCollection<IAnalysisIssueVisualization> originalCollection = null,
            ILocationNavigator locationNavigator = null,
            Mock<ITaintStore> store = null,
            IActiveDocumentTracker activeDocumentTracker = null,
            IActiveDocumentLocator activeDocumentLocator = null,
            IShowInBrowserService showInBrowserService = null)
        {
            originalCollection ??= new ObservableCollection<IAnalysisIssueVisualization>();
            var readOnlyWrapper = new ReadOnlyObservableCollection<IAnalysisIssueVisualization>(originalCollection);

            store ??= new Mock<ITaintStore>();
            store.Setup(x => x.GetAll()).Returns(readOnlyWrapper);

            activeDocumentTracker ??= Mock.Of<IActiveDocumentTracker>();
            activeDocumentLocator ??= Mock.Of<IActiveDocumentLocator>();
            showInBrowserService ??= Mock.Of<IShowInBrowserService>();

            return new TaintIssuesControlViewModel(store.Object,
                locationNavigator,
                activeDocumentTracker,
                activeDocumentLocator,
                showInBrowserService);
        }

        private IAnalysisIssueVisualization CreateIssueViz(string filePath = "test.cpp", string issueKey = "issue key")
        {
            var issue = new Mock<ITaintIssue>();
            issue.Setup(x => x.IssueKey).Returns(issueKey);

            var issueViz = new Mock<IAnalysisIssueVisualization>();
            issueViz.Setup(x => x.CurrentFilePath).Returns(filePath);
            issueViz.Setup(x => x.Issue).Returns(issue.Object);

            return issueViz.Object;
        }

        private void VerifyCommandExecution(ICommand command, object parameter, bool canExecute)
        {
            var result = command.CanExecute(parameter);
            result.Should().Be(canExecute);
        }
    }
}