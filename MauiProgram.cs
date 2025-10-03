using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Data;
using AdmBeachApp.Services;
using Supabase;

namespace AdmBeachApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		
		// Configurar Entity Framework (mantido para fallback)
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "beachapp.db");
		builder.Services.AddDbContext<BeachAppContext>(options =>
			options.UseSqlite($"Data Source={dbPath}"));
		
		// Configurar Supabase
		builder.Services.AddSingleton(provider =>
		{
			var options = AdmBeachApp.Data.SupabaseConfig.GetOptions();
			return new Supabase.Client(AdmBeachApp.Data.SupabaseConfig.Url, AdmBeachApp.Data.SupabaseConfig.AnonKey, options);
		});
		
        // Registrar serviços Supabase como principais
        builder.Services.AddScoped<IProdutoService, SupabaseProdutoService>();
        
        // Registrar PedidoService com notificações
        builder.Services.AddScoped<SupabasePedidoService>(); // Serviço base
        builder.Services.AddScoped<IPedidoService>(provider =>
        {
            var baseService = provider.GetRequiredService<SupabasePedidoService>();
            var notificacaoService = provider.GetRequiredService<IEventNotificacaoService>();
            var logger = provider.GetRequiredService<ILogger<PedidoServiceComNotificacao>>();
            return new PedidoServiceComNotificacao(baseService, notificacaoService, logger);
        });
        
        builder.Services.AddScoped<IVendaService, VendaService>();
        
        // Registrar serviço de autenticação
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        // Registrar serviço de autenticação Google para clientes
        builder.Services.AddScoped<IGoogleClientAuthService, GoogleClientAuthService>();
        
        // Registrar serviço de notificações baseado em eventos
        builder.Services.AddSingleton<IEventNotificacaoService, EventNotificacaoService>();
        
        // Registrar serviços locais como fallback (caso necessário)
        builder.Services.AddScoped<ProdutoService>();
        builder.Services.AddScoped<PedidoService>();
		
		// Configurar autenticação (OAuth será gerenciado pelo Supabase)
		// A configuração OAuth do Google é feita no painel do Supabase

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		
		// Garantir que o banco de dados seja criado
		using (var scope = app.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<BeachAppContext>();
			context.Database.EnsureCreated();
		}
		
		return app;
	}
}
