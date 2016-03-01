using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FreeSpace
{
  class Program
  {
    class NativeMethods
    {
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
      [return: MarshalAs(UnmanagedType.Bool)]
      static public extern bool GetDiskFreeSpaceEx(
        [MarshalAs(UnmanagedType.LPTStr)]
        [In] [Optional] string lpDirectoryName,
        [Out] [Optional] out ulong lpFreeBytesAvailable,
        [Out] [Optional] out ulong lpTotalNumberOfBytes,
        [Out] [Optional] out ulong lpTotalNumberOfFreeBytes
        );

    }

    static public ulong GetFreeSpace(string driveName)
    {
      ulong free = 0;
      ulong dummy1;
      ulong dummy2;
      var ret = NativeMethods.GetDiskFreeSpaceEx(driveName, out free, out dummy1, out dummy2);
      if (!ret)
      {
        throw new Win32Exception(Marshal.GetLastWin32Error());
      }
      return free;
    }

    // http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
    static String BytesToString(float byteCount)
    {
      string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
      if (byteCount == 0)
        return "0" + suf[0];
      var bytes = Math.Abs(byteCount);
      int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
      double num = Math.Round(bytes / Math.Pow(1024, place), 1);
      return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    static void ReportFreeSpace(ulong SizeinBytes,string Path)
    {
      Console.WriteLine($"Free Space in Location {Path} Is {BytesToString(SizeinBytes)}");
    }

    static void ShowHelp()
    {
      Console.WriteLine(@"Free Space on Local Drives and Network Shares");
      Console.WriteLine(@"Usage: < X: | \\Server\Share >");
    }


    static void Main(string[] args)
    {
      string path = null;

      if (args.Length == 0)
      {
        path = Directory.GetCurrentDirectory();
      }
      if (args.Length > 0)
      {
        if (args[0].StartsWith("-?") || args[0].StartsWith("--?") || args[0].StartsWith("/?") || args[0].StartsWith("--help"))
        {
          ShowHelp();
          return;
        }
        path = args[0];
      }

        //path = Path.GetPathRoot(path);
      try
      {
        var free = GetFreeSpace(path);
        ReportFreeSpace(free, path);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}

