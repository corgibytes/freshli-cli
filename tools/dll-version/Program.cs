// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

var info = FileVersionInfo.GetVersionInfo(args[0]);
Console.WriteLine(info.ProductVersion);
