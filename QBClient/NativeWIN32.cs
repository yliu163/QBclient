using System;
using System.Windows.Forms; // for Key namespace
using System.Runtime.InteropServices;



namespace QBClient
{
	/// <summary>
	/// Summary description for NativeWIN32.
	/// </summary>
	public class NativeWIN32
	{
		public NativeWIN32()
		{}

	

/* ------- using WIN32 Windows API in a C# application ------- */

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		static public extern IntPtr GetForegroundWindow(); // 

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		public struct STRINGBUFFER
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
			public string szText;
		}

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetWindow(IntPtr hwnd, int wFlag);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hwnd);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hWnd,  out STRINGBUFFER ClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int BM_CLICK = 0x00F5;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int VM_ESCAPE = 0x1B;
        public const int VM_RETURN = 0x0d;

		public delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr next, string sClassName, IntPtr sWindowTitle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetActiveWindow(IntPtr wHandle);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr wHandle);


        


/* ------- using HOTKEYs in a C# application -------

 in form load :
	bool success = RegisterHotKey(Handle, 100,     KeyModifiers.Control | KeyModifiers.Shift, Keys.J);

 in form closing :
	UnregisterHotKey(Handle, 100);
 

 protected override void WndProc( ref Message m )
 {	
	const int WM_HOTKEY = 0x0312; 	
	
	switch(m.Msg)	
	{	
		case WM_HOTKEY:		
			MessageBox.Show("Hotkey pressed");		
			break;	
	} 	
	base.WndProc(ref m );
}

------- using HOTKEYs in a C# application ------- */

		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool RegisterHotKey(	IntPtr hWnd, // handle to window    
			int id,            // hot key identifier    
			KeyModifiers fsModifiers,  // key-modifier options    
			Keys vk            // virtual-key code    
			); 
		
		[DllImport("user32.dll", SetLastError=true)]
		public static extern bool UnregisterHotKey(	IntPtr hWnd,  // handle to window    
			int id      // hot key identifier    
			);

		[Flags()]
			public enum KeyModifiers
		{  
			None = 0,
			Alt = 1,    
			Control = 2,    
			Shift = 4,    
			Windows = 8
		}


	}
}
