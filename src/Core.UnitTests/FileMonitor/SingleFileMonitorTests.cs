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

using System.IO;
using System.IO.Abstractions;
using Moq;
using SonarLint.VisualStudio.Core.FileMonitor;
using SonarLint.VisualStudio.TestInfrastructure;

namespace SonarLint.VisualStudio.Core.UnitTests.FileMonitor
{
    [TestClass]
    public class SingleFileMonitorTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void NonCriticalExceptions_AreSuppressed()
        {
            // Arrange
            var fileSystemMock = CreateFileSystemMock();

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger))
            {
                fileMonitor.FileChanged += (s, args) => throw new InvalidOperationException("XXX non-critical exception");

                // Act
                watcherMock
                    .Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));

                // Assert
                testLogger.AssertPartialOutputStringExists("XXX non-critical exception");
                fileMonitor.MonitoredFilePath.Should().Be("c:\\dummy\\file.txt");
            }
        }

        [TestMethod]
        public void CriticalExceptions_AreNotSuppressed()
        {
            // Arrange
            var fileSystemMock = CreateFileSystemMock();

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            Action act = () =>
            {
                watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
            };

            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger))
            {
                fileMonitor.FileChanged += (s, args) => throw new StackOverflowException("YYY critical exception");

                // Act and assert
                act.Should().ThrowExactly<StackOverflowException>().And.Message.Should().Be("YYY critical exception");
            }
        }

        [TestMethod]
        public void DirectoryDoesNotExist_IsCreated()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(false);

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            // Act
            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger))
            {
                // Assert
                fileSystemMock.Verify(x => x.Directory.Exists("c:\\dummy"), Times.Once);
                fileSystemMock.Verify(x => x.Directory.CreateDirectory("c:\\dummy"), Times.Once);
                testLogger.AssertPartialOutputStringExists("c:\\dummy");
            }
        }

        [TestMethod]
        public void DirectoryDoesExist_IsNotCreated()
        {
            // Arrange
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            // Act
            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger))
            {
                // Assert
                fileSystemMock.Verify(x => x.Directory.Exists("c:\\dummy"), Times.Once);
                fileSystemMock.Verify(x => x.Directory.CreateDirectory("c:\\dummy"), Times.Never);
                testLogger.AssertPartialOutputStringDoesNotExist("c:\\dummy");
            }
        }

        [TestMethod]
        public void OnlyRaisesEventsIfHasListeners()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");

            EventHandler dummyHandler = (sender, args) => { };

            using (var fileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var singleFileMonitor = fileMonitor as SingleFileMonitor;
                singleFileMonitor.Should().NotBeNull();

                singleFileMonitor.MonitoredFilePath.Should().Be(filePathToMonitor);

                // 1. Nothing registered -> underlying wrapper should not be raising events
                singleFileMonitor.FileWatcherIsRaisingEvents.Should().BeFalse();

                // 2. Register a listener -> start monitoring
                singleFileMonitor.FileChanged += dummyHandler;
                singleFileMonitor.FileWatcherIsRaisingEvents.Should().BeTrue();

                // 3. Unregister the listener -> stop monitoring
                singleFileMonitor.FileChanged -= dummyHandler;
                singleFileMonitor.FileWatcherIsRaisingEvents.Should().BeFalse();
            }
        }

        [TestMethod]
        public void AfterDispose_EventsAreIgnored()
        {
            Action<Mock<IFileSystemWatcher>> op = (watcherMock) => watcherMock.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, "", ""));
            DoRaise_Dispose_RaiseAgain(op);

            op = (watcherMock) => watcherMock.Raise(x => x.Deleted += null, new FileSystemEventArgs(WatcherChangeTypes.Created, "", ""));
            DoRaise_Dispose_RaiseAgain(op);

            op = (watcherMock) => watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Created, "", ""));
            DoRaise_Dispose_RaiseAgain(op);

            op = (watcherMock) => watcherMock.Raise(x => x.Renamed += null, new RenamedEventArgs(WatcherChangeTypes.Created, "", "", ""));
            DoRaise_Dispose_RaiseAgain(op);


            void DoRaise_Dispose_RaiseAgain(Action<Mock<IFileSystemWatcher>> raiseEvent)
            {
                // Arrange
                var fileSystemMock = CreateFileSystemMock();

                var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
                var testLogger = new TestLogger();

                var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger);

                var eventCount = 0;
                fileMonitor.FileChanged += (s, args) => eventCount++;

                // 1. First event is handled
                raiseEvent(watcherMock);
                eventCount.Should().Be(1);

                // 2. Dispose then re-raise
                fileMonitor.Dispose();
                raiseEvent(watcherMock);
                eventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void DisposeWhileHandlingFileEvent_DisposedWatcherIsNotCalled()
        {
            // Timing issue - event notifications happen on a different thread so we could
            // be in the middle of handling an event when the monitor is disposed. This
            // shouldn't cause an error.

            // Arrange
            var timeout = System.Diagnostics.Debugger.IsAttached ? 1000 * 60 * 5 : 1000;
            var testLogger = new TestLogger();

            var fileSystemMock  = CreateFileSystemMock();

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, "c:\\dummy\\file.txt", testLogger);

            var eventHandlerStartedEvent = new ManualResetEvent(false);
            var disposeCalledEvent = new ManualResetEvent(false);


            // Stage 1: listen to events from the monitor
            var disposedEventSignalled = false;
            fileMonitor.FileChanged += (s, args) =>
            {
                //*******************************
                // Do not assert in this callback
                //*******************************
                // We're on a background thread so assertions won't cause a test failure

                // Signal that we are in the event handler, and block until dispose is called
                eventHandlerStartedEvent.Set();
                disposedEventSignalled = disposeCalledEvent.WaitOne(timeout);
            };


            // Stage 2: raise the file system event then block until we are in the event handler
            var eventHandlerMethodTask = System.Threading.Tasks.Task.Run((Action)(() =>
                watcherMock.Raise(x => x.Created += null, new FileSystemEventArgs(WatcherChangeTypes.Created, "", ""))));

            eventHandlerStartedEvent.WaitOne(timeout).Should().BeTrue();


            // Stage 3: dispose the monitor, unblock the background thread and wait until it has finished
            watcherMock.Reset(); // reset the counts of calls to the watcher

            fileMonitor.Dispose();
            disposeCalledEvent.Set();

            eventHandlerMethodTask.Wait(timeout).Should().BeTrue();
            disposedEventSignalled.Should().BeTrue();

            // Expect a single call to watcher.Dispose(), and the event handlers to be unregistered
            watcherMock.Verify(x => x.Dispose(), Times.Once);
            watcherMock.VerifyRemove(x => x.Changed -= It.IsAny<FileSystemEventHandler>(), Times.Once);
            watcherMock.VerifyRemove(x => x.Created -= It.IsAny<FileSystemEventHandler>(), Times.Once);
            watcherMock.VerifyRemove(x => x.Deleted -= It.IsAny<FileSystemEventHandler>(), Times.Once);
            watcherMock.VerifyRemove(x => x.Renamed -= It.IsAny<RenamedEventHandler>(), Times.Once);

            watcherMock.VerifyNoOtherCalls();
        }

        #region Simple file operations
        // Check simple file operations are handled and don't produce duplicates

        [TestMethod]
        public void RealFile_FileCreationIsTracked()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");

            using (var singleFileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var testWrapper = new WaitableFileMonitor(singleFileMonitor);
                testWrapper.EventCount.Should().Be(0);

                // Act: create the file
                File.WriteAllText(filePathToMonitor, "initial text");
                testWrapper.WaitForEventAndThrowIfMissing();
                testWrapper.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void RealFile_FileChangeIsTracked()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");
            File.WriteAllText(filePathToMonitor, "contents");

            using (var singleFileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var testWrapper = new WaitableFileMonitor(singleFileMonitor);
                testWrapper.EventCount.Should().Be(0);

                // Act: amend the file
                File.AppendAllText(filePathToMonitor, " more text");
                testWrapper.WaitForEventAndThrowIfMissing();
                testWrapper.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void RealFile_FileDeletionIsTracked()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");
            File.WriteAllText(filePathToMonitor, "contents");

            using (var singleFileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var testWrapper = new WaitableFileMonitor(singleFileMonitor);
                testWrapper.EventCount.Should().Be(0);

                // Act: delete the file
                File.Delete(filePathToMonitor);
                testWrapper.WaitForEventAndThrowIfMissing();
                testWrapper.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void RealFile_RenameFromTrackedFileName_IsTracked()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");
            File.WriteAllText(filePathToMonitor, "contents");

            using (var singleFileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var testWrapper = new WaitableFileMonitor(singleFileMonitor);
                testWrapper.EventCount.Should().Be(0);

                // Act: rename the file from the tracked name
                var renamedFile = Path.ChangeExtension(filePathToMonitor, "moved");
                File.Move(filePathToMonitor, renamedFile);

                testWrapper.WaitForEventAndThrowIfMissing();
                testWrapper.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void RealFile_RenameToTrackedFileName_IsTracked()
        {
            // Arrange
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");
            var otherFilePath = Path.ChangeExtension(filePathToMonitor, "other");
            File.WriteAllText(otherFilePath, "contents");

            using (var singleFileMonitor = CreateTestSubject(filePathToMonitor, testLogger))
            {
                var testWrapper = new WaitableFileMonitor(singleFileMonitor);
                testWrapper.EventCount.Should().Be(0);

                // Act: rename the file to the tracked name
                File.Move(otherFilePath, filePathToMonitor);

                testWrapper.WaitForEventAndThrowIfMissing();
                testWrapper.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void WatchesProvidedFile()
        {
            var testDir = CreateTestSpecificDirectory();
            var filePathToMonitor = Path.Combine(testDir, "settingsFile.txt");
            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);

            using var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, CreateFileSystemMock().Object, filePathToMonitor, new TestLogger());

            fileMonitor.MonitoredFilePath.Should().Be(filePathToMonitor);
            watcherMock.VerifySet(mock => mock.Path = Path.GetDirectoryName(filePathToMonitor), Times.Once());
            watcherMock.VerifySet(mock => mock.Filter = Path.GetFileName(filePathToMonitor), Times.Once());
            watcherMock.VerifySet(mock => mock.NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.LastWrite |
                                                              NotifyFilters.FileName | NotifyFilters.DirectoryName), Times.Once());
        }

        [TestMethod]
        public void File_DuplicateChangesSameTime_AreIgnored()
        {
            var filePath = "test\\";

            var fileSystemMock = CreateFileSystemMock();

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, filePath, testLogger))
            {
                var waitableFileMonitor = new WaitableFileMonitor(fileMonitor);
                waitableFileMonitor.EventCount.Should().Be(0);
                watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, filePath , ""));
                watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, filePath, ""));
                waitableFileMonitor.EventCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void File_DuplicateChangesDifferentTime_NotIgnored()
        {
            var filePath = "test\\";

            var fileSystemMock = CreateFileSystemMock();

            var watcherFactoryMock = CreateFactoryAndWatcherMocks(out var watcherMock);
            var testLogger = new TestLogger(logToConsole: true, logThreadId: true);

            var dateTime = DateTime.Now;

            using (var fileMonitor = new SingleFileMonitor(watcherFactoryMock.Object, fileSystemMock.Object, filePath, testLogger))
            {
                var waitableFileMonitor = new WaitableFileMonitor(fileMonitor);
                waitableFileMonitor.EventCount.Should().Be(0);

                fileSystemMock.Setup(x => x.File.GetLastWriteTimeUtc(filePath)).Returns(dateTime);
                watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, filePath, ""));

                fileSystemMock.Setup(x => x.File.GetLastWriteTimeUtc(filePath)).Returns(dateTime.AddMilliseconds(1));
                watcherMock.Raise(x => x.Changed += null, new FileSystemEventArgs(WatcherChangeTypes.Changed, filePath, ""));

                waitableFileMonitor.EventCount.Should().Be(2);
            }
        }

        private Mock<IFileSystem> CreateFileSystemMock()
        {
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.Directory.Exists(It.IsAny<string>())).Returns(true);
            fileSystemMock.Setup(x => x.File.GetLastWriteTimeUtc(It.IsAny<string>())).Returns(DateTime.MaxValue);

            return fileSystemMock;
        }

        private static Mock<IFileSystemWatcherFactory> CreateFactoryAndWatcherMocks(out Mock<IFileSystemWatcher> watcherMock)
        {
            watcherMock = new Mock<IFileSystemWatcher>();
            var watcherFactoryMock = new Mock<IFileSystemWatcherFactory>();
            watcherFactoryMock
                .Setup(x => x.CreateNew())
                .Returns(watcherMock.Object);
            return watcherFactoryMock;
        }

        #endregion Simple file operations

        private static ISingleFileMonitor CreateTestSubject(string filePathToMonitor, ILogger logger)
        {
            return new SingleFileMonitorFactory(logger).Create(filePathToMonitor);
        }

        private string CreateTestSpecificDirectory()
        {
            var testPath = Path.Combine(TestContext.DeploymentDirectory, TestContext.TestName);
            Directory.CreateDirectory(testPath);
            return testPath;
        }

        /// <summary>
        /// Decorator class that adds waiting for events with a timeout to
        /// make testing simpler and more reliable
        /// </summary>
        private class WaitableFileMonitor
        {
            private readonly ISingleFileMonitor singleFileMonitor;

            public WaitableFileMonitor(ISingleFileMonitor singleFileMonitor)
            {
                this.singleFileMonitor = singleFileMonitor;
                this.singleFileMonitor.FileChanged += OnFileChanged;
            }

            private readonly AutoResetEvent signal = new AutoResetEvent(false);

            public int EventCount { get; private set; }

            public void WaitForEventAndThrowIfMissing()
            {
                var timeout = System.Diagnostics.Debugger.IsAttached ? 1000 * 60 * 5 : 1000;
                var signaled = signal.WaitOne(timeout);
                signaled.Should().Be(true); // throw if we timed out
            }

            private void OnFileChanged(object sender, EventArgs args)
            {
                EventCount++;
                signal.Set();
            }
        }
    }
}
