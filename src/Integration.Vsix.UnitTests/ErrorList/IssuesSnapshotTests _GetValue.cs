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

using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.Text;
using SonarLint.VisualStudio.Core.Analysis;
using SonarLint.VisualStudio.Infrastructure.VS;
using SonarLint.VisualStudio.Integration.Vsix;
using SonarLint.VisualStudio.IssueVisualization.Models;

namespace SonarLint.VisualStudio.Integration.UnitTests.ErrorList;

[TestClass]
public class IssuesSnapshotTests_GetValue
{
    private const string AFilePath = "foo.js";
    private DummyAnalysisIssue issue;
    private IAnalysisIssueVisualization issueViz;
    private Guid projectGuid;

    [TestInitialize]
    public void SetUp()
    {
        issue = CreateIssue(AFilePath);
        projectGuid = Guid.NewGuid();

        var textSnap = CreateTextSnapshot();
        issueViz = CreateIssueViz(issue, new SnapshotSpan(new SnapshotPoint(textSnap, 25), new SnapshotPoint(textSnap, 27)));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void Count_ReturnsNumberOfIssues(int numberOfIssues)
    {
        var analysisIssueVisualizations = Enumerable.Repeat(issueViz, numberOfIssues);

        var testSubject = new IssuesSnapshot("MyProject", Guid.Empty, "foo.js", analysisIssueVisualizations);
        testSubject.Count.Should().Be(numberOfIssues);
    }

    [TestMethod]
    public void TryCreateDetailsStringContent_ReturnsIssueMessage()
    {
        var snapshot = CreateIssueSnapshot("MyProject", projectGuid, AFilePath, new List<IAnalysisIssueVisualization> { issueViz });
        snapshot.TryCreateDetailsStringContent(0, out var content);
        content.Should().Be("This is dangerous");
    }

    [TestMethod]
    public void GetValue_UnknownColumn_False() => AssertGetValueReturnsFalse(columnName: "asdsdgdsgrgddfgfg");

    [TestMethod]
    [DataRow(-1)]
    [DataRow(999)]
    public void GetValue_IndexOutOfRange_False(int index) => AssertGetValueReturnsFalse(index);

    [DataRow(StandardTableKeyNames.Line)]
    [DataRow(StandardTableKeyNames.Column)]
    [TestMethod]
    public void GetValue_IssueFileLevel_ContentIsNull(string keyName)
    {
        var analysisIssue = CreateIssue(AFilePath, true);
        var analysisIssueViz = CreateIssueViz(analysisIssue, new SnapshotSpan());
        var issueSnapshot = CreateIssueSnapshot("FileLevel", Guid.NewGuid(), AFilePath, [analysisIssueViz]);

        issueSnapshot.TryGetValue(0, keyName, out var content).Should().BeTrue();

        content.Should().BeNull();
    }

    [TestMethod]
    public void GetValue_IssueHasNoSpan_False()
    {
        issueViz.Span = null;

        AssertGetValueReturnsFalse();
    }

    [TestMethod]
    public void GetValue_IssueHasEmptySpan_False()
    {
        issueViz.InvalidateSpan();

        AssertGetValueReturnsFalse();
    }

    [TestMethod]
    public void Ctor_BaseIssueIsNotAnalysisIssue_InvalidCastException()
    {
        var notAnalysisIssue = new Mock<IAnalysisIssueBase>();
        notAnalysisIssue.Setup(i => i.PrimaryLocation).Returns(() => new DummyAnalysisIssueLocation
        {
            TextRange = new DummyTextRange()
        });
        var notAnalysisIssueViz = new Mock<IAnalysisIssueVisualization>();
        notAnalysisIssueViz.SetupGet(x => x.Issue).Returns(notAnalysisIssue.Object);

        Action act = () => new IssuesSnapshot("test", projectGuid, "test.cpp", [notAnalysisIssueViz.Object]);
        act.Should().Throw<InvalidCastException>();
    }

    [TestMethod]
    public void GetValue_Line() => GetValue(StandardTableKeyNames.Line).Should().Be(12);

    [TestMethod]
    public void GetValue_Column() => GetValue(StandardTableKeyNames.Column).Should().Be(25 - 10);

    [TestMethod]
    public void GetValue_Path() => GetValue(StandardTableKeyNames.DocumentName).Should().Be(issue.PrimaryLocation.FilePath);

    [TestMethod]
    public void GetValue_Message() => GetValue(StandardTableKeyNames.Text).Should().Be(issue.PrimaryLocation.Message);

    [TestMethod]
    public void GetValue_ErrorCode() => GetValue(StandardTableKeyNames.ErrorCode).Should().Be(issue.RuleKey);

    [TestMethod]
    public void GetValue_Severity() => GetValue(StandardTableKeyNames.ErrorSeverity).Should().NotBeNull();

    [TestMethod]
    public void GetValue_BuildTool() => GetValue(StandardTableKeyNames.BuildTool).Should().Be("SonarLint");

    [TestMethod]
    public void GetValue_ErrorRank_Other() => GetValue(StandardTableKeyNames.ErrorRank).Should().Be(ErrorRank.Other);

    [TestMethod]
    public void GetValue_ErrorCategory_Is_CodeSmell_By_Default() => GetValue(StandardTableKeyNames.ErrorCategory).Should().Be("Blocker Code Smell");

    [TestMethod]
    public void GetValue_StandardMode_ErrorCategory_IsSeverityAndType()
    {
        issue.Type = AnalysisIssueType.Bug;
        issue.Severity = AnalysisIssueSeverity.Blocker;
        GetValue(StandardTableKeyNames.ErrorCategory).Should().Be("Blocker Bug");
        issue.Type = AnalysisIssueType.CodeSmell;
        GetValue(StandardTableKeyNames.ErrorCategory).Should().Be("Blocker Code Smell");
        issue.Type = AnalysisIssueType.Vulnerability;
        GetValue(StandardTableKeyNames.ErrorCategory).Should().Be("Blocker Vulnerability");
    }

    [TestMethod]
    [DataRow(SoftwareQuality.Security, SoftwareQualitySeverity.Blocker)]
    [DataRow(SoftwareQuality.Security, SoftwareQualitySeverity.High)]
    [DataRow(SoftwareQuality.Maintainability, SoftwareQualitySeverity.Medium)]
    [DataRow(SoftwareQuality.Maintainability, SoftwareQualitySeverity.Low)]
    [DataRow(SoftwareQuality.Reliability, SoftwareQualitySeverity.Info)]
    public void GetValue_MqrMode_ErrorCategory_IsSoftwareQualityAndSeverity(SoftwareQuality quality, SoftwareQualitySeverity severity)
    {
        issue.HighestImpact = new Impact(quality, severity);

        GetValue(StandardTableKeyNames.ErrorCategory).Should().Be(severity + " " + quality);
    }

    [TestMethod]
    public void GetValue_ErrorCodeToolTip()
    {
        issue.RuleKey = "javascript:123";
        GetValue(StandardTableKeyNames.ErrorCodeToolTip).Should().Be("Open description of rule javascript:123");
    }

    [TestMethod]
    public void GetValue_HelpLink()
    {
        issue.RuleKey = "javascript:123";
        GetValue(StandardTableKeyNames.HelpLink).Should().Be("https://rules.sonarsource.com/javascript/RSPEC-123");
    }

    [TestMethod]
    public void GetValue_ProjectName() => GetValue(StandardTableKeyNames.ProjectName).Should().Be("MyProject");

    [TestMethod]
    public void GetValue_ProjectGuid() => GetValue(StandardTableKeyNames.ProjectGuid).Should().Be(projectGuid);

    [TestMethod]
    public void GetValue_Issue() => GetValue(SonarLintTableControlConstants.IssueVizColumnName).Should().BeSameAs(issueViz);

    [TestMethod]
    public void GetValue_SuppressionState_Is_SuppressionState()
    {
        var textSnap = CreateTextSnapshot();

        var suppressedIssue = CreateIssue(AFilePath, isResolved: true);
        var suppressedIssueViz = CreateIssueViz(suppressedIssue, new SnapshotSpan(new SnapshotPoint(textSnap, 25), new SnapshotPoint(textSnap, 27)));
        var suppressedSnapshot = CreateIssueSnapshot("MyProject", projectGuid, AFilePath, new List<IAnalysisIssueVisualization> { suppressedIssueViz });
        GetValue(StandardTableKeyNames.SuppressionState, suppressedSnapshot).Should().BeSameAs(Boxes.SuppressionState.Suppressed);

        var activeIssue = CreateIssue(AFilePath, isResolved: false);
        var activeIssueViz = CreateIssueViz(activeIssue, new SnapshotSpan(new SnapshotPoint(textSnap, 25), new SnapshotPoint(textSnap, 27)));
        var activeSnapshot = CreateIssueSnapshot("MyProject", projectGuid, AFilePath, new List<IAnalysisIssueVisualization> { activeIssueViz });
        GetValue(StandardTableKeyNames.SuppressionState, activeSnapshot).Should().BeSameAs(Boxes.SuppressionState.Active);
    }

    private object GetValue(string columnName, IssuesSnapshot snapshot = null)
    {
        snapshot ??= CreateIssueSnapshot("MyProject", projectGuid, AFilePath, new List<IAnalysisIssueVisualization> { issueViz });
        snapshot.TryGetValue(0, columnName, out var content).Should().BeTrue();
        return content;
    }

    private void AssertGetValueReturnsFalse(int index = 0, string columnName = StandardTableKeyNames.Line)
    {
        var snapshot = CreateIssueSnapshot("MyProject", projectGuid, AFilePath, new List<IAnalysisIssueVisualization> { issueViz });
        snapshot.TryGetValue(index, columnName, out object content).Should().BeFalse();
        content.Should().BeNull();
    }

    private static IAnalysisIssueVisualization CreateIssueViz(IAnalysisIssue issue, SnapshotSpan snapshotSpan)
    {
        var issueVizMock = new Mock<IAnalysisIssueVisualization>();
        issueVizMock.Setup(x => x.Issue).Returns(issue);
        issueVizMock.Setup(x => x.IsResolved).Returns(issue.IsResolved);
        issueVizMock.Setup(x => x.Location).Returns(new DummyAnalysisIssueLocation { FilePath = "any.txt" });
        issueVizMock.Setup(x => x.Flows).Returns(Array.Empty<IAnalysisIssueFlowVisualization>());
        issueVizMock.SetupProperty(x => x.Span);
        issueVizMock.Object.Span = snapshotSpan;
        return issueVizMock.Object;
    }

    private static DummyAnalysisIssue CreateIssue(string path, bool isFileLevel = false, bool isResolved = false)
    {
        var analysisIssue = new DummyAnalysisIssue
        {
            PrimaryLocation = CreateIssueLocation(path, !isFileLevel),
            RuleKey = "javascript:123",
            Severity = AnalysisIssueSeverity.Blocker,
            IsResolved = isResolved
        };

        return analysisIssue;
    }

    private static DummyAnalysisIssueLocation CreateIssueLocation(string path, bool hasTextRange = true)
    {
        var issueLocation = new DummyAnalysisIssueLocation
        {
            FilePath = path,
            Message = "This is dangerous"
        };

        if (hasTextRange)
        {
            issueLocation.TextRange = new DummyTextRange();
        }
        return issueLocation;
    }

    private static ITextSnapshot CreateTextSnapshot()
    {
        var mockTextSnap = new Mock<ITextSnapshot>();
        mockTextSnap.Setup(t => t.Length).Returns(50);

        var mockTextSnapLine = new Mock<ITextSnapshotLine>();
        mockTextSnapLine.Setup(l => l.LineNumber).Returns(12);
        mockTextSnapLine.Setup(l => l.Start).Returns(new SnapshotPoint(mockTextSnap.Object, 10));

        mockTextSnap.Setup(t => t.GetLineFromPosition(25)).Returns(mockTextSnapLine.Object);
        var textSnap = mockTextSnap.Object;
        return textSnap;
    }

    private static IssuesSnapshot CreateIssueSnapshot(string projectName, Guid projectGuid, string filePath, IEnumerable<IAnalysisIssueVisualization> issues) => new(projectName, projectGuid, filePath, issues);
}
