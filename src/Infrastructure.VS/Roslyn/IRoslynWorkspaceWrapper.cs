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
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using SonarLint.VisualStudio.Core;

namespace SonarLint.VisualStudio.Infrastructure.VS.Roslyn;

internal interface IRoslynWorkspaceWrapper
{
    IRoslynSolutionWrapper CurrentSolution { get; }
    bool TryApplyChanges(IRoslynSolutionWrapper solution);
}

[Export(typeof(IRoslynWorkspaceWrapper))]
[PartCreationPolicy(CreationPolicy.Shared)]
internal class RoslynWorkspaceWrapper : IRoslynWorkspaceWrapper
{
    private readonly Workspace workspace;
    private readonly IThreadHandling threadHandling;

    [ImportingConstructor]
    [ExcludeFromCodeCoverage] // not mef-testable
    public RoslynWorkspaceWrapper(VisualStudioWorkspace workspace) : this(workspace as Workspace, ThreadHandling.Instance)
    {
    }

    internal /* for testing */ RoslynWorkspaceWrapper(Workspace workspace, IThreadHandling threadHandling)
    {
        this.workspace = workspace;
        this.threadHandling = threadHandling;
    }

    public IRoslynSolutionWrapper CurrentSolution =>
        new RoslynSolutionWrapper(workspace.CurrentSolution);

    public bool TryApplyChanges(IRoslynSolutionWrapper solution)
    {
        var wasApplied = false;
        threadHandling.RunOnUIThread(() => wasApplied = workspace.TryApplyChanges(solution.GetRoslynSolution()));
        return wasApplied;
    }
}
