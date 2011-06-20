using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace QBClient
{
    class WindowOperation
    {
        //when trying to close a popup window, if succeeded (even closed manually), go on; it that process does not exist (it may rarely happen, but people may close it manually); if the window is still there return false
        public const int NOTCLOSED = -1;
        public const int CLOSED = 0;
        public const int PROCESSNOTFOUND = 1;

        private const int TRYTIMES = 5;

        public static int CloseModalDialog(string processName)
        {
            Process[] localByName = Process.GetProcessesByName(processName);
            if (localByName.Length == 0)
                return PROCESSNOTFOUND;
            Process qbw32 = localByName[0];
            //Find the handle for QB frame
            System.IntPtr mwh = qbw32.MainWindowHandle;

            /* The second para: GW_ENABLEDPOPUP = 6
             * The retrieved handle identifies the enabled popup window owned by the specified 
             * window (the search uses the first such window found using GW_HWNDNEXT); otherwise, 
             * if there are no enabled popup windows, the retrieved handle is that of the specified window. 
             */
            System.IntPtr target = NativeWIN32.GetWindow(mwh, 6);

            int count = 0;
            //if target modal dialog exists
            bool isDescendent = (NativeWIN32.GetParent(target) == mwh);

            /*include the comment below might block the action for sometimes the parent 
             * handle of a modal dialog is the other modal dialog, instead of the mainwindowhandle of qbw32,
             * eg. open 'preference' change something in 'general' and go to 'finance', a popup will ask if you want to save the change
             * (NativeWIN32.GetParent(target) == mwh)&&*/

            while((target != IntPtr.Zero) && (target != mwh) && count<TRYTIMES) 
            {
                count++;
                System.IntPtr cancelButton = NativeWIN32.FindWindowEx(target, IntPtr.Zero, "MauiPushButton", "Cancel");
                System.IntPtr okButton = NativeWIN32.FindWindowEx(target, IntPtr.Zero, "MauiPushButton", "OK");
                System.IntPtr noButton = NativeWIN32.FindWindowEx(target, IntPtr.Zero, "MauiPushButton", "&No");
                System.IntPtr yesButton = NativeWIN32.FindWindowEx(target, IntPtr.Zero, "MauiPushButton", "&Yes");

                /*If the button is in a dialog box and the dialog box is not active, 
                 * the BM_CLICK message might fail. To ensure success in this situation, 
                 * call the SetActiveWindow function to activate the dialog box before sending the BM_CLICK message to the button.
                 * 
                 * The SetActiveWindow function activates a window, but not if the application is in the background.
                 * The window will be brought into the foreground (top of Z-Order) if its application is in the foreground 
                 * when the system activates the window.
                 * 
                 * If the window identified by the hWnd parameter was created by the calling thread, the active window status 
                 * of the calling thread is set to hWnd. Otherwise, the active window status of the calling thread is set to NULL.
                */

                bool isTargetFore = NativeWIN32.SetForegroundWindow(target);
                //System.IntPtr temp = NativeWIN32.SetActiveWindow(target);
                System.IntPtr temp;
                if (cancelButton != IntPtr.Zero)
                {
                    //Solution 1: hit the cancel button by SendMessage, failed
                    //NativeWIN32.SendMessage(cancelButton, NativeWIN32.WM_LBUTTONUP, 0, IntPtr.Zero); 
                    //NativeWIN32.SendMessage(cancelButton, NativeWIN32.WM_LBUTTONDOWN, 0, IntPtr.Zero); 
                    //NativeWIN32.PostMessage(cancelButton, NativeWIN32.BM_CLICK, 1, 0); 
                    //NativeWIN32.SendMessage(cancelButton, NativeWIN32.BM_CLICK, 1, IntPtr.Zero); 
                    //NativeWIN32.SendMessage(target, NativeWIN32.WM_KEYDOWN, 0, 0x1B); 
                    //NativeWIN32.SendMessage(target, NativeWIN32.WM_KEYDOWN, 0, 0); 

                    //Solution 2: hit the escape button. Wont work for Edit > Preferences, then click on any checkbox
                    //NativeWIN32.keybd_event(0x1B, 0,0,0);

                    //Solution 3: focus on the cancel button and press enter/return
                    temp = NativeWIN32.SendMessage(cancelButton, NativeWIN32.WM_LBUTTONUP, 0, IntPtr.Zero);
                    Thread.Sleep(200*count);
                    temp = NativeWIN32.SendMessage(cancelButton, NativeWIN32.WM_LBUTTONDOWN, 0, IntPtr.Zero);
                    Thread.Sleep(200*count);
                    NativeWIN32.keybd_event(NativeWIN32.VM_RETURN, 0, 0, 0);
                    Thread.Sleep(200 * count);
                }
                else if (okButton != IntPtr.Zero)
                {
                    temp = NativeWIN32.SendMessage(okButton, NativeWIN32.WM_LBUTTONUP, 0, IntPtr.Zero);
                    Thread.Sleep(200 * count);
                    temp = NativeWIN32.SendMessage(okButton, NativeWIN32.WM_LBUTTONDOWN, 0, IntPtr.Zero);
                    Thread.Sleep(200 * count);
                    NativeWIN32.keybd_event(NativeWIN32.VM_RETURN, 0, 0, 0);
                    Thread.Sleep(200 * count);
                }
                else if (noButton != IntPtr.Zero)
                {
                    temp = NativeWIN32.SendMessage(noButton, NativeWIN32.WM_LBUTTONUP, 0, IntPtr.Zero);
                    Thread.Sleep(200 * count);
                    temp = NativeWIN32.SendMessage(noButton, NativeWIN32.WM_LBUTTONDOWN, 0, IntPtr.Zero);
                    Thread.Sleep(200 * count);
                    NativeWIN32.keybd_event(NativeWIN32.VM_RETURN, 0, 0, 0);
                    Thread.Sleep(200 * count);
                }
                else
                    // close the window using API 
                    NativeWIN32.SendMessage(target, NativeWIN32.WM_SYSCOMMAND, NativeWIN32.SC_CLOSE, 0);

                System.IntPtr oldTarget = target;
                target = NativeWIN32.GetWindow(mwh, 6);
                if (target == oldTarget)
                    count = 0;
            }
            isDescendent = (NativeWIN32.GetParent(target) == mwh);
            if ((target != IntPtr.Zero) && (target != mwh) && (NativeWIN32.GetParent(target) == mwh))
                return NOTCLOSED;
            else
                return CLOSED;
        }

    }
}
