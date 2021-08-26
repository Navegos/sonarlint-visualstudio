﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2021 SonarSource SA
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

namespace SonarLint.VisualStudio.Core.CFamily
{
    /// <summary>
    /// Returns information if the currently open solution is one of the supported cfamily project types.
    /// Methods return false if there is no open solution.
    /// </summary>
    public interface ICFamilyProjectTypeIndicator
    {
        /// <summary>
        /// Returns true if the currently open solution is a CMake project.
        /// Returns false if the solution is not CMake, or if there is no open solution.
        /// </summary>
        bool IsCMake();
    }
}