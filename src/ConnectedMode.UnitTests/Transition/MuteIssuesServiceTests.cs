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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SonarLint.VisualStudio.ConnectedMode.Suppressions;
using SonarLint.VisualStudio.ConnectedMode.Transition;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.Core.Binding;
using SonarLint.VisualStudio.Core.Transition;
using SonarLint.VisualStudio.Integration.TestInfrastructure.Helpers;
using SonarLint.VisualStudio.TestInfrastructure;
using SonarQube.Client;
using SonarQube.Client.Models;

namespace SonarLint.VisualStudio.ConnectedMode.UnitTests.Transition
{
    [TestClass]
    public class MuteIssuesServiceTests
    {
        [TestMethod]
        public void MefCtor_CheckIsExported()
        {
            MefTestHelpers.CheckTypeCanBeImported<MuteIssuesService, IMuteIssuesService>(
                MefTestHelpers.CreateExport<IActiveSolutionBoundTracker>(),
                MefTestHelpers.CreateExport<ILogger>(),
                MefTestHelpers.CreateExport<IMuteIssuesWindowService>(),
                MefTestHelpers.CreateExport<ISonarQubeService>(),
                MefTestHelpers.CreateExport<IServerIssuesStoreWriter>());
        }

        [TestMethod]
        public void MefCtor_CheckIsSingleton()
        {
            MefTestHelpers.CheckIsSingletonMefComponent<MuteIssuesService>();
        }

        [TestMethod]
        public void CacheOutOfSyncResolvedIssue_ThrowsIfNotResolved()
        {
            var testSubject = CreateTestSubject();

            Action act = () => testSubject.CacheOutOfSyncResolvedIssue(DummySonarQubeIssueFactory.CreateServerIssue());

            act.Should().ThrowExactly<ArgumentException>();
        }

        [TestMethod]
        public void CacheOutOfSyncResolvedIssue_SavesIssueToStore()
        {
            var sonarQubeIssue = DummySonarQubeIssueFactory.CreateServerIssue(true);
            var storeMock = new Mock<IServerIssuesStoreWriter>();
            var threadHandlingMock = new Mock<IThreadHandling>();

            var testSubject = CreateTestSubject(serverIssuesStore:storeMock.Object, threadHandling:threadHandlingMock.Object);

            testSubject.CacheOutOfSyncResolvedIssue(sonarQubeIssue);

            storeMock.Verify(x => x.AddIssues(It.Is<IEnumerable<SonarQubeIssue>>(p => p.SequenceEqual(new[] { sonarQubeIssue })), false));
            threadHandlingMock.Verify(x => x.ThrowIfOnUIThread());
        }

        [TestMethod]
        public async Task ResolveIssueWithDialogAsyncNotInConnectedMode_Logs()
        {
            var threadHandling = CreateThreadHandling();
            var activeSolutionBoundTracker = CreateActiveSolutionBoundTracker(false);
            var logger = new Mock<ILogger>();

            var testSubject = CreateTestSubject(activeSolutionBoundTracker: activeSolutionBoundTracker, logger: logger.Object, threadHandling: threadHandling.Object);

            await testSubject.ResolveIssueWithDialogAsync(DummySonarQubeIssueFactory.CreateServerIssue(), CancellationToken.None);

            logger.Verify(l => l.LogVerbose("[Transition]Issue muting is only supported in connected mode"), Times.Once);
            threadHandling.Verify(t => t.ThrowIfOnUIThread(), Times.Once());
        }

