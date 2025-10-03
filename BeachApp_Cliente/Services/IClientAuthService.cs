using BeachApp_Cliente.Models;

namespace BeachApp_Cliente.Services
{
    public interface IClientAuthService
    {
        Task<ClienteInfo?> LoginAsync(string nomeOuCpf, string? telefone = null);
        Task LogoutAsync();
        Task<ClienteInfo?> GetCurrentClienteAsync();
        Task<bool> IsLoggedInAsync();
        event Action<ClienteInfo?> AuthStateChanged;
    }

    public class ClienteInfo
    {
        public string Nome { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Mesa { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
