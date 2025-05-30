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

using Moq;
using SonarLint.VisualStudio.Core.Analysis;
using SonarLint.VisualStudio.IssueVisualization.Security.Hotspots.Models;

namespace SonarLint.VisualStudio.IssueVisualization.Security.UnitTests.Hotspots.Models
{
    [TestClass]
    public class HotspotTests
    {
        private static readonly IHotspotRule ValidRule = CreateRule("x123");

        [TestMethod]
        public void Ctor_NullLocation_ArgumentNullException()
        {
            Action act = () => new Hotspot(id: null,
                "hotspot key",
                false,
                "server-path",
                primaryLocation: null,
                ValidRule,
                null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("primaryLocation");
        }

        [TestMethod]
        public void Ctor_PropertiesSet()
        {
            var hotspot = new Hotspot(id: null,
                "hotspot key",
                true,
                "server-path",
                primaryLocation: new AnalysisIssueLocation(
                    "message",
                    "local-path.cpp",
                    textRange: new TextRange(
                        1,
                        2,
                        3,
                        4,
                        "hash")),
                ValidRule,
                null,
                "contextKey");

            hotspot.IssueServerKey.Should().Be("hotspot key");
            hotspot.IsResolved.Should().BeTrue();
            hotspot.ServerFilePath.Should().Be("server-path");
            hotspot.RuleKey.Should().Be(ValidRule.RuleKey);
            hotspot.Rule.Should().BeSameAs(ValidRule);
            hotspot.RuleDescriptionContextKey.Should().Be("contextKey");

            hotspot.PrimaryLocation.FilePath.Should().Be("local-path.cpp");
            hotspot.PrimaryLocation.Message.Should().Be("message");
            hotspot.PrimaryLocation.TextRange.StartLine.Should().Be(1);
            hotspot.PrimaryLocation.TextRange.EndLine.Should().Be(2);
            hotspot.PrimaryLocation.TextRange.StartLineOffset.Should().Be(3);
            hotspot.PrimaryLocation.TextRange.EndLineOffset.Should().Be(4);
            hotspot.PrimaryLocation.TextRange.LineHash.Should().Be("hash");
        }

        [TestMethod]
        public void Ctor_NoFlows_EmptyFlows()
        {
            IReadOnlyList<IAnalysisIssueFlow> flows = null;
            var hotspot = new Hotspot(id: null,
                "hotspot key",
                false,
                "server-path",
                new AnalysisIssueLocation(
                    "message",
                    "local-path.cpp",
                    textRange: new TextRange(
                        1,
                        2,
                        3,
                        4,
                        "hash")),
                ValidRule,
                flows);

            hotspot.Flows.Should().BeEmpty();
        }

        [TestMethod]
        public void Ctor_HasFlows_CorrectFlows()
        {
            var flows = new[] { Mock.Of<IAnalysisIssueFlow>(), Mock.Of<IAnalysisIssueFlow>() };
            var hotspot = new Hotspot(id: null,
                "hotspot key",
                false,
                "server-path",
                new AnalysisIssueLocation(
                    "message",
                    "local-path.cpp",
                    textRange: new TextRange(
                        1,
                        2,
                        3,
                        4,
                        "hash")),
                ValidRule,
                flows);

            hotspot.Flows.Should().BeEquivalentTo(flows);
        }

        private static IHotspotRule CreateRule(string ruleKey)
        {
            var ruleMock = new Mock<IHotspotRule>();
            ruleMock.Setup(x => x.RuleKey).Returns(ruleKey);
            return ruleMock.Object;
        }
    }
}
