
namespace Ekip.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        Task<bool> Verify(string hash , string password);
    }
}
