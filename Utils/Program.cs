﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Utils
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 1)
            {
                Clipboard.SetText(Pathing.GetUNCPath(args[0]));
            }
            else
            {
     
                Clipboard.SetText(Pathing.GetUNCPath(Clipboard.GetText()));
            }
        }
    }
}

public static class Pathing
{
    [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WNetGetConnection(
        [MarshalAs(UnmanagedType.LPTStr)] string localName,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
        ref int length);

    public static string GetUNCPath(string originalPath)
    {
        StringBuilder sb = new StringBuilder(512);
        int size = sb.Capacity;

        if (originalPath.Length > 2 && originalPath[1] == ':')
        {

            char c = originalPath[0];
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                int error = WNetGetConnection(originalPath.Substring(0, 2),
                    sb, ref size);
                if (error == 0)
                {
                    DirectoryInfo dir = new DirectoryInfo(originalPath);

                    string path = Path.GetFullPath(originalPath)
                        .Substring(Path.GetPathRoot(originalPath).Length);
                    return Path.Combine(sb.ToString().TrimEnd(), path);
                }
            }
        }

        return originalPath;
    }
}