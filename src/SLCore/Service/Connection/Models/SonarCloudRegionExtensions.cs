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

using SonarLint.VisualStudio.Core.Binding;

namespace SonarLint.VisualStudio.SLCore.Service.Connection.Models;

public static class SonarCloudRegionExtensions
{
    private static readonly IReadOnlyDictionary<CloudServerRegion, SonarCloudRegion> CoreToSlCoreLanguageMap = new Dictionary<CloudServerRegion, SonarCloudRegion>()
    {
        { CloudServerRegion.Eu, SonarCloudRegion.EU }, { CloudServerRegion.Us, SonarCloudRegion.US },
    };

    public static SonarCloudRegion ToSlCoreRegion(this CloudServerRegion region)
    {
        if (CoreToSlCoreLanguageMap.TryGetValue(region, out var slCoreRegion))
        {
            return slCoreRegion;
        }
        throw new ArgumentOutOfRangeException(region.Name);
    }

    public static CloudServerRegion ToCloudServerRegion(this SonarCloudRegion slCoreRegion)
    {
        if (CoreToSlCoreLanguageMap.FirstOrDefault(x => x.Value == slCoreRegion) is { Key: not null } kvp)
        {
            return kvp.Key;
        }
        throw new ArgumentOutOfRangeException(slCoreRegion.ToString());
    }
}
