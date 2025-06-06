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

using SonarLint.VisualStudio.Core.SystemAbstractions;
using SonarLint.VisualStudio.TestInfrastructure;

namespace SonarLint.VisualStudio.Core.UnitTests.SystemAbstractions;

[TestClass]
public class EmbeddedResourceReaderTests
{
    private const string ResourceFileName = "SonarLint.VisualStudio.Core.UnitTests.SystemAbstractions.TestData.txt";
    private const string ResourceFileContent = "// this is a test file\r\n";
    private EmbeddedResourceReader testSubject;

    [TestInitialize]
    public void TestInitialize() => testSubject = new EmbeddedResourceReader();

    [TestMethod]
    public void MefCtor_CheckIsExported() => MefTestHelpers.CheckTypeCanBeImported<EmbeddedResourceReader, IEmbeddedResourceReader>();

    [TestMethod]
    public void MefCtor_IsSingleton() => MefTestHelpers.CheckIsSingletonMefComponent<EmbeddedResourceReader>();

    [TestMethod]
    public void GetResourceContent_ValidResource_ReturnsContent()
    {
        var result = testSubject.Read(GetType().Assembly, ResourceFileName);

        result.Should().Be(ResourceFileContent);
    }

    [TestMethod]
    public void GetResourceContent_InvalidResource_ReturnsNull()
    {
        var result = testSubject.Read(GetType().Assembly, "invalidresourcename");

        result.Should().BeNull();
    }
}
