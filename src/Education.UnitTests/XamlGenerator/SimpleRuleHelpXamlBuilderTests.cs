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

using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SonarLint.VisualStudio.Education.Rule;
using SonarLint.VisualStudio.Education.XamlGenerator;
using SonarLint.VisualStudio.TestInfrastructure;

namespace SonarLint.VisualStudio.Education.UnitTests.XamlGenerator
{
    [TestClass]
    public class SimpleRuleHelpXamlBuilderTests
    {
        [TestMethod]
        public void MefCtor_CheckExports()
        {
            MefTestHelpers.CheckTypeCanBeImported<SimpleRuleHelpXamlBuilder, ISimpleRuleHelpXamlBuilder>(
                MefTestHelpers.CreateExport<IRuleHelpXamlTranslatorFactory>(), MefTestHelpers.CreateExport<IXamlGeneratorHelperFactory>(), MefTestHelpers.CreateExport<IXamlWriterFactory>());
        }

        [TestMethod]
        public void Create_FormsCorrectStructure()
        {
            var callSequence = new MockSequence();
            var description = "<p>Hi</p>";
            var ruleHelpXamlTranslatorFactoryMock = new Mock<IRuleHelpXamlTranslatorFactory>(MockBehavior.Strict);
            var ruleHelpXamlTranslatorMock = new Mock<IRuleHelpXamlTranslator>(MockBehavior.Strict);
            var xamlGeneratorHelperFactoryMock = new Mock<IXamlGeneratorHelperFactory>(MockBehavior.Strict);
            var xamlGeneratorHelperMock = new Mock<IXamlGeneratorHelper>(MockBehavior.Strict);
            var ruleInfoMock = new Mock<IRuleInfo>(MockBehavior.Strict);
            var xamlWriterFactoryMock = new Mock<IXamlWriterFactory>(MockBehavior.Strict);
            XmlWriter writer = null;

            ruleHelpXamlTranslatorFactoryMock
                .InSequence(callSequence)
                .Setup(x => x.Create())
                .Returns(ruleHelpXamlTranslatorMock.Object);
            xamlWriterFactoryMock
                .InSequence(callSequence)
                .Setup(x => x.Create(It.IsAny<StringBuilder>()))
                .Returns((StringBuilder sb) =>
                {
                    writer = new XamlWriterFactory().Create(sb);
                    return writer;
                });
            xamlGeneratorHelperFactoryMock
                .InSequence(callSequence)
                .Setup(x => x.Create(It.IsAny<XmlWriter>()))
                .Returns(xamlGeneratorHelperMock.Object);
            xamlGeneratorHelperMock
                .InSequence(callSequence)
                .Setup(x => x.WriteDocumentHeader(ruleInfoMock.Object))
                .Callback(() => { writer.WriteStartElement("FlowDocument", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"); });
            ruleInfoMock
                .InSequence(callSequence)
                .SetupGet(x => x.Description)
                .Returns(description);
            ruleHelpXamlTranslatorMock
                .InSequence(callSequence)
                .Setup(x => x.TranslateHtmlToXaml(description))
                .Returns("<Paragraph>Hi</Paragraph>");
            xamlGeneratorHelperMock
                .InSequence(callSequence)
                .Setup(x => x.EndDocument())
                .Callback(() =>
                {
                    writer.WriteFullEndElement();
                    writer.Close();
                });

            var testSubject = new SimpleRuleHelpXamlBuilder(ruleHelpXamlTranslatorFactoryMock.Object, xamlGeneratorHelperFactoryMock.Object, xamlWriterFactoryMock.Object);

            var flowDocument = testSubject.Create(ruleInfoMock.Object);

            flowDocument.Blocks.Single().Should().BeOfType<Paragraph>().Which.Inlines.Single().Should().BeOfType<Run>().Which.Text.Should().Be("Hi");
        }
    }
}
