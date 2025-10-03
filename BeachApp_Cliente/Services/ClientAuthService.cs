using Microsoft.Extensions.Logging;

namespace BeachApp_Cliente.Services
{
    public class ClientAuthService : IClientAuthService
    {
        private readonly ILogger<ClientAuthService> _logger;
        private ClienteInfo? _currentCliente;

        public event Action<ClienteInfo?>? AuthStateChanged;

        public ClientAuthService(ILogger<ClientAuthService> logger)
        {
            _logger = logger;
        }

        public async Task<ClienteInfo?> LoginAsync(string nomeOuCpf, string? telefone = null)
        {
            try
            {
                _logger.LogInformation($"Tentativa de login do cliente: {nomeOuCpf}");

                // Validações básicas
                if (string.IsNullOrWhiteSpace(nomeOuCpf))
                {
                    throw new ArgumentException("Nome ou CPF é obrigatório");
                }

                // Para esta versão simples, vamos apenas criar o objeto do cliente
                // Em uma versão mais complexa, aqui você poderia validar contra um banco de dados
                var cliente = new ClienteInfo
                {
                    Nome = nomeOuCpf.Length == 11 && nomeOuCpf.All(char.IsDigit) 
                        ? $"Cliente {nomeOuCpf.Substring(0, 3)}.***.**-**" // Se for CPF, mascarar
                        : nomeOuCpf, // Se for nome, usar diretamente
                    Cpf = nomeOuCpf.Length == 11 && nomeOuCpf.All(char.IsDigit) ? nomeOuCpf : null,
                    Telefone = telefone,
                    LoginTime = DateTime.Now
                };

                _currentCliente = cliente;
                AuthStateChanged?.Invoke(_currentCliente);

                _logger.LogInformation($"Login bem-sucedido para cliente: {cliente.Nome}");
                return cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro durante o login do cliente: {nomeOuCpf}");
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation($"Logout do cliente: {_currentCliente?.Nome}");
                
                _currentCliente = null;
                AuthStateChanged?.Invoke(null);
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o logout");
                throw;
            }
        }

        public async Task<ClienteInfo?> GetCurrentClienteAsync()
        {
            await Task.CompletedTask;
            return _currentCliente;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            await Task.CompletedTask;
            return _currentCliente != null;
        }
    }
}
