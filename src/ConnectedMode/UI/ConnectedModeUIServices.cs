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

using System.ComponentModel.Composition;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.Integration;

namespace SonarLint.VisualStudio.ConnectedMode.UI;

public interface IConnectedModeUIServices
{
    public IBrowserService BrowserService { get; }
    public IMessageBox MessageBox { get; }
    public IIDEWindowService IdeWindowService { get; }
    public ISonarLintSettings SonarLintSettings { get; }
}

[Export(typeof(IConnectedModeUIServices))]
[PartCreationPolicy(CreationPolicy.Shared)]
[method: ImportingConstructor]
public class ConnectedModeUIServices(
    IBrowserService browserService,
    IMessageBox messageBox,
    IIDEWindowService ideWindowService,
    ISonarLintSettings sonarLintSettings)
    : IConnectedModeUIServices
{
    public IBrowserService BrowserService { get; } = browserService;
    public IMessageBox MessageBox { get; } = messageBox;
    public IIDEWindowService IdeWindowService { get; } = ideWindowService;
    public ISonarLintSettings SonarLintSettings { get; } = sonarLintSettings;
}
