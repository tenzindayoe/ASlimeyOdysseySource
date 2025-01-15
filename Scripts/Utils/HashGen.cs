using System;
using System.Security.Cryptography;
using System.Text;

public class HashGen{
    public static string GenerateTimeAndRandBasedHash(){
        // use sha256 to hash the string
        string hash = sha256(currentDateTime() + generateRandomString());
        return hash;
    }

    public static string GenerateKeyBasedHash(string key){
        // use sha256 to hash the string
        string hash = sha256(key);
        return hash;
    }
    private static string currentDateTime(){
        return DateTime.Now.ToString("yyyyMMddHHmmss");
    }
    private static string generateRandomString(){
        return Guid.NewGuid().ToString();
    }
    private static string sha256(string input){
        using (SHA256 sha256Hash = SHA256.Create()){
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++){
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}