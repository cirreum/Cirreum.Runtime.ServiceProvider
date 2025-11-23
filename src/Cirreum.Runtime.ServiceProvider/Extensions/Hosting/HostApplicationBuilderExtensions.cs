namespace Microsoft.Extensions.Hosting;

using Cirreum.Health;
using Cirreum.Logging.Deferred;
using Cirreum.Providers;
using Cirreum.ServiceProvider;
using Cirreum.ServiceProvider.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class HostApplicationBuilderExtensions {

	/// <summary>
	/// Register Service Provider.
	/// </summary>
	/// <typeparam name="TRegistrar"></typeparam>
	/// <typeparam name="TSettings"></typeparam>
	/// <typeparam name="TInstanceSettings"></typeparam>
	/// <typeparam name="THealthOptions"></typeparam>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static IHostApplicationBuilder RegisterServiceProvider<TRegistrar, TSettings, TInstanceSettings, THealthOptions>(
		this IHostApplicationBuilder builder)
		where TRegistrar : ServiceProviderRegistrar<TSettings, TInstanceSettings, THealthOptions>, new()
		where TSettings : ServiceProviderSettings<TInstanceSettings, THealthOptions>
		where TInstanceSettings : ServiceProviderInstanceSettings<THealthOptions>
		where THealthOptions : ServiceProviderHealthCheckOptions, new() {

		var registrarName = typeof(TRegistrar).Name;
		var deferredLogger = Logger.CreateDeferredLogger();

		using (var loggingScope = deferredLogger.BeginScope($"Registrar {registrarName}")) {

			// Check if this specific registrar type is already registered
			if (builder.Services.IsMarkerTypeRegistered<TRegistrar>()) {
				deferredLogger.LogDebug($"Duplicate request for '{registrarName}' and will be skipped.");
				return builder;
			}
			// Mark this registrar type as registered
			builder.Services.MarkTypeAsRegistered<TRegistrar>();

			var registrar = new TRegistrar();

			var providerSectionKey = GetProviderConfigPath(registrar.ProviderType, registrar.ProviderName);
			var providerSection = builder.Configuration.GetSection(providerSectionKey);
			if (!providerSection.Exists()) {
				deferredLogger.LogWarning($"No configuration settings found for '{registrarName}'.");
				return builder;
			}

			var providerSettings = providerSection.Get<TSettings>();
			if (providerSettings is null) {
				// Log the raw configuration that failed to bind
				var configDump = providerSection.GetChildren()
					.Select(c => $"{c.Key}={c.Value ?? "[section]"}")
					.ToList();

				deferredLogger.LogError(
					$"Invalid configuration for '{registrarName}' - section exists but cannot be bound to settings. " +
					$"Found keys: {string.Join(", ", configDump)}");

				throw new InvalidOperationException(
					$"Invalid configuration for '{registrarName}' - section exists but cannot be bound to settings.");
			}

			if (providerSettings.Instances.Count == 0) {
				deferredLogger.LogWarning($"0 instances found to register for '{registrarName}'.");
				return builder;
			}

			// Register the service Provider
			registrar.Register(
				providerSettings,
				builder.Services,
				builder.Configuration);

			deferredLogger.LogDebug(
				$"Registered {providerSettings.Instances.Count} provider instances for '{registrarName}' of type '{registrar.ProviderType}'.");

		}

		return builder;

	}

	// Helper method for building provider configuration paths
	private static string GetProviderConfigPath(ProviderType providerType, string providerName) =>
		$"Cirreum:{providerType}:Providers:{providerName}";

}