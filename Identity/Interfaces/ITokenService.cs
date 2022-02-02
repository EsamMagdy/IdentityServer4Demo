using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityServerHost.Quickstart.UI
{
  public interface ITokenService
  {
    Task<string> GetToken(string scope);
  }
}