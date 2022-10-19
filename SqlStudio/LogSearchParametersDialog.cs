using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class LogSearchParametersDialog : Form
    {
        public string QueryString { get; set; }
        public LogSearchParametersDialog()
        {
            InitializeComponent();
            dateTimePickerStart.Value = DateTime.Now.AddMinutes(-30);
            dateTimePickerEnd.Value = DateTime.Now;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            string queryString =
                "SELECT  Application, TenantId, ClientId, LogArea, LogLevel, LogDate, LogDateLocal, ClientLogId, ThreadId, SessionId, Message, Data FROM ApplicationLog WHERE ";
            
            queryString += $" LogDateLocal > '{dateTimePickerStart.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
            queryString += $" AND LogDateLocal < '{dateTimePickerEnd.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (!string.IsNullOrEmpty(tbTenantId.Text))
            {
                queryString += $" AND TenantId = '{tbTenantId.Text}' ";
            }

            if (!string.IsNullOrEmpty(tbClientId.Text))
            {
                queryString += $" AND ClientId like '%{tbClientId.Text}%' ";
            }

            string application = "";
            if (checkBoxBagGateService.Checked)
                application += "'BCS.BPU.Service.exe'";
            if (checkBoxWpfClient.Checked)
            {
                if (application.Length > 0)
                    application += ", ";
                application += "'BCS.SBD.WPF.APP.exe'";
            }

            if (checkBoxCameraATR.Checked)
            {
                if (application.Length > 0)
                    application += ", ";
                application += "'Atec.Atr.CameraService.exe'";
            }

            if (application.Length > 0)
            {
                queryString += $" AND Application in ({application})";
            }

            if (!string.IsNullOrEmpty(tbMessageSearch.Text))
            {
                queryString += $" AND Message like '%{tbMessageSearch.Text}%'";
            }   
            
            queryString += " order by applicationlogid;";

            QueryString = queryString;
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd.Value = dateTimePickerStart.Value.AddMinutes(10);
        }
    }
}
