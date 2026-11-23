using System;
using System.IO;
using System.Text.Json;
using CfgDataStore;

namespace SqlStudio.DatabaseTransfer
{
    /// <summary>Persists the transfer pairs as JSON in the application config store.</summary>
    public static class TransferSettingsStore
    {
        private const string SettingsKey = "db_transfer_settings";

        public static string DefaultLocalFolder => Path.Combine(Path.GetTempPath(), "SqlStudioTransfer");

        public static TransferSettings Load(IConfigDataStore cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));

            TransferSettings settings = null;
            var json = cfg.GetStringValue(SettingsKey);
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<TransferSettings>(json);
                }
                catch (JsonException)
                {
                    // Corrupt settings: start over rather than crash the dialog.
                }
            }

            settings ??= new TransferSettings();
            settings.Pairs ??= new System.Collections.Generic.List<TransferPair>();
            foreach (var p in settings.Pairs)
            {
                p.SourceSsh ??= new SshEndpoint();
                p.TargetSsh ??= new SshEndpoint();
            }
            if (string.IsNullOrWhiteSpace(settings.LocalFolder))
                settings.LocalFolder = DefaultLocalFolder;
            return settings;
        }

        public static void Save(IConfigDataStore cfg, TransferSettings settings)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            cfg.SetValue(SettingsKey, JsonSerializer.Serialize(settings));
            cfg.Save();
        }
    }
}
