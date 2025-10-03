using System.Text.Json;
using Microsoft.Extensions.Logging;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace AdmBeachApp.Services
{
    public interface IAuthService
    {
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsLoggedInAsync();
        Task<User?> LoginWithGoogleAsync();
        Task LogoutAsync();
        Task<User?> GetUserFromStorageAsync();
        event Action<User?> AuthStateChanged;
    }

    public class AuthService : IAuthService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<AuthService> _logger;
        private User? _currentUser;

        public event Action<User?>? AuthStateChanged;

        public AuthService(Supabase.Client supabaseClient, ILogger<AuthService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
            
            // Inscrever-se nas mudanças de autenticação do Supabase
            _supabaseClient.Auth.AddStateChangedListener(OnAuthStateChanged);
        }

        public Task<User?> GetCurrentUserAsync()
        {
            try
            {
                if (_currentUser == null)
                {
                    _currentUser = _supabaseClient.Auth.CurrentUser;
                }
                return Task.FromResult(_currentUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário atual");
                return Task.FromResult<User?>(null);
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var user = await GetCurrentUserAsync();
            return user != null;
        }

        public async Task<User?> LoginWithGoogleAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando login com Google");

                // Para MAUI, usaremos o fluxo PKCE (Proof Key for Code Exchange)
                var options = new SignInOptions
                {
                    FlowType = Constants.OAuthFlowType.PKCE,
                    RedirectTo = "https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback"
                };

                // Iniciar o fluxo OAuth com Google
                var authResponse = await _supabaseClient.Auth.SignIn(Constants.Provider.Google, options);
                
                if (authResponse != null)
                {
                    // Aguardar um pouco para o Supabase processar
                    await Task.Delay(1000);
                    _currentUser = _supabaseClient.Auth.CurrentUser;
                    
                    if (_currentUser != null)
                    {
                        _logger.LogInformation($"Login bem-sucedido para usuário: {_currentUser.Email}");
                        
                        // Salvar informações do usuário localmente
                        await SaveUserToStorageAsync(_currentUser);
                        
                        AuthStateChanged?.Invoke(_currentUser);
                        return _currentUser;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante login com Google");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Realizando logout");
                
                await _supabaseClient.Auth.SignOut();
                _currentUser = null;
                
                // Limpar dados locais
                await ClearUserFromStorageAsync();
                
                AuthStateChanged?.Invoke(null);
                
                _logger.LogInformation("Logout realizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante logout");
            }
        }

        public Task<User?> GetUserFromStorageAsync()
        {
            try
            {
                // Tentar recuperar usuário atual
                var currentUser = _supabaseClient.Auth.CurrentUser;
                if (currentUser != null)
                {
                    _currentUser = currentUser;
                    return Task.FromResult(_currentUser);
                }

                return Task.FromResult<User?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar usuário do storage");
                return Task.FromResult<User?>(null);
            }
        }

        private async Task SaveUserToStorageAsync(User user)
        {
            try
            {
                var userJson = JsonSerializer.Serialize(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.UserMetadata?.ContainsKey("full_name") == true ? 
                           user.UserMetadata["full_name"]?.ToString() : "",
                    Avatar = user.UserMetadata?.ContainsKey("avatar_url") == true ? 
                            user.UserMetadata["avatar_url"]?.ToString() : "",
                    Provider = "google"
                });

                await SecureStorage.SetAsync("current_user", userJson);
                _logger.LogDebug("Dados do usuário salvos localmente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar dados do usuário");
            }
        }

        private Task ClearUserFromStorageAsync()
        {
            try
            {
                SecureStorage.Remove("current_user");
                _logger.LogDebug("Dados do usuário removidos do storage");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar dados do usuário");
                return Task.CompletedTask;
            }
        }

        private void OnAuthStateChanged(IGotrueClient<User, Session> sender, Constants.AuthState state)
        {
            _logger.LogDebug($"Estado de autenticação alterado: {state}");
            
            switch (state)
            {
                case Constants.AuthState.SignedIn:
                    _currentUser = sender.CurrentUser;
                    AuthStateChanged?.Invoke(_currentUser);
                    break;
                    
                case Constants.AuthState.SignedOut:
                    _currentUser = null;
                    AuthStateChanged?.Invoke(null);
                    break;
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Tentar recuperar sessão existente
                await GetUserFromStorageAsync();
                _logger.LogInformation("Serviço de autenticação inicializado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inicializar serviço de autenticação");
            }
        }
    }

    // Modelo para armazenar dados do usuário
    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}
