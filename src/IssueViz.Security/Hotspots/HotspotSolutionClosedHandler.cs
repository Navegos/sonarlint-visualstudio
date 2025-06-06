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

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using SonarLint.VisualStudio.ConnectedMode.Hotspots;
using SonarLint.VisualStudio.Core;
using SonarLint.VisualStudio.Core.Binding;

namespace SonarLint.VisualStudio.IssueVisualization.Security.Hotspots
{
    [Export(typeof(IHotspotSolutionClosedHandler))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HotspotSolutionClosedHandler : IHotspotSolutionClosedHandler, IDisposable
    {
        private readonly ILocalHotspotsStoreUpdater localHotspotsStore;
        private readonly IActiveSolutionBoundTracker activeSolutionBoundTracker;
        private readonly IThreadHandling threadHandling;

        [ImportingConstructor]
        internal HotspotSolutionClosedHandler(ILocalHotspotsStoreUpdater localHotspotsStore,
            IActiveSolutionBoundTracker activeSolutionBoundTracker,
            IThreadHandling threadHandling)
        {
            this.localHotspotsStore = localHotspotsStore;
            this.activeSolutionBoundTracker = activeSolutionBoundTracker;
            this.threadHandling = threadHandling;
            this.activeSolutionBoundTracker.SolutionBindingChanged += OnBindingChanged;
        }

        private void OnBindingChanged(object sender, ActiveSolutionBindingEventArgs e)
        {
            if (e.Configuration.Mode == SonarLintMode.Standalone)
            {
                ClearStoreAsync().Forget();
            }
        }

        private Task ClearStoreAsync()
        {
            return threadHandling.RunOnBackgroundThread(() =>
            {
                localHotspotsStore.Clear();
                return Task.FromResult(true);
            });
        }

        public void Dispose()
        {
            activeSolutionBoundTracker.SolutionBindingChanged -= OnBindingChanged;
        }
    }
}
