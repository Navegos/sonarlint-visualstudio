﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2024 SonarSource SA
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

using SonarLint.VisualStudio.SLCore.Protocol;

namespace SonarLint.VisualStudio.SLCore.UnitTests.Protocol;

[TestClass]
public class EitherTests
{
    [TestMethod]
    public void ImplicitConvertFrom_LeftValue_ConvertsToEquivalentEither()
    {
        LeftType value = new LeftType();

        Either<LeftType, RightType> either = value;

        either.Left.Should().BeSameAs(value);
        either.Right.Should().BeNull();
    }

    [TestMethod]
    public void ImplicitConvertFrom_RightValue_ConvertsToEquivalentEither()
    {
        RightType value = new RightType();

        Either<LeftType, RightType> either = value;

        either.Right.Should().BeSameAs(value);
        either.Left.Should().BeNull();
    }

    private class LeftType;
    private class RightType;
}