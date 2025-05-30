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

using SonarLint.VisualStudio.Core.WPF;
using SonarLint.VisualStudio.Integration.Vsix.Resources;

namespace SonarLint.VisualStudio.Integration.Vsix.Settings.FileExclusions;

internal class ExclusionViewModel : ViewModelBase
{
    private const char InvalidCharacter = ',';
    private string error;
    private string pattern;

    public string Pattern
    {
        get => pattern;
        set
        {
            pattern = value;
            Error = GetNameValidationError();
            RaisePropertyChanged();
        }
    }

    public string Error
    {
        get => error;
        private set
        {
            error = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(Error);

    internal ExclusionViewModel(string pattern) => Pattern = pattern;

    private string GetNameValidationError()
    {
        if (string.IsNullOrWhiteSpace(Pattern))
        {
            return Strings.FileExclusions_EmptyErrorMessage;
        }
        return Pattern.Contains(InvalidCharacter) ? Strings.FileExclusions_CommaErrorMessage : null;
    }
}
