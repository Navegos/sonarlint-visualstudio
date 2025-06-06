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

using FluentAssertions;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.Core.Analysis;
using SonarLint.VisualStudio.IssueVisualization.Helpers;
using SonarLint.VisualStudio.TestInfrastructure;

namespace SonarLint.VisualStudio.IssueVisualization.UnitTests.Helpers
{
    [TestClass]
    public class AnalysisSeverityToVsSeverityConverterTests
    {
        private Mock<IEnvironmentSettings> envSettingsMock;
        private AnalysisSeverityToVsSeverityConverter testSubject;

        [TestInitialize]
        public void TestInitialize()
        {
            envSettingsMock = new Mock<IEnvironmentSettings>();
            testSubject = new AnalysisSeverityToVsSeverityConverter(envSettingsMock.Object);
        }

        [TestMethod]
        [DataRow(AnalysisIssueSeverity.Info, __VSERRORCATEGORY.EC_MESSAGE)]
        [DataRow(AnalysisIssueSeverity.Minor, __VSERRORCATEGORY.EC_MESSAGE)]
        [DataRow(AnalysisIssueSeverity.Major, __VSERRORCATEGORY.EC_WARNING)]
        [DataRow(AnalysisIssueSeverity.Critical, __VSERRORCATEGORY.EC_WARNING)]
        public void Convert_NotBlocker_CorrectlyMapped(AnalysisIssueSeverity severity, __VSERRORCATEGORY expectedVsErrorCategory)
        {
            testSubject.Convert(severity).Should().Be(expectedVsErrorCategory);
        }

        [TestMethod]
        [DataRow(true, __VSERRORCATEGORY.EC_ERROR)]
        [DataRow(false, __VSERRORCATEGORY.EC_WARNING)]
        public void Convert_Blocker_CorrectlyMapped(bool shouldTreatBlockerAsError, __VSERRORCATEGORY expectedVsErrorCategory)
        {
            envSettingsMock.Setup(x => x.TreatBlockerSeverityAsError()).Returns(shouldTreatBlockerAsError);

            testSubject.Convert(AnalysisIssueSeverity.Blocker).Should().Be(expectedVsErrorCategory);
        }

        [TestMethod]
        [DataRow(SoftwareQualitySeverity.High, __VSERRORCATEGORY.EC_WARNING)]
        [DataRow(SoftwareQualitySeverity.Medium, __VSERRORCATEGORY.EC_WARNING)]
        [DataRow(SoftwareQualitySeverity.Low, __VSERRORCATEGORY.EC_MESSAGE)]
        public void ConvertFromCct_CorrectlyMapped(SoftwareQualitySeverity severity, __VSERRORCATEGORY expectedVsErrorCategory)
        {
            testSubject.ConvertFromCct(severity).Should().Be(expectedVsErrorCategory);
        }

        [TestMethod]
        public void ConvertFromCct_InvalidCctSeverity_DoesNotThrow()
        {
            testSubject.ConvertFromCct((SoftwareQualitySeverity)(-999)).Should().Be(__VSERRORCATEGORY.EC_MESSAGE);
        }

        [TestMethod]
        public void Convert_InvalidDaemonSeverity_DoesNotThrow()
        {
            testSubject.Convert((AnalysisIssueSeverity)(-999)).Should().Be(__VSERRORCATEGORY.EC_MESSAGE);
        }

        [TestMethod]
        public void GetVsSeverity_IssueWithNewCct_UsesNewCctConverter()
        {
            var converter = new Mock<IAnalysisSeverityToVsSeverityConverter>();

            converter.Object.GetVsSeverity(new DummyAnalysisIssue
            {
                Severity = AnalysisIssueSeverity.Major, HighestImpact = new Impact(SoftwareQuality.Maintainability, SoftwareQualitySeverity.High)
            });

            converter.Verify(x => x.ConvertFromCct(SoftwareQualitySeverity.High), Times.Once);
            converter.Invocations.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetVsSeverity_IssueWithoutNewCct_UsesOldSeverityConverter()
        {
            var converter = new Mock<IAnalysisSeverityToVsSeverityConverter>();

            converter.Object.GetVsSeverity(new DummyAnalysisIssue
            {
                Severity = AnalysisIssueSeverity.Major
            });

            converter.Verify(x => x.Convert(AnalysisIssueSeverity.Major), Times.Once);
            converter.Invocations.Should().HaveCount(1);
        }
    }
}
