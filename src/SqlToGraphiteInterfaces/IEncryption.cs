namespace SqlToGraphiteInterfaces
{
    public interface IEncryption
    {
        string Encrypt(string clearText);

        string Decrypt(string cipherText);
    }
}