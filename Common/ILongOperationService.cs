﻿using System;

namespace Common
{
    /// <summary>
    /// Service for blocking screen when performing long operations
    /// </summary>
    public interface ILongOperationService
    {
        string BlockingMessage { get; }
        string GeneralMessage { get; }
        bool IsBusy { get; }
        bool IsBlockingBusy { get; }

        void ShowMessage(string message);

        void StartLongBlockingOperation(string message);

        void FinishLongBlockingOperation();

        void StartLongOperation(string message);

        void FinishLongOperation(string message);

        void HandleException(Exception ex, string errorMessage=default);
    }
}