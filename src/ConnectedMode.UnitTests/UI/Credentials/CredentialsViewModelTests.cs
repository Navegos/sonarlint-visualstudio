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

using System.ComponentModel;
using System.IO;
using SonarLint.VisualStudio.ConnectedMode.UI;
using SonarLint.VisualStudio.ConnectedMode.UI.Credentials;
using SonarLint.VisualStudio.ConnectedMode.UI.Resources;
using SonarLint.VisualStudio.Core.Binding;

namespace SonarLint.VisualStudio.ConnectedMode.UnitTests.UI.Credentials
{
    [TestClass]
    public class CredentialsViewModelTests
    {
        private CredentialsViewModel testSubject;
        private ConnectionInfo sonarQubeConnectionInfo;
        private ConnectionInfo sonarCloudEuConnectionInfo;
        private ConnectionInfo sonarCloudUsConnectionInfo;
        private ISlCoreConnectionAdapter slCoreConnectionAdapter;
        private IProgressReporterViewModel progressReporterViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            sonarQubeConnectionInfo = new ConnectionInfo("http://localhost:9000", ConnectionServerType.SonarQube);
            sonarCloudEuConnectionInfo = new ConnectionInfo("myOrg", ConnectionServerType.SonarCloud, CloudServerRegion.Eu);
            sonarCloudUsConnectionInfo = new ConnectionInfo("myOrg", ConnectionServerType.SonarCloud, CloudServerRegion.Us);
            slCoreConnectionAdapter = Substitute.For<ISlCoreConnectionAdapter>();
            progressReporterViewModel = Substitute.For<IProgressReporterViewModel>();

            testSubject = new CredentialsViewModel(sonarQubeConnectionInfo, slCoreConnectionAdapter, progressReporterViewModel);
        }

        [TestMethod]
        public void IsConfirmationEnabled_TokenIsFilled_ReturnsTrue()
        {
            testSubject.Token = "dummy token".CreateSecureString();

            testSubject.IsConfirmationEnabled.Should().BeTrue();
        }

        [TestMethod]
        public void IsConfirmationEnabled_CorrectTokenIsProvidedAndValidationIsInProgress_ReturnsFalse()
        {
            testSubject.Token = "dummy token".CreateSecureString();

            testSubject.ProgressReporterViewModel.IsOperationInProgress.Returns(true);

            testSubject.IsConfirmationEnabled.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void IsConfirmationEnabled_TokenIsNotFilled_ReturnsFalse(string token)
        {
            testSubject.Token = token.CreateSecureString();

            testSubject.IsConfirmationEnabled.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ShouldTokenBeFilled_TokenIsEmpty_ReturnsTrue(string token)
        {
            testSubject.Token = token.CreateSecureString();

            testSubject.ShouldTokenBeFilled.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldTokenBeFilled_TokenIsFilled_ReturnsFalse()
        {
            testSubject.Token = "dummy token".CreateSecureString();

            testSubject.ShouldTokenBeFilled.Should().BeFalse();
        }

        [TestMethod]
        public void AccountSecurityUrl_ConnectionIsSonarCloudForEu_ReturnsExpectedSonarCloudUrl()
        {
            var viewModel = new CredentialsViewModel(sonarCloudEuConnectionInfo, slCoreConnectionAdapter, progressReporterViewModel);

            viewModel.AccountSecurityUrl.Should().Be("https://sonarcloud.io/account/security");
        }

        [TestMethod]
        public void AccountSecurityUrl_ConnectionIsSonarCloudForUs_ReturnsExpectedSonarCloudUrl()
        {
            var viewModel = new CredentialsViewModel(sonarCloudUsConnectionInfo, slCoreConnectionAdapter, progressReporterViewModel);

            viewModel.AccountSecurityUrl.Should().Be("https://sonarqube.us/account/security");
        }

        [TestMethod]
        public void AccountSecurityUrl_ConnectionIsSonarQube_ReturnsSonarQubeUrl()
        {
            var qubeUrl = "http://localhost:9000/";
            var viewModel = new CredentialsViewModel(new ConnectionInfo(qubeUrl, ConnectionServerType.SonarQube), slCoreConnectionAdapter, progressReporterViewModel);
            var expectedUrl = Path.Combine(qubeUrl, CredentialsViewModel.SecurityPageUrl);

            viewModel.AccountSecurityUrl.Should().Be(expectedUrl);
        }

        [TestMethod]
        public async Task AdapterValidateConnectionAsync_TokenIsProvided_ShouldValidateConnectionWithToken()
        {
            MockAdapterValidateConnectionAsync();
            testSubject.Token = "dummyToken".CreateSecureString();

            await testSubject.AdapterValidateConnectionAsync();

            await slCoreConnectionAdapter.Received(1).ValidateConnectionAsync(testSubject.ConnectionInfo,
                Arg.Is<TokenCredentialsModel>(x => x.Token == testSubject.Token));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ValidateConnectionAsync_ReturnsResponseFromSlCore(bool success)
        {
            progressReporterViewModel.ExecuteTaskWithProgressAsync(Arg.Any<TaskToPerformParams<ResponseStatus>>()).Returns(new ResponseStatus(success));

            var response = await testSubject.ValidateConnectionAsync();

            response.Should().Be(success);
        }

        [TestMethod]
        public async Task ValidateConnectionAsync_ReturnsResponseFromSlCore()
        {
            await testSubject.ValidateConnectionAsync();

            await progressReporterViewModel.Received(1)
                .ExecuteTaskWithProgressAsync(Arg.Is<TaskToPerformParams<ResponseStatus>>(x =>
                    x.TaskToPerform == testSubject.AdapterValidateConnectionAsync &&
                    x.ProgressStatus == UiResources.ValidatingConnectionProgressText &&
                    x.WarningText == UiResources.ValidatingConnectionFailedText &&
                    x.AfterProgressUpdated == testSubject.AfterProgressStatusUpdated));
        }

        [TestMethod]
        public void UpdateProgressStatus_RaisesEvents()
        {
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();
            testSubject.PropertyChanged += eventHandler;
            eventHandler.ReceivedCalls().Should().BeEmpty();

            testSubject.AfterProgressStatusUpdated();

            eventHandler.Received().Invoke(testSubject, Arg.Is<PropertyChangedEventArgs>(x => x.PropertyName == nameof(testSubject.IsConfirmationEnabled)));
        }

        [TestMethod]
        public void GetCredentialsModel_ReturnsModelWithToken()
        {
            testSubject.Token = "token".CreateSecureString();

            var credentialsModel = testSubject.GetCredentialsModel();

            credentialsModel.Should().BeOfType<TokenCredentialsModel>();
            ((TokenCredentialsModel)credentialsModel).Token.Should().Be(testSubject.Token);
        }

        [TestMethod]
        [DataRow(true, "token")]
        [DataRow(false, null)]
        public async Task GenerateTokenWithProgressAsync_ReturnsResponseFromTaskToPerformParams(bool success, string token)
        {
            progressReporterViewModel.ExecuteTaskWithProgressAsync(Arg.Any<TaskToPerformParams<ResponseStatusWithData<string>>>()).Returns(new ResponseStatusWithData<string>(success, token));

            var response = await testSubject.GenerateTokenWithProgressAsync();

            response.Should().NotBeNull();
            response.Success.Should().Be(success);
            response.ResponseData.Should().Be(token);
        }

        [TestMethod]
        public async Task GenerateTokenWithProgressAsync_ExecutesTaskWithCorrectParameters()
        {
            await testSubject.GenerateTokenWithProgressAsync();

            await progressReporterViewModel.Received(1)
                .ExecuteTaskWithProgressAsync(Arg.Is<TaskToPerformParams<ResponseStatusWithData<string>>>(x =>
                    x.TaskToPerform == testSubject.GenerateTokenAsync &&
                    x.ProgressStatus == UiResources.GeneratingTokenProgressText &&
                    x.WarningText == UiResources.GeneratingTokenFailedText &&
                    x.AfterProgressUpdated == testSubject.AfterProgressStatusUpdated));
        }

        [TestMethod]
        public async Task GenerateTokenAsync_InitializesTokenGenerationCancellationSource()
        {
            testSubject.TokenGenerationCancellationSource.Should().BeNull();

            await testSubject.GenerateTokenAsync();

            testSubject.TokenGenerationCancellationSource.Should().NotBeNull();
        }

        [TestMethod]
        [DataRow(true, "token")]
        [DataRow(false, null)]
        public async Task GenerateTokenAsync_ReturnsResponseFromSlCore(bool success, string token)
        {
            slCoreConnectionAdapter.GenerateTokenAsync(Arg.Any<ConnectionInfo>(), Arg.Any<CancellationToken>()).Returns(new ResponseStatusWithData<string>(success, token));

            var result = await testSubject.GenerateTokenAsync();

            await slCoreConnectionAdapter.Received(1).GenerateTokenAsync(testSubject.ConnectionInfo, testSubject.TokenGenerationCancellationSource.Token);
            result.Success.Should().Be(success);
            result.ResponseData.Should().Be(token);
        }

        [TestMethod]
        public void Ctor_TokenGenerationCancellationSource_NotInitialized() => testSubject.TokenGenerationCancellationSource.Should().BeNull();

        [TestMethod]
        public async Task TokenGenerationCancellationSource_Sets_CancelsPreviousCancellationTokenSource()
        {
            await testSubject.GenerateTokenAsync();
            var cancellationTokenSource1 = testSubject.TokenGenerationCancellationSource;

            await testSubject.GenerateTokenAsync();
            var cancellationTokenSource2 = testSubject.TokenGenerationCancellationSource;

            cancellationTokenSource1.IsCancellationRequested.Should().BeTrue();
            cancellationTokenSource2.IsCancellationRequested.Should().BeFalse();
        }

        private void MockAdapterValidateConnectionAsync(bool success = true) =>
            slCoreConnectionAdapter.ValidateConnectionAsync(Arg.Any<ConnectionInfo>(), Arg.Any<ICredentialsModel>())
                .Returns(new ResponseStatus(success));
    }
}