        [TestMethod]
        public async Task ResolveIssueWithDialogAsyncWindowOK_CallService()
        {
            var sonarQubeIssue = DummySonarQubeIssueFactory.CreateServerIssue();
            sonarQubeIssue.IsResolved.Should().BeFalse();

            var threadHandling = CreateThreadHandling();
            var muteIssuesWindowService = CreateMuteIssuesWindowService(true, SonarQubeIssueTransition.FalsePositive, "some comment");

            var sonarQubeService = new Mock<ISonarQubeService>();

            sonarQubeService.Setup(s => s.TransitionIssueAsync(It.IsAny<string>(), It.IsAny<SonarQubeIssueTransition>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(SonarQubeIssueTransitionResult.FailedToTransition);
            sonarQubeService.Setup(s => s.TransitionIssueAsync(sonarQubeIssue.IssueKey, SonarQubeIssueTransition.FalsePositive, "some comment", CancellationToken.None)).ReturnsAsync(SonarQubeIssueTransitionResult.Success);

            var serverIssuesStore = new Mock<IServerIssuesStoreWriter>();

            var testSubject = CreateTestSubject(muteIssuesWindowService: muteIssuesWindowService.Object, sonarQubeService: sonarQubeService.Object, serverIssuesStore: serverIssuesStore.Object, threadHandling: threadHandling.Object);

            await testSubject.ResolveIssueWithDialogAsync(sonarQubeIssue, CancellationToken.None);

            muteIssuesWindowService.Verify(s => s.Show(), Times.Once);
            sonarQubeService.Verify(s => s.TransitionIssueAsync(sonarQubeIssue.IssueKey, SonarQubeIssueTransition.FalsePositive, "some comment", CancellationToken.None), Times.Once);
            serverIssuesStore.Verify(s => s.AddIssues(It.Is<IEnumerable<SonarQubeIssue>>(p => p.SequenceEqual(new[] { sonarQubeIssue })), false), Times.Once);
            threadHandling.Verify(t => t.ThrowIfOnUIThread(), Times.Once());
            sonarQubeIssue.IsResolved.Should().BeTrue();
        }

        [TestMethod]
        public async Task ResolveIssueWithDialogAsyncWindowCancel_DontCallService()
        {
            var muteIssuesWindowService = CreateMuteIssuesWindowService(false, SonarQubeIssueTransition.FalsePositive, "some comment");

            var sonarQubeService = new Mock<ISonarQubeService>();

            var testSubject = CreateTestSubject(muteIssuesWindowService: muteIssuesWindowService.Object, sonarQubeService: sonarQubeService.Object);

            await testSubject.ResolveIssueWithDialogAsync(DummySonarQubeIssueFactory.CreateServerIssue(), CancellationToken.None);

            muteIssuesWindowService.Verify(s => s.Show(), Times.Once);
            sonarQubeService.VerifyNoOtherCalls();
        }

        [DataRow(SonarQubeIssueTransitionResult.InsufficientPermissions, "Credentials you have provided do not have enough permission to resolve issues. It requires the permission 'Administer Issues'.")]
        [DataRow(SonarQubeIssueTransitionResult.FailedToTransition, "Unable to resolve the issue, please refer to the logs for more information.")]
        [DataRow(SonarQubeIssueTransitionResult.CommentAdditionFailed, "Issue is resolved but an error occured while adding the comment, please refer to the logs for more information.")]
        [TestMethod]
        public async Task ResolveIssueWithDialogAsyncSQError_ShowsError(SonarQubeIssueTransitionResult result, string errorMessage)
        {
            var messageBox = new Mock<IMessageBox>();
            var muteIssuesWindowService = CreateMuteIssuesWindowService(true, SonarQubeIssueTransition.FalsePositive, "some comment");

            var sonarQubeService = new Mock<ISonarQubeService>();

            sonarQubeService.Setup(s => s.TransitionIssueAsync(It.IsAny<string>(), It.IsAny<SonarQubeIssueTransition>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            var testSubject = CreateTestSubject(muteIssuesWindowService: muteIssuesWindowService.Object, sonarQubeService: sonarQubeService.Object, messageBox: messageBox.Object);

            await testSubject.ResolveIssueWithDialogAsync(DummySonarQubeIssueFactory.CreateServerIssue(), CancellationToken.None);

            messageBox.Verify(mb => mb.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        private Mock<IMuteIssuesWindowService> CreateMuteIssuesWindowService(bool result, SonarQubeIssueTransition transition = default, string comment = default)
        {
            var muteIssuesWindowResponse = CreateMuteIssuesWindowResponse(result, transition, comment);

            var service = new Mock<IMuteIssuesWindowService>();
            service.Setup(s => s.Show()).Returns(muteIssuesWindowResponse);

            return service;

            static MuteIssuesWindowResponse CreateMuteIssuesWindowResponse(bool result, SonarQubeIssueTransition transition, string comment)
            {
                return new MuteIssuesWindowResponse
                {
                    Result = result,
                    IssueTransition = transition,
                    Comment = comment
                };
            }
        }

        private Mock<IThreadHandling> CreateThreadHandling()
        {
            var threadHandling = new Mock<IThreadHandling>();
            threadHandling
                .Setup(x => x.RunOnUIThreadAsync(It.IsAny<Action>()))
                .Callback<Action>(callbackAction =>
                {
                    callbackAction();
                });

            return threadHandling;
        }

        private IActiveSolutionBoundTracker CreateActiveSolutionBoundTracker(bool isConnectedMode = true)
        {
            var modeToReturn = isConnectedMode ? SonarLintMode.Connected : SonarLintMode.Standalone;
            var configuration = new BindingConfiguration(null, modeToReturn, null);

            var activeSolutionBoundTracker = new Mock<IActiveSolutionBoundTracker>();
            activeSolutionBoundTracker.SetupGet(x => x.CurrentConfiguration).Returns(configuration);

            return activeSolutionBoundTracker.Object;
        }

        private MuteIssuesService CreateTestSubject(IActiveSolutionBoundTracker activeSolutionBoundTracker = null,
            ILogger logger = null,
            IMuteIssuesWindowService muteIssuesWindowService = null,
            ISonarQubeService sonarQubeService = null,
            IServerIssuesStoreWriter serverIssuesStore = null,
            IThreadHandling threadHandling = null,
            IMessageBox messageBox = null)
        {
            activeSolutionBoundTracker ??= CreateActiveSolutionBoundTracker();
            logger ??= Mock.Of<ILogger>();
            muteIssuesWindowService ??= Mock.Of<IMuteIssuesWindowService>();
            sonarQubeService ??= Mock.Of<ISonarQubeService>();
            serverIssuesStore ??= Mock.Of<IServerIssuesStoreWriter>();
            threadHandling ??= CreateThreadHandling().Object;
            messageBox ??= Mock.Of<IMessageBox>();

            return new MuteIssuesService(activeSolutionBoundTracker, logger, muteIssuesWindowService, sonarQubeService, serverIssuesStore, threadHandling, messageBox);
        }
    }
}
