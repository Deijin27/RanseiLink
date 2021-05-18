using RanseiWpf.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tests.Mocks
{
    public class MockDialogService : IDialogService
    {
        public int RequestRomFileCallCount = 0;
        public bool RequestRomFileReturnBool = true;
        public string RequestRomFileOutString = "rompath";
        public bool RequestRomFile(string dialogWindowTitle, out string result)
        {
            RequestRomFileCallCount++;
            result = RequestRomFileOutString;
            return RequestRomFileReturnBool;
        }

        public int ShowMessageBoxCallCount = 0;
        public MessageBoxResult ShowMessageBoxReturn = MessageBoxResult.OK;
        public MessageBoxResult ShowMessageBox(MessageBoxArgs options)
        {
            ShowMessageBoxCallCount++;
            return ShowMessageBoxReturn;
        }
    }
}
