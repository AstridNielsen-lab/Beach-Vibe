using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Data;
using AdmBeachApp.Services;

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
		
		// Configurar Entity Framework
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "beachapp.db");
		builder.Services.AddDbContext<BeachAppContext>(options =>
			options.UseSqlite($"Data Source={dbPath}"));
		
        // Registrar serviços
        builder.Services.AddScoped<IProdutoService, ProdutoService>();
        builder.Services.AddScoped<IVendaService, VendaService>();
        builder.Services.AddScoped<IPedidoService, PedidoService>();
		
		// Configurar autenticação Google OAuth (será implementado posteriormente)
		// builder.Services.AddAuthentication().AddGoogle(...);

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
