using System;
using System.Threading; 
using System.IO; 
using System.Diagnostics;
using System.Runtime.InteropServices;

//todo: start as shortcut scenario:
//create event here and set it in the cscs.exe as mutex climed in the cscs.exe may be released if cscs.exe does not live long enough
//so css would beleive console should not be visible
class Script
{
	static bool done = false;

	const int SW_HIDE = 0;
	const int SW_SHOW = 5;
	
	[DllImport("User32")]
	static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
	
	[DllImport("kernel32")]
	static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")] 
	private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr ProcessId);

	[DllImport("kernel32", SetLastError = true)]
	static extern int SetConsoleMode(int hConsoleHandle, int dwMode);

	//[DllImport("kernel32.dll")]
	//static extern bool TerminateThread(IntPtr hThread, uint dwExitCod);

	static Process myProcess = new Process();
	static Thread inputThread = null;
	static bool consoleVisibilityForced = false;
	static bool cmdConsole = false;
	static bool nologo = false;
	static int lineCount = 0;

	static public void Main(string[] args)
	{
		try
		{
			IntPtr hwnd = GetConsoleWindow();
			if (hwnd != IntPtr.Zero)
			{
				IntPtr processId = IntPtr.Zero;
				GetWindowThreadProcessId(hwnd, out processId);
				if (Process.GetCurrentProcess().Id == (int)processId)
					ShowWindow(hwnd, SW_HIDE);
				else
					cmdConsole = true;
			}

			myProcess.StartInfo.FileName = "cscs.exe";

			string arguments = "";
			foreach (string arg in args)
			{
				if (arg == "/nl")
					nologo = true;
				arguments += "\"" + arg + "\" ";
			}
			myProcess.StartInfo.Arguments = arguments;

			myProcess.StartInfo.UseShellExecute = false;
			myProcess.StartInfo.RedirectStandardOutput = true;
			myProcess.StartInfo.RedirectStandardInput = true;
			myProcess.StartInfo.CreateNoWindow = true;
			myProcess.Start();

			Thread workingThread = new Thread(new ThreadStart(HandleOutput));
			workingThread.IsBackground = true;
			workingThread.Start();

			inputThread = new Thread(new ThreadStart(HandleInput));
			inputThread.IsBackground = true;
			inputThread.Start();

			myProcess.WaitForExit();

			while (!done)
				Thread.Sleep(100);

			//inputThread.Abort(); //need, but cannot kill HandleInput thread

			Thread.Sleep(100);
			if (!cmdConsole && consoleVisibilityForced)
			{
				SetConsoleMode(3, 0);
				Console.WriteLine("\n\nScript is completed...");
				//while(true)
					Console.Read();
			}
		}
		catch
		{
		}
	}
	
	static void HandleOutput()
	{
		try
		{
			int key = 0;
			while (-1 != (key = myProcess.StandardOutput.Read()))
			{
				if (!cmdConsole && !consoleVisibilityForced)
				{
					if (key == 10)
						lineCount++;
					if (nologo || (!nologo && lineCount > 3)) //cscs.exe has finished printing logo
					{
						ShowWindow(GetConsoleWindow(), SW_SHOW);
						consoleVisibilityForced = true;
					}
				}
				Console.Write(Convert.ToChar(key));
			}
			done = true;
		}
		catch
		{
		}
	}

	static void HandleInput()
	{
		try
		{
			//string line;
			//while (null != (line = Console.ReadLine()))
			//	myProcess.StandardInput.WriteLine(line);

			ConsoleKeyInfo cki;
			while (true)
			{
				cki = Console.ReadKey(false);
				if (cki.Key == ConsoleKey.Enter)
					myProcess.StandardInput.WriteLine("");
				else
					myProcess.StandardInput.Write(cki.KeyChar);
			}
		}
		catch
		{
		}
	}
}

