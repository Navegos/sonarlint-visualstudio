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
using SonarLint.VisualStudio.Core.WPF;
using SonarLint.VisualStudio.Core.Helpers;

namespace SonarLint.VisualStudio.ConnectedMode.UI.ServerSelection
{
    public class ServerSelectionViewModel(IConnectedModeUIServices connectedModeUiServices) : ViewModelBase
    {
        private bool isSonarCloudSelected = true;
        private bool isSonarQubeSelected;
        private bool isEuRegionSelected = true;
        private bool isUsRegionSelected;
        private string sonarQubeUrl;

        public bool IsSonarCloudSelected
        {
            get => isSonarCloudSelected;
            set
            {
                isSonarCloudSelected = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNextButtonEnabled));
                RaisePropertyChanged(nameof(ShouldSonarQubeUrlBeFilled));
            }
        }

        public bool IsSonarQubeSelected
        {
            get => isSonarQubeSelected;
            set
            {
                isSonarQubeSelected = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNextButtonEnabled));
                RaisePropertyChanged(nameof(ShouldSonarQubeUrlBeFilled));
            }
        }

        public string SonarQubeUrl
        {
            get => sonarQubeUrl;
            set
            {
                sonarQubeUrl = value.WithoutSpaces();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNextButtonEnabled));
                RaisePropertyChanged(nameof(ShouldSonarQubeUrlBeFilled));
                RaisePropertyChanged(nameof(ShowSecurityWarning));
            }
        }

        public bool IsEuRegionSelected
        {
            get => isEuRegionSelected;
            set
            {
                isEuRegionSelected = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNextButtonEnabled));
            }
        }

        public bool IsUsRegionSelected
        {
            get => isUsRegionSelected;
            set
            {
                isUsRegionSelected = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNextButtonEnabled));
            }
        }

        public bool ShowCloudRegion => connectedModeUiServices.SonarLintSettings.ShowCloudRegion;
        public bool IsNextButtonEnabled => (IsSonarCloudSelected && IsSonarCloudRegionSelected) || (IsSonarQubeSelected && IsSonarQubeUrlProvided);
        public bool ShouldSonarQubeUrlBeFilled => IsSonarQubeSelected && !IsSonarQubeUrlProvided;
        public static string SonarCloudForEuRegion => CloudServerRegion.Eu.Url.Host;
        public static string SonarCloudForUsRegion => CloudServerRegion.Us.Url.Host;
        private bool IsSonarQubeUrlProvided => !string.IsNullOrWhiteSpace(SonarQubeUrl);
        public bool ShowSecurityWarning => Uri.TryCreate(SonarQubeUrl, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme != Uri.UriSchemeHttps;
        private bool IsSonarCloudRegionSelected => IsEuRegionSelected || IsUsRegionSelected;

        public ConnectionInfo CreateTransientConnectionInfo()
        {
            var url = IsSonarQubeSelected ? SonarQubeUrl : null;
            var serverType = IsSonarCloudSelected ? ConnectionServerType.SonarCloud : ConnectionServerType.SonarQube;
            var region = IsSonarCloudSelected ? CloudServerRegion.GetRegion(IsUsRegionSelected) : null;
            return new ConnectionInfo(url, serverType, region);
        }
    }
}
