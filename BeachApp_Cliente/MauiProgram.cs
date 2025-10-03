using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BeachApp_Cliente.Data;
using BeachApp_Cliente.Services;
using Supabase;

namespace BeachApp_Cliente;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddMauiBlazorWebView();

		// Configurar Entity Framework
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "beachapp_cliente.db");
		builder.Services.AddDbContext<BeachAppContext>(options =>
			options.UseSqlite($"Data Source={dbPath}"));

		// Registrar serviços locais primeiro para estabilidade
		builder.Services.AddScoped<IProdutoService, ProdutoService>();
		builder.Services.AddScoped<IPedidoService, PedidoService>();
		builder.Services.AddScoped<IClientAuthService, ClientAuthService>();

		// TODO: Adicionar Supabase depois quando o app estiver estável

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
