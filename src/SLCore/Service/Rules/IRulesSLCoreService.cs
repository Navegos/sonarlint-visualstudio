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

using SonarLint.VisualStudio.SLCore.Core;
using SonarLint.VisualStudio.SLCore.Protocol;

namespace SonarLint.VisualStudio.SLCore.Service.Rules;

[JsonRpcClass("rule")]
public interface IRulesSLCoreService : ISLCoreService
{
    /// <summary>
    /// Gets Rule Meta Data from SLCORE
    /// </summary>
    Task<GetEffectiveRuleDetailsResponse> GetEffectiveRuleDetailsAsync(GetEffectiveRuleDetailsParams parameters);

    /// <summary>
    /// Lists all available standalone rule definitions
    /// </summary>
    Task<ListAllStandaloneRulesDefinitionsResponse> ListAllStandaloneRulesDefinitionsAsync();

    /// <summary>
    /// Notify the backend about changes to the standalone rule's configuration. This configuration will override defaults rule activation and parameters
    /// </summary>
    void UpdateStandaloneRulesConfiguration(UpdateStandaloneRulesConfigurationParams parameters);
}
