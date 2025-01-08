using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
			SetupSerilog();
			builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

			builder.Logging.AddSerilog(dispose: true);
			return builder.Build();
		}
		private static void SetupSerilog()
		{
			var flushInterval = new TimeSpan(0, 0, 1);
			var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyApp.log");

			Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
			.CreateLogger();
		}
		public static Microsoft.Extensions.Logging.ILogger CreateLogger<T>()
		{ 
			var factory = new LoggerFactory();
			return factory.CreateLogger<T>();
		}
	}
}