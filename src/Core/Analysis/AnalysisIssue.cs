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

namespace SonarLint.VisualStudio.Core.Analysis
{
    public class AnalysisIssue : IAnalysisIssue
    {
        private static readonly IReadOnlyList<IAnalysisIssueFlow> EmptyFlows = [];
        private static readonly IReadOnlyList<IQuickFix> EmptyFixes = [];

        public AnalysisIssue(
            Guid? id,
            string ruleKey,
            string issueServerKey,
            bool isResolved,
            AnalysisIssueSeverity? severity,
            AnalysisIssueType? type,
            Impact highestImpact,
            IAnalysisIssueLocation primaryLocation,
            IReadOnlyList<IAnalysisIssueFlow> flows,
            IReadOnlyList<IQuickFix> fixes = null)
        {
            Id = id;
            RuleKey = ruleKey;
            IssueServerKey = issueServerKey;
            IsResolved = isResolved;
            Severity = severity;
            HighestImpact = highestImpact;
            Type = type;
            PrimaryLocation = primaryLocation ?? throw new ArgumentNullException(nameof(primaryLocation));
            Flows = flows ?? EmptyFlows;
            Fixes = fixes ?? EmptyFixes;
        }

        public Guid? Id { get; }

        public string RuleKey { get; }

        public AnalysisIssueSeverity? Severity { get; }

        public AnalysisIssueType? Type { get; }

        public IReadOnlyList<IAnalysisIssueFlow> Flows { get; }

        public IAnalysisIssueLocation PrimaryLocation { get; }
        public bool IsResolved { get; }
        public string IssueServerKey { get; }

        public IReadOnlyList<IQuickFix> Fixes { get; }
        public Impact HighestImpact { get; }
    }

    public class AnalysisHotspotIssue : AnalysisIssue, IAnalysisHotspotIssue
    {
        public AnalysisHotspotIssue(
            Guid? id,
            string ruleKey,
            string issueServerKey,
            bool isResolved,
            AnalysisIssueSeverity? severity,
            AnalysisIssueType? type,
            Impact highestImpact,
            IAnalysisIssueLocation primaryLocation,
            IReadOnlyList<IAnalysisIssueFlow> flows,
            HotspotStatus hotspotStatus,
            IReadOnlyList<IQuickFix> fixes = null,
            HotspotPriority? hotspotPriority = null) :
            base(id, ruleKey, issueServerKey, isResolved, severity, type, highestImpact, primaryLocation, flows, fixes)
        {
            HotspotPriority = hotspotPriority;
            HotspotStatus = hotspotStatus;
        }

        public HotspotPriority? HotspotPriority { get; }
        public HotspotStatus HotspotStatus { get; }
    }

    public class AnalysisIssueFlow : IAnalysisIssueFlow
    {
        public AnalysisIssueFlow(IReadOnlyList<IAnalysisIssueLocation> locations)
        {
            Locations = locations ?? throw new ArgumentNullException(nameof(locations));
        }

        public IReadOnlyList<IAnalysisIssueLocation> Locations { get; }
    }

    public class AnalysisIssueLocation : IAnalysisIssueLocation
    {
        public AnalysisIssueLocation(string message, string filePath, ITextRange textRange)
        {
            Message = message;
            FilePath = filePath;
            TextRange = textRange;
        }

        public string FilePath { get; }

        public string Message { get; }

        public ITextRange TextRange { get; }
    }

    public class TextRange : ITextRange
    {
        public TextRange(
            int startLine,
            int endLine,
            int startLineOffset,
            int endLineOffset,
            string lineHash)
        {
            StartLine = startLine;
            EndLine = endLine;
            StartLineOffset = startLineOffset;
            EndLineOffset = endLineOffset;
            LineHash = lineHash;
        }

        public int StartLine { get; }
        public int EndLine { get; }
        public int StartLineOffset { get; }
        public int EndLineOffset { get; }
        public string LineHash { get; }
    }
}
