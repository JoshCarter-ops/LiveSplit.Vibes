using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;

namespace LiveSplit.UI.Components
{
    public partial class VibesSettings : UserControl
    {
        public bool Display2Rows { get; set; }
        public string Comparison { get; set; }
        public LiveSplitState CurrentState { get; set; }
        public LayoutMode Mode { get; set; }

        public string TextDefault { get; set; }
        public string TextPB { get; set; }
        public string TextNotPB { get; set; }
        public string TextAheadLarge { get; set; }
        public string TextAheadLow { get; set; }
        public string TextBehindLarge { get; set; }
        public string TextBehindLow { get; set; }

        public VibesSettings() {
            InitializeComponent();
            Display2Rows = false;
            Comparison = "Personal Best";

            TextDefault = "";
            TextPB = "";
            TextNotPB = "";
            TextAheadLarge = "";
            TextAheadLow = "";
            TextBehindLarge = "";
            TextBehindLow = "";

            txtDefault.DataBindings.Add("Text", this, "TextDefault");
            txtPB.DataBindings.Add("Text", this, "TextPB");
            txtNotPB.DataBindings.Add("Text", this, "TextNotPB");
            txtAheadLarge.DataBindings.Add("Text", this, "TextAheadLarge");
            txtAheadLow.DataBindings.Add("Text", this, "TextAheadLow");
            txtBehindLarge.DataBindings.Add("Text", this, "TextBehindLarge");
            txtBehindLow.DataBindings.Add("Text", this, "TextBehindLow");
        }

        private void VibesSettings_Load(object sender, EventArgs e) {
            if (Mode == LayoutMode.Horizontal){
                isTwoRows.Enabled = false;
                isTwoRows.DataBindings.Clear();
                isTwoRows.Checked = true;
            } else {
                isTwoRows.Enabled = true;
                isTwoRows.DataBindings.Clear();
                isTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent) {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "TextDefault", TextDefault) ^
            SettingsHelper.CreateSetting(document, parent, "TextPB", TextPB) ^
            SettingsHelper.CreateSetting(document, parent, "TextNotPB", TextNotPB) ^
            SettingsHelper.CreateSetting(document, parent, "TextAheadLarge", TextAheadLarge) ^
            SettingsHelper.CreateSetting(document, parent, "TextAheadLow", TextAheadLow) ^
            SettingsHelper.CreateSetting(document, parent, "TextBehindLarge", TextBehindLarge) ^
            SettingsHelper.CreateSetting(document, parent, "TextBehindLow", TextBehindLow) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        public XmlNode GetSettings(XmlDocument document) {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode() {
            return CreateSettingsNode(null, null);
        }

        public void SetSettings(XmlNode node) {
            var element = (XmlElement)node;
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
            TextDefault = SettingsHelper.ParseString(element["TextDefault"]);
            TextPB = SettingsHelper.ParseString(element["TextPB"]);
            TextNotPB = SettingsHelper.ParseString(element["TextNotPB"]);
            TextAheadLarge = SettingsHelper.ParseString(element["TextAheadLarge"]);
            TextAheadLow = SettingsHelper.ParseString(element["TextAheadLow"]);
            TextBehindLarge = SettingsHelper.ParseString(element["TextBehindLarge"]);
            TextBehindLow = SettingsHelper.ParseString(element["TextBehindLow"]);
        }
    }
}
