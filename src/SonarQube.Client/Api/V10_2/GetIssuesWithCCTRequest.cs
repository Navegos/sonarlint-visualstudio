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

using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SonarQube.Client.Api.Common;
using SonarQube.Client.Api.V7_20;
using SonarQube.Client.Api.V9_6;
using SonarQube.Client.Helpers;
using SonarQube.Client.Models;

namespace SonarQube.Client.Api.V10_2
{
    /// <remarks>
    /// This class does not support component-based search. See <see cref="GetIssuesRequest"/> for more information
    /// </remarks>
    internal class GetIssuesWithCCTRequest : GetIssuesWithContextRequest
    {
        protected override SonarQubeIssue[] ParseResponse(string response)
        {
            var root = JObject.Parse(response);

            // This is a paged request so ParseResponse will be called once for each "page"
            // of the response. However, we expect each page to be self-contained, so we want
            // to rebuild the lookup each time.
            componentKeyPathLookup = root.GetComponentKeyPathLookup();

            return root["issues"]
                .ToObject<ServerIssueWithCCT[]>()
                .Select(ToSonarQubeIssue)
                .ToArray();
        }

        private SonarQubeIssue ToSonarQubeIssue(ServerIssueWithCCT issue) => new SonarQubeIssue(issue.IssueKey,
                ComputePath(issue.Component),
                issue.Hash,
                issue.Message,
                ComputeModuleKey(issue),
                issue.CompositeRuleKey,
                issue.Status == "RESOLVED",
                SonarQubeIssueSeverityConverter.Convert(issue.Severity),
                issue.CreationDate,
                issue.UpdateDate,
                issue.TextRange.ToIssueTextRange(),
                ToIssueFlows(issue.Flows),
                issue.ContextKey,
                CleanCodeTaxonomyHelpers.ToSonarQubeCleanCodeAttribute(issue.CleanCodeAttribute),
                CleanCodeTaxonomyHelpers.ToDefaultImpacts(issue.Impacts));

        private class ServerIssueWithCCT : ServerIssue
        {
            [JsonProperty("cleanCodeAttribute")]
            public string CleanCodeAttribute { get; set; }

            [JsonProperty("impacts")]
            public ServerImpact[] Impacts { get; set; }
        }
    }
}
