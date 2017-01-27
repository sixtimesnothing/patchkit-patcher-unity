﻿using System;
using System.Collections.Generic;
using System.IO;
using PatchKit.Unity.Patcher.Debug;

namespace PatchKit.Unity.Patcher.AppData.Local
{
    public class TemporaryData : ITemporaryData
    {
        private static readonly List<string> CurrentInstances = new List<string>();

        private static readonly DebugLogger DebugLogger = new DebugLogger(typeof(TemporaryData));

        private readonly string _path;

        private bool _disposed;

        private bool _writeAccess;

        public TemporaryData(string path)
        {
            AssertChecks.IsFalse(CurrentInstances.Contains(path),
                "You cannot create two instances of TemporaryData pointing to the same path.");
            Checks.ArgumentNotNullOrEmpty(path, "path");

            DebugLogger.LogConstructor();
            DebugLogger.LogVariable(path, "path");

            _path = path;

            CurrentInstances.Add(_path);
        }

        public void EnableWriteAccess()
        {
            DebugLogger.Log("Enabling write access.");

            if (!_writeAccess)
            {
                Directory.CreateDirectory(_path);
                _writeAccess = true;
            }
        }

        private void CheckWriteAccess()
        {
            AssertChecks.IsTrue(_writeAccess, "Write access is required for this operation.");
        }

        public string GetUniquePath()
        {
            CheckWriteAccess();
            AssertChecks.IsTrue(_writeAccess, "Write access is required for this operation.");

            string uniquePath = string.Empty;

            for (int i = 0; i < 1000; i++)
            {
                // Use first function if second would cause problems.
                //path = System.IO.Path.Combine(Path, Guid.NewGuid().ToString("N"));
                uniquePath = Path.Combine(_path, Path.GetRandomFileName());
                if (!File.Exists(uniquePath) && !Directory.Exists(uniquePath))
                {
                    break;
                }
            }

            return uniquePath;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemporaryData()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            CurrentInstances.Remove(_path);
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path, true);
            }

            _disposed = true;
        }
    }
}