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

namespace SonarLint.VisualStudio.Core.Initialization;

public interface IInitializationProcessorFactory
{
    IInitializationProcessor Create<T>(IReadOnlyCollection<IRequireInitialization> dependencies, Func<IThreadHandling, Task> initialization);
}

public interface IInitializationProcessor
{
    /// <summary>
    /// Returns true if initialization was completed or failed, false if it's not been finished.
    /// Does not await for initialization to finish.
    /// </summary>
    bool IsFinalized { get; }

    /// <summary>
    /// Initializes once on a background thread using given dependencies and owner's initializer
    /// </summary>
    Task InitializeAsync();
}
