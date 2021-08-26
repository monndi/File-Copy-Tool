using System;
using System.Threading;

namespace File_Copy_Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the location path of the file you want to copy. Example: D:\\source\\file.txt");
                string sourcePath = Console.ReadLine();

                Console.WriteLine("Enter the location path of the destination where you want your file to be copied. Example: C:\\destination");
                string destinationPath = Console.ReadLine();
                
                Console.WriteLine("Checking the location paths...");
                System.Threading.Thread.Sleep(500);

                FileCopyTool fileCopyTool = new FileCopyTool(sourcePath: sourcePath, destinationPath: destinationPath, blockLen: 1024 * 1024);
                Console.WriteLine("\nCopying started ...");
                System.Threading.Thread.Sleep(500);
                fileCopyTool.CopyFile();

                Console.WriteLine("File Successfully Transferred.\nPress Enter to check if the file is properly copied.");
                Console.ReadLine();

                fileCopyTool.VerifyFiles();
                Console.ReadLine();

        }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
}
    }
}
