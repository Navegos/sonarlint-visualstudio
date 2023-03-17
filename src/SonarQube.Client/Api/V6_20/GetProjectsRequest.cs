﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2023 SonarSource SA
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

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SonarQube.Client.Models;
using SonarQube.Client.Requests;

namespace SonarQube.Client.Api.V6_20
{
    public class GetProjectsRequest : PagedRequestBase<SonarQubeProject>, IGetProjectsRequest
    {
        [JsonProperty("organization")]
        public virtual string OrganizationKey { get; set; }

        [JsonProperty("asc")]
        public virtual bool Ascending { get; set; } = true;

        protected override string Path => "api/components/search_projects";

        protected override SonarQubeProject[] ParseResponse(string response) =>
            JObject.Parse(response)["components"]
                .ToObject<ComponentResponse[]>()
                .Select(ToProject)
                .ToArray();

        protected override void ValidateResult(Result<SonarQubeProject[]> pageResult, List<SonarQubeProject> allResponseItems)
        {
            // We want to swallow the 10K error
            if (!pageResult.IsSuccess &&
                pageResult.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                allResponseItems.Count == 10000)
            {
                return;
            }

            pageResult.EnsureSuccess();
        }

        private SonarQubeProject ToProject(ComponentResponse response) =>
            new SonarQubeProject(response.Key, response.Name);

        private sealed class ComponentResponse
        {
            [JsonProperty("organization")]
            public string Organization { get; set; }

            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}