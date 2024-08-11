using System.Security.Cryptography;
using System.Text;

namespace BusinessObject.Ultils;

public static class HashPass
{
    public static string HashWithSHA256(string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        // Normalize the input string to avoid issues with extra spaces or different encodings.
        input = input.Trim();

        using SHA256 sHA256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] bytes = sHA256.ComputeHash(inputBytes);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
}