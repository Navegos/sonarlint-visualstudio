﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2023 SonarSource SA
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

// Note: we have multiple enum definitions for IssueSeverity and IssueType in SLVS
// They should be consolidated if possible.
// See https://github.com/SonarSource/sonarlint-visualstudio/issues/3617

using System;
using System.Collections.Generic;

namespace SonarLint.VisualStudio.Rules
{
    public enum RuleIssueSeverity
    {
        Blocker,
        Critical,
        Major,
        Minor,
        Info,
    }

    public enum RuleIssueType
    {
        CodeSmell,      // SonarQube serialization = CODE_SMELL
        Bug,
        Vulnerability,
        Hotspot         // SonarQube serialization = SECURITY_HOTSPOT
    }

    /// <summary>
    /// Help data about a single rule, extracted using the Java plugin API
    /// </summary>
    public interface IRuleInfo
    {
        /// <summary>
        /// Unique identifier for the rule, including the repo key e.g. cpp:S123
        /// </summary>
        string FullRuleKey { get; }

        string Name { get; }

        RuleIssueSeverity DefaultSeverity { get; }

        RuleIssueType IssueType { get; }

        bool IsActiveByDefault {  get; }

        string LanguageKey { get; }

        /// <summary>
        /// The HTML description, tweaked so it can be parsed as XML
        /// </summary>
        string Description { get; }

        IReadOnlyList<string> Tags { get; }
    }

    public class RuleInfo : IRuleInfo
    {
        public RuleInfo(string languageKey, string fullRuleKey, string description, string name,
            RuleIssueSeverity defaultSeverity, RuleIssueType issueType, bool isActiveByDefault,
            IReadOnlyList<string> tags)
        {
            LanguageKey = languageKey;
            FullRuleKey = fullRuleKey;
            Description = description;
            Name = name;
            DefaultSeverity = defaultSeverity;
            IssueType = issueType;
            IsActiveByDefault = isActiveByDefault;
            Tags = tags ?? Array.Empty<string>();
        }

        public string FullRuleKey { get; private set; }

        public string Name {  get; private set; }

        public RuleIssueSeverity DefaultSeverity { get; private set; }

        public RuleIssueType IssueType { get; private set; }

        public bool IsActiveByDefault { get; private set; }

        public string LanguageKey { get; private set; }

        public string Description { get; private set; }

        public IReadOnlyList<string> Tags { get; private set; }
    }
}