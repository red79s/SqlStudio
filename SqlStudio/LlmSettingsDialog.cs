using System;
using System.Windows.Forms;
using CfgDataStore;
using LlmService;

namespace SqlStudio
{
    public partial class LlmSettingsDialog : Form
    {
        private readonly IConfigDataStore _cfg;

        public LlmSettingsDialog(IConfigDataStore cfgDataStore)
        {
            InitializeComponent();
            _cfg = cfgDataStore;

            var defaults = new LlmOptions();
            textBoxApiKey.Text = _cfg.GetStringValue("gemini_api_key") ?? string.Empty;
            var model = _cfg.GetStringValue("llm_model");
            var endpoint = _cfg.GetStringValue("llm_endpoint");
            textBoxModel.Text = string.IsNullOrWhiteSpace(model) ? defaults.Model : model;
            textBoxEndpoint.Text = string.IsNullOrWhiteSpace(endpoint) ? defaults.Endpoint : endpoint;
        }

        private void checkBoxShowKey_CheckedChanged(object sender, EventArgs e)
        {
            textBoxApiKey.UseSystemPasswordChar = !checkBoxShowKey.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _cfg.SetValue("gemini_api_key", textBoxApiKey.Text.Trim());
            _cfg.SetValue("llm_model", textBoxModel.Text.Trim());
            _cfg.SetValue("llm_endpoint", textBoxEndpoint.Text.Trim());
            _cfg.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
