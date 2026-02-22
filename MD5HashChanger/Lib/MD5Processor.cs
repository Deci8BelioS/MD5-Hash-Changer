using System.Security.Cryptography;
using System.Text;

namespace MD5_Hash_Changer.Lib;

public static class MD5Processor
{
    private static readonly Random rng = new Random();

    public static string GetMD5(string file)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(file);
        byte[] hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static void ApplyHashChange(string filePath, out string oldMd5, out string newMd5)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        oldMd5 = GetMD5(filePath);
        byte[] signature = Encoding.ASCII.GetBytes("HCHG");
        string seedData = $"{fileInfo.Name}|{fileInfo.Length}|{DateTime.UtcNow.Ticks}";
        byte[] metadataHash;
        using (MD5 md5Padding = MD5.Create()) metadataHash = md5Padding.ComputeHash(Encoding.UTF8.GetBytes(seedData));
        int extraRandomSize = rng.Next(11, 44);
        byte[] extraRandomBytes = new byte[extraRandomSize];
        rng.NextBytes(extraRandomBytes);
        int totalPaddingSize = metadataHash.Length + extraRandomBytes.Length + 1 + signature.Length;
        byte[] finalPadding = new byte[totalPaddingSize];
        Buffer.BlockCopy(metadataHash, 0, finalPadding, 0, metadataHash.Length);
        Buffer.BlockCopy(extraRandomBytes, 0, finalPadding, metadataHash.Length, extraRandomBytes.Length);
        finalPadding[totalPaddingSize - 5] = (byte)totalPaddingSize;
        Buffer.BlockCopy(signature, 0, finalPadding, totalPaddingSize - 4, signature.Length);
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            if (fs.Length > totalPaddingSize) {
                fs.Seek(-4, SeekOrigin.End);
                byte[] checkSig = new byte[4];
                fs.Read(checkSig, 0, 4);
                if (Encoding.ASCII.GetString(checkSig) == "HCHG") {
                    fs.Seek(-5, SeekOrigin.End);
                    int oldPaddingSize = fs.ReadByte();
                    if (oldPaddingSize > 0) fs.SetLength(fs.Length - oldPaddingSize);
                }
            }
            fs.Seek(0, SeekOrigin.End);
            fs.Write(finalPadding, 0, finalPadding.Length);
        }
        newMd5 = GetMD5(filePath);
    }
}