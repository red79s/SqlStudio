using System;
using System.Windows.Forms;
using CfgDataStore;
using LlmService;

namespace SqlStudio
{
    public partial class LlmSettingsDialog : Form
    {
        private readonly IConfigDataStore _cfg;
        private LlmProvider _provider;
        private bool _loading;

        public LlmSettingsDialog(IConfigDataStore cfgDataStore)
        {
            InitializeComponent();
            _cfg = cfgDataStore;

            _loading = true;
            comboBoxProvider.Items.Add(new ProviderItem(LlmProvider.OpenAiCompatible, "OpenAI-compatible (Gemini, OpenAI, ...)"));
            comboBoxProvider.Items.Add(new ProviderItem(LlmProvider.Ollama, "Ollama (local server)"));

            var options = LlmSettingsStore.Load(_cfg);
            _provider = options.Provider;
            SelectProvider(_provider);
            PopulateModelSuggestions(_provider);
            textBoxApiKey.Text = options.ApiKey;
            comboBoxModel.Text = options.Model;
            textBoxEndpoint.Text = options.Endpoint;
            _loading = false;

            UpdateProviderDependentState();
        }

        private LlmProvider SelectedProvider
        {
            get
            {
                var item = comboBoxProvider.SelectedItem as ProviderItem;
                return item == null ? LlmProvider.OpenAiCompatible : item.Provider;
            }
        }

        private void SelectProvider(LlmProvider provider)
        {
            for (int i = 0; i < comboBoxProvider.Items.Count; i++)
            {
                if (((ProviderItem)comboBoxProvider.Items[i]).Provider == provider)
                {
                    comboBoxProvider.SelectedIndex = i;
                    return;
                }
            }

            comboBoxProvider.SelectedIndex = 0;
        }

        private void comboBoxProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;

            var selected = SelectedProvider;
            if (selected == _provider)
                return;

            // Only replace model/endpoint that the user hasn't customized, so switching providers
            // doesn't silently discard hand-typed values.
            bool keepModel = !IsProviderSuggestedOrEmpty(comboBoxModel.Text, _provider);
            string model = comboBoxModel.Text;

            PopulateModelSuggestions(selected);
            comboBoxModel.Text = keepModel ? model : LlmProviderDefaults.Model(selected);

            if (IsProviderDefaultOrEmpty(textBoxEndpoint.Text, LlmProviderDefaults.Endpoint(_provider)))
                textBoxEndpoint.Text = LlmProviderDefaults.Endpoint(selected);

            _provider = selected;
            UpdateProviderDependentState();
        }

        /// <summary>
        /// Fills the model list with the provider's known models. The combo stays editable, so a
        /// model that isn't listed can still be typed in.
        /// </summary>
        private void PopulateModelSuggestions(LlmProvider provider)
        {
            comboBoxModel.Items.Clear();
            foreach (var model in LlmProviderDefaults.SuggestedModels(provider))
            {
                comboBoxModel.Items.Add(model);
            }
        }

        /// <summary>
        /// True when the model box holds nothing the user picked deliberately — empty, or one of
        /// <paramref name="provider"/>'s own suggestions, which are meaningless to another provider.
        /// </summary>
        private static bool IsProviderSuggestedOrEmpty(string value, LlmProvider provider)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            var trimmed = value.Trim();
            foreach (var model in LlmProviderDefaults.SuggestedModels(provider))
            {
                if (string.Equals(trimmed, model, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static bool IsProviderDefaultOrEmpty(string value, string providerDefault)
        {
            return string.IsNullOrWhiteSpace(value)
                || string.Equals(value.Trim(), providerDefault, StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateProviderDependentState()
        {
            bool needsApiKey = LlmProviderDefaults.RequiresApiKey(_provider);

            labelApiKey.Enabled = needsApiKey;
            textBoxApiKey.Enabled = needsApiKey;
            checkBoxShowKey.Enabled = needsApiKey;

            labelInfo.Text = needsApiKey
                ? "Changes apply to new connections / after restart."
                : "Ollama needs no API key. Changes apply to new connections / after restart.";
        }

        private void checkBoxShowKey_CheckedChanged(object sender, EventArgs e)
        {
            textBoxApiKey.UseSystemPasswordChar = !checkBoxShowKey.Checked;
        }

        /// <summary>
        /// Builds the options described by the dialog as it currently stands, so testing and saving
        /// always operate on exactly the same configuration.
        /// </summary>
        private LlmOptions CurrentOptions()
        {
            return new LlmOptions
            {
                Provider = _provider,
                ApiKey = LlmProviderDefaults.RequiresApiKey(_provider) ? textBoxApiKey.Text.Trim() : string.Empty,
                Model = comboBoxModel.Text.Trim(),
                Endpoint = textBoxEndpoint.Text.Trim()
            };
        }

        private async void buttonTest_Click(object sender, EventArgs e)
        {
            var options = CurrentOptions();

            if (LlmProviderDefaults.RequiresApiKey(options.Provider) && string.IsNullOrWhiteSpace(options.ApiKey))
            {
                MessageBox.Show(
                    this,
                    "Enter an API key before testing the connection.",
                    "LLM Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                textBoxApiKey.Focus();
                return;
            }

            // The probe is a real network round-trip; keep the dialog responsive and make it
            // obvious that something is happening, and that a second click won't help.
            var previousInfo = labelInfo.Text;
            buttonTest.Enabled = false;
            buttonOK.Enabled = false;
            labelInfo.Text = "Testing connection ...";
            UseWaitCursor = true;

            try
            {
                var result = await LlmConnectionTester.TestAsync(options);

                MessageBox.Show(
                    this,
                    result.Message,
                    result.Success ? "Connection OK" : "Connection failed",
                    MessageBoxButtons.OK,
                    result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                UseWaitCursor = false;
                labelInfo.Text = previousInfo;
                buttonOK.Enabled = true;
                buttonTest.Enabled = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var options = CurrentOptions();

            LlmSettingsStore.Store(_cfg, options);
            _cfg.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>Combo-box item that shows a friendly name for an <see cref="LlmProvider"/>.</summary>
        private sealed class ProviderItem
        {
            public ProviderItem(LlmProvider provider, string displayName)
            {
                Provider = provider;
                DisplayName = displayName;
            }

            public LlmProvider Provider { get; }

            public string DisplayName { get; }

            public override string ToString() => DisplayName;
        }
    }
}
