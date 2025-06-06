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
using Microsoft.VisualStudio.Threading;
using SonarLint.VisualStudio.ConnectedMode.QualityProfiles;
using SonarLint.VisualStudio.ConnectedMode.Suppressions;
using SonarLint.VisualStudio.Core.Binding;

namespace SonarLint.VisualStudio.ConnectedMode
{
    [Export(typeof(BoundSolutionUpdateHandler))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class BoundSolutionUpdateHandler : IDisposable
    {
        private readonly IActiveSolutionBoundTracker activeSolutionBoundTracker;
        private readonly IRoslynSuppressionUpdater roslynSuppressionUpdater;
        private readonly IQualityProfileUpdater qualityProfileUpdater;

        private bool disposed;

        [ImportingConstructor]
        public BoundSolutionUpdateHandler(
            IActiveSolutionBoundTracker activeSolutionBoundTracker,
            IRoslynSuppressionUpdater roslynSuppressionUpdater,
            IQualityProfileUpdater qualityProfileUpdater)
        {
            this.activeSolutionBoundTracker = activeSolutionBoundTracker;
            this.roslynSuppressionUpdater = roslynSuppressionUpdater;
            this.qualityProfileUpdater = qualityProfileUpdater;

            this.activeSolutionBoundTracker.SolutionBindingChanged += OnSolutionBindingChanged;
            this.activeSolutionBoundTracker.SolutionBindingUpdated += OnSolutionBindingUpdated;
        }

        private void OnSolutionBindingUpdated(object sender, EventArgs e) => TriggerUpdate();

        private void OnSolutionBindingChanged(object sender, ActiveSolutionBindingEventArgs e) => TriggerUpdate();

        private void TriggerUpdate()
        {
            roslynSuppressionUpdater.UpdateAllServerSuppressionsAsync().Forget();
            qualityProfileUpdater.UpdateAsync().Forget();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                activeSolutionBoundTracker.SolutionBindingChanged -= OnSolutionBindingChanged;
                activeSolutionBoundTracker.SolutionBindingUpdated -= OnSolutionBindingUpdated;
                disposed = true;
            }
        }
    }
}
