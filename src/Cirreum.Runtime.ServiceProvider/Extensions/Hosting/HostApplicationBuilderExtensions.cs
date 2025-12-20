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
	/// <typeparam name="TRegistrar">The registrar type.</typeparam>
	/// <typeparam name="TSettings">The settings type.</typeparam>
	/// <typeparam name="TInstanceSettings">The instance settings type.</typeparam>
	/// <typeparam name="THealthOptions">The health options type.</typeparam>
	/// <param name="builder">The host application builder.</param>
	/// <param name="required">If true, throws an exception when configuration is missing. Default is false.</param>
	/// <returns>The host application builder for chaining.</returns>
	/// <exception cref="InvalidOperationException">Thrown when required is true and configuration is missing or invalid.</exception>
	public static IHostApplicationBuilder RegisterServiceProvider<TRegistrar, TSettings, TInstanceSettings, THealthOptions>(
		this IHostApplicationBuilder builder,
		bool required = false)
		where TRegistrar : ServiceProviderRegistrar<TSettings, TInstanceSettings, THealthOptions>, new()
		where TSettings : ServiceProviderSettings<TInstanceSettings, THealthOptions>
		where TInstanceSettings : ServiceProviderInstanceSettings<THealthOptions>
		where THealthOptions : ServiceProviderHealthCheckOptions, new() {

		var registrarName = typeof(TRegistrar).Name;
		var deferredLogger = Logger.CreateDeferredLogger();

		using (var loggingScope = deferredLogger.BeginScope(new { RegistrarName = registrarName })) {

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
				if (required) {
					throw new InvalidOperationException(
						$"Configuration required but not found for '{registrarName}' at '{providerSectionKey}'.");
				}

				deferredLogger.LogDebug(
					"Skipping '{registrarName}' - no configuration found at '{configPath}'.",
					registrarName,
					providerSectionKey);
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