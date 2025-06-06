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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarLint.VisualStudio.Core.Synchronization;

namespace SonarLint.VisualStudio.Infrastructure.VS.UnitTests;

[TestClass]
public class AsyncLockTests
{
    [TestMethod]
    public async Task SmokeTest()
    {
        var testSubject = new AsyncLock();

        var numbers = new List<int> { 0 };
        var threadNumbers = new List<int>();

        await Task.WhenAll(Enumerable.Range(0, 10).Select(number => Task.Run(() => AddIncrement(numbers, threadNumbers, number, testSubject))));

        threadNumbers.Should().NotBeAscendingInOrder(); // checks multiple threads actually ran in parallel and not in sequence
        numbers.Should().BeInAscendingOrder(); // checks threads were correctly synchronized
    }

    private static async Task AddIncrement(List<int> numbers, List<int> threadNumbers, int assignedThreadNumber, IAsyncLock asyncLock)
    {
        for (var i = 0; i < 10000; i++)
        {
            using (await asyncLock.AcquireAsync())
            {
                threadNumbers.Add(assignedThreadNumber);
                numbers.Add(numbers[numbers.Count - 1] + 1);
            }
        }
    }
}
