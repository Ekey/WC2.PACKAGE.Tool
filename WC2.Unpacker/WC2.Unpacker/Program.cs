using System;
using System.IO;

namespace WC2.Unpacker
{
    class Program
    {
        private static String m_Title = "Wushu Chronicles 2 PACKAGE Unpacker";

        static void Main(String[] args)
        {
            Console.Title = m_Title;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(m_Title);
            Console.WriteLine("(c) 2022 Ekey (h4x0r) / v{0}\n", Utils.iGetApplicationVersion());
            Console.ResetColor();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Usage]");
                Console.WriteLine("    WC2.Unpacker <m_File> <m_Directory>\n");
                Console.WriteLine("    m_File - Source of PACKAGE archive file");
                Console.WriteLine("    m_Directory - Destination directory\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Examples]");
                Console.WriteLine("    WC2.Unpacker E:\\Games\\WC2\\wlz2\\res\\res_1.package D:\\Unpacked");
                Console.ResetColor();
                return;
            }

            String m_PackageFile = args[0];
            String m_Output = Utils.iCheckArgumentsPath(args[1]);

            if (!File.Exists(m_PackageFile))
            {
                Utils.iSetError("[ERROR]: Input PACKAGE file -> " + m_PackageFile + " <- does not exist");
                return;
            }

            PackageUnpack.iDoIt(m_PackageFile, m_Output);
        }
    }
}
