namespace ParkingManagement.Infrastructure
{
    public interface IEncryptionUtility
    {
        string ComputeHashWithSalt(string text);

        bool IsHashEqual(string hashWithSalt, string text);
    }
}
