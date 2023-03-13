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

using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace SonarLint.VisualStudio.Education.Layout.Visual
{
    /// <summary>
    /// Represents section with a header that has a configurable set of content subsections
    /// </summary>
    internal class MultiBlockSection : IAbstractVisualizationTreeNode
    {
        internal /* for testing */ readonly IList<IAbstractVisualizationTreeNode> blocks;

        public MultiBlockSection(params IAbstractVisualizationTreeNode[] blocks)
        {
            this.blocks = blocks;
        }

        public MultiBlockSection(IList<IAbstractVisualizationTreeNode> blocks)
        {
            this.blocks = blocks;
        }

        public Block CreateVisualization()
        {
            throw new NotImplementedException();
            // var container = new Section();
            //
            // foreach (var block in blocks)
            // {
            //     container.Blocks.Add(block.CreateVisualization());
            // }
            //
            // return container;
        }
    }
}