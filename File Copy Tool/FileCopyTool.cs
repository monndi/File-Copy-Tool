using System;
using System.IO;
using System.Linq;
using System.Text;

namespace File_Copy_Tool
{
    public class FileCopyTool
    {
        private string _sourcePath;
        private string _destinationPath;

        private readonly int _blockLen;

        /// <summary>
        /// Provide file transfer tool to securely copy a large file from source file location path to destination file location path using block of bytes for transferring.
        /// </summary>
        /// <param name="sourcePath">The location path of the file to be copied.</param>
        /// <param name="destinationPath">The location path where the file needs to be copied.</param>
        /// <param name="blockLen">Length of the bytes transferring block</param>
        public FileCopyTool(string sourcePath, string destinationPath, int blockLen)
        {
            ValidateFileLocationPaths(sourcePath, destinationPath);
            _blockLen = blockLen; 
        }
        

        // Check if the given location paths are valid.
        private void ValidateFileLocationPaths(string sourcePath, string destinationPath)
        {

            FileInfo sourceFile = new FileInfo(sourcePath);
            if (!sourceFile.Exists) throw new FileNotFoundException($"File with path: {sourcePath} does not exists!");

            destinationPath = Path.Combine(destinationPath, sourceFile.Name);
            FileInfo destinationFile = new FileInfo(destinationPath);

            if (destinationFile.Exists)
            {
                Console.WriteLine($"The file with path:{destinationPath} already exists!\nDo you want to replace it? y/n");
                var confirm = Console.ReadKey().Key.ToString();
                if (!(confirm.ToUpper() == ConsoleKey.Y.ToString() || confirm == ConsoleKey.Enter.ToString())) throw new CopyingCanceledException("\n!!!Copying Canceled!!!");
            }
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
        }

        /// <summary>
        /// Copy file securely using MD5.
        /// </summary>
        public void CopyFile()
        {
            long position = 0;
            long totalBytesTransferred = 0;
            byte[] blockBuffer = new byte[_blockLen];
            int bytesLoad = 0;
            using (FileStream _sourceStream = File.Open(_sourcePath, FileMode.Open))
            {
                using (FileStream _destinationStream = File.Create(_destinationPath))
                {
                    while (true)
                    {
                        Array.Clear(blockBuffer, 0, _blockLen);

                        position = _sourceStream.Position;
                        bytesLoad = _sourceStream.Read(blockBuffer, 0, _blockLen);

                        if (bytesLoad == 0) break;
                       
                        _destinationStream.Write(blockBuffer, 0, bytesLoad);
                        _destinationStream.Flush();

                        if (VerifyTransferredBytes(_sourceStream, _destinationStream, position))
                        {
                            Console.WriteLine("Block succesfully copied!");
                            totalBytesTransferred += bytesLoad;
                            if (bytesLoad < _blockLen)
                            {
                                _destinationStream.SetLength(totalBytesTransferred);
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Block re-submitted!");
                            _sourceStream.Seek(position, SeekOrigin.Begin);
                            _destinationStream.Seek(position, SeekOrigin.Begin);
                        }
                    }
                }
            }
        }

        // Check if the block of bytes is properly transferred using MD5 Hash.
        private bool VerifyTransferredBytes(FileStream sourceStream, FileStream destinationStream, long position)
        {
            byte[] sourceBlockBuffer = new byte[_blockLen];
            byte[] destinationBlockBuffer = new byte[_blockLen];

            sourceStream.Seek(position, SeekOrigin.Begin);
            int bytesLoad = sourceStream.Read(sourceBlockBuffer, 0, _blockLen);

            destinationStream.Seek(position, SeekOrigin.Begin);
            destinationStream.Read(destinationBlockBuffer, 0, _blockLen);

            if (FileCopyToolUtils.CompareMD5Hash(sourceBlockBuffer.Take(bytesLoad).ToArray(), destinationBlockBuffer.Take(bytesLoad).ToArray())) return true;
            else return false;
        }
        /// <summary>
        /// Check if the file is properly copied.
        /// </summary>
        public void VerifyFiles()
        {
            FileInfo sourceFile = new FileInfo(_sourcePath);
            FileInfo copiedFile = new FileInfo(_destinationPath);

            if (sourceFile.Length != copiedFile.Length)
            {
                Console.WriteLine("The file is corrupted!");
                return;
            }

            using (FileStream _sourceStream = File.Open(_sourcePath, FileMode.Open))
            {
                using (FileStream _destinationStream = File.Open(_destinationPath, FileMode.Open))
                {
                    string sourceHash = FileCopyToolUtils.GetSHA1Hash(_sourceStream);
                    string destinationHash = FileCopyToolUtils.GetSHA1Hash(_destinationStream);

                    if (!(sourceHash.CompareTo(destinationHash) == 0)) Console.WriteLine("The file is corrupted!");
                    else
                    {
                        Console.WriteLine($"Source file checksum: {sourceHash}");
                        Console.WriteLine($"Destination file checksum: {destinationHash}");
                        Console.WriteLine("Print checksum of each block?");
                        Console.ReadLine();
                        PrintBlockHashedValues(_sourceStream, _destinationStream);
                    }
                }
            }
        }

        // Print checksum of each block and position of the block in the source file.
        private void PrintBlockHashedValues(FileStream _sourceStream, FileStream _destinationStream)
        {
            int bufferLen = 1024;
            byte[] sourceBlockBuffer = new byte[bufferLen];
            byte[] destBlockBuffer = new byte[bufferLen];
            
            long position;

            _sourceStream.Seek(0, SeekOrigin.Begin);
            _destinationStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                Array.Clear(sourceBlockBuffer, 0, bufferLen);
                Array.Clear(destBlockBuffer, 0, bufferLen);
                position = _sourceStream.Position;

                int bytesRead = _sourceStream.Read(sourceBlockBuffer, 0, bufferLen);
                _destinationStream.Read(destBlockBuffer, 0, bufferLen);

                if (bytesRead == 0) break;

                string souceHash = FileCopyToolUtils.GetSHA1Hash(sourceBlockBuffer);
                string destHash = FileCopyToolUtils.GetSHA1Hash(destBlockBuffer);

                if (souceHash.CompareTo(destHash) != 0) { Console.WriteLine("The files are different"); break; }

                Console.WriteLine($"Position: {position}, hash: {souceHash}");
                if (bytesRead < bufferLen)
                {
                    Console.WriteLine("The file has been securely transferred!");
                    break;
                }
            }
        }
    }
}
