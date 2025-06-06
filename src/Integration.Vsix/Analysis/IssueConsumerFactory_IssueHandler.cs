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

using Microsoft.VisualStudio.Text;
using SonarLint.VisualStudio.IssueVisualization.Models;
using SonarLint.VisualStudio.IssueVisualization.Security.Hotspots;

namespace SonarLint.VisualStudio.Integration.Vsix.Analysis
{
    partial class IssueConsumerFactory
    {
        internal interface IIssueHandler
        {
            void HandleNewIssues(IEnumerable<IAnalysisIssueVisualization> issues);

            void HandleNewHotspots(IEnumerable<IAnalysisIssueVisualization> hotspots);
        }

        internal class IssueHandler : IIssueHandler
        {
            // We can't mock the span translation code (to difficult to mock),
            // so we use this delegate to by-passes it in tests.
            // See https://github.com/SonarSource/sonarlint-visualstudio/issues/1522
            internal /* for testing */ delegate IAnalysisIssueVisualization[] TranslateSpans(IEnumerable<IAnalysisIssueVisualization> issues, ITextSnapshot activeSnapshot);

            private readonly ITextDocument textDocument;
            private readonly string projectName;
            private readonly Guid projectGuid;
            private readonly SnapshotChangedHandler onSnapshotChanged;
            private readonly ILocalHotspotsStoreUpdater localHotspotsStore;

            private readonly TranslateSpans translateSpans;

            public IssueHandler(
                ITextDocument textDocument,
                string projectName,
                Guid projectGuid,
                SnapshotChangedHandler onSnapshotChanged,
                ILocalHotspotsStoreUpdater localHotspotsStore)
                : this(textDocument, projectName, projectGuid, onSnapshotChanged, localHotspotsStore, DoTranslateSpans)
            {
            }

            internal /* for testing */ IssueHandler(
                ITextDocument textDocument,
                string projectName,
                Guid projectGuid,
                SnapshotChangedHandler onSnapshotChanged,
                ILocalHotspotsStoreUpdater localHotspotsStore,
                TranslateSpans translateSpans)
            {
                this.textDocument = textDocument;
                this.projectName = projectName;
                this.projectGuid = projectGuid;
                this.onSnapshotChanged = onSnapshotChanged;
                this.localHotspotsStore = localHotspotsStore;

                this.translateSpans = translateSpans;
            }

            public void HandleNewIssues(IEnumerable<IAnalysisIssueVisualization> issues)
            {
                var translatedIssues = PrepareIssues(issues);
                var newSnapshot = new IssuesSnapshot(projectName, projectGuid, textDocument.FilePath, translatedIssues);
                onSnapshotChanged(newSnapshot);
            }

            public void HandleNewHotspots(IEnumerable<IAnalysisIssueVisualization> hotspots)
            {
                var translatedIssues = PrepareIssues(hotspots);
                localHotspotsStore.UpdateForFile(textDocument.FilePath, translatedIssues);
            }

            private IAnalysisIssueVisualization[] PrepareIssues(IEnumerable<IAnalysisIssueVisualization> issues)
            {
                // The text buffer might have changed since the analysis was triggered, so translate
                // all issues to the current snapshot.
                // See bug #1487: https://github.com/SonarSource/sonarlint-visualstudio/issues/1487
                var translatedIssues = translateSpans(issues, textDocument.TextBuffer.CurrentSnapshot);
                return translatedIssues;
            }

            private static IAnalysisIssueVisualization[] DoTranslateSpans(IEnumerable<IAnalysisIssueVisualization> issues, ITextSnapshot activeSnapshot)
            {
                var issuesWithTranslatedSpans = issues
                    .Where(x => x.Span.HasValue)
                    .Select(x =>
                    {
                        var oldSpan = x.Span.Value;
                        var newSpan = oldSpan.TranslateTo(activeSnapshot, SpanTrackingMode.EdgeExclusive);
                        x.Span = oldSpan.Length == newSpan.Length ? newSpan : (SnapshotSpan?)null;
                        return x;
                    })
                    .Where(x => x.Span.HasValue)
                    .Union(issues.Where(i => i.IsFileLevel()))
                    .ToArray();

                return issuesWithTranslatedSpans;
            }
        }
    }
}
