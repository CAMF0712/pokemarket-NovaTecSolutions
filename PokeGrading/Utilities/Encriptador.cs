
using System.Security.Cryptography;
using System.Text;
public class Encriptador
{
    public static string ObtenerHashMD5(string input)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convertir el arreglo de bytes a string hexadecimal
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
