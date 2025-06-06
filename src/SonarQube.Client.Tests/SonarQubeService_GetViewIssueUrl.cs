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
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonarQube.Client.Tests
{
    [TestClass]
    public class SonarQubeService_GetViewIssueUrl : SonarQubeService_TestBase
    {
        [TestMethod]
        public async Task GetViewIssueUrl_ReturnsExpectedUrl()
        {
            await ConnectToSonarQube("3.3.0.0");

            var result = service.GetViewIssueUrl("myProject", "myIssue");

            result.Host.Should().Be("localhost");
            result.LocalPath.Should().Be("/project/issues");
            result.Query.Should().Be("?id=myProject&issues=myIssue&open=myIssue");
        }

        [TestMethod]
        public void GetViewIssueUrl_NotConnected()
        {
            // No calls to Connect
            // No need to setup request, the operation should fail

            Action action = () => service.GetViewIssueUrl("myProject", "myIssue");

            action.Should().ThrowExactly<InvalidOperationException>()
                .WithMessage("This operation expects the service to be connected.");

            logger.ErrorMessages.Should().Contain("The service is expected to be connected.");
        }
    }
}
