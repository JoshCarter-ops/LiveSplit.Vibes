using LiveSplit.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class VibesSettings : UserControl
    {
        public bool Display2Rows { get; set; }
        public string Comparison { get; set; }
        public LiveSplitState CurrentState { get; set; }
        public LayoutMode Mode { get; set; }

        public string TextDefault { get; set; }
        public string LeftText { get; set; }
        public string TextPB { get; set; }
        public string TextNotPB { get; set; }
        public string TextAheadLarge { get; set; }
        public string TextAheadLow { get; set; }
        public string TextBehindLarge { get; set; }
        public string TextBehindLow { get; set; }

        public decimal NumAheadHigh { get; set; }
        public decimal NumAheadLow { get; set; }
        public decimal NumBehindHigh { get; set; }
        public decimal NumBehindLow { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public VibesSettings() {
            InitializeComponent();
            Display2Rows = false;
            Comparison = "Personal Best";

            TextDefault = txtDefault.Text;
            LeftText = leftText.Text;
            TextPB = txtPB.Text;
            TextNotPB = txtNotPB.Text;
            TextAheadLarge = txtAheadLarge.Text;
            TextAheadLow = txtAheadLow.Text;
            TextBehindLarge = txtBehindLarge.Text;
            TextBehindLow = txtBehindLow.Text;

            NumAheadHigh = numAheadHigh.Value;
            NumAheadLow = numAheadLow.Value;
            NumBehindHigh = numBehindHigh.Value;
            NumBehindLow = numBehindLow.Value;

            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;

            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);

            txtDefault.DataBindings.Add("Text", this, "TextDefault");
            leftText.DataBindings.Add("Text", this, "LeftText");
            txtPB.DataBindings.Add("Text", this, "TextPB");
            txtNotPB.DataBindings.Add("Text", this, "TextNotPB");
            txtAheadLarge.DataBindings.Add("Text", this, "TextAheadLarge");
            txtAheadLow.DataBindings.Add("Text", this, "TextAheadLow");
            txtBehindLarge.DataBindings.Add("Text", this, "TextBehindLarge");
            txtBehindLow.DataBindings.Add("Text", this, "TextBehindLow");

            numAheadHigh.DataBindings.Add("Text", this, "NumAheadHigh");
            numAheadLow.DataBindings.Add("Text", this, "NumAheadLow");
            numBehindHigh.DataBindings.Add("Text", this, "NumBehindHigh");
            numBehindLow.DataBindings.Add("Text", this, "NumBehindLow");
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
            SettingsHelper.CreateSetting(document, parent, "LeftText", LeftText) ^
            SettingsHelper.CreateSetting(document, parent, "TextPB", TextPB) ^
            SettingsHelper.CreateSetting(document, parent, "TextNotPB", TextNotPB) ^
            SettingsHelper.CreateSetting(document, parent, "TextAheadLarge", TextAheadLarge) ^
            SettingsHelper.CreateSetting(document, parent, "TextAheadLow", TextAheadLow) ^
            SettingsHelper.CreateSetting(document, parent, "TextBehindLarge", TextBehindLarge) ^
            SettingsHelper.CreateSetting(document, parent, "TextBehindLow", TextBehindLow) ^
            
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
           
            SettingsHelper.CreateSetting(document, parent, "NumAheadHigh", NumAheadHigh) ^
            SettingsHelper.CreateSetting(document, parent, "NumAheadLow", NumAheadLow) ^
            SettingsHelper.CreateSetting(document, parent, "NumBehindHigh", NumBehindHigh) ^
            SettingsHelper.CreateSetting(document, parent, "NumBehindLow", NumBehindLow) ^
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

            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);

            TextDefault = SettingsHelper.ParseString(element["TextDefault"]);
            LeftText = SettingsHelper.ParseString(element["LeftText"]);
            TextPB = SettingsHelper.ParseString(element["TextPB"]);
            TextNotPB = SettingsHelper.ParseString(element["TextNotPB"]);
            TextAheadLarge = SettingsHelper.ParseString(element["TextAheadLarge"]);
            TextAheadLow = SettingsHelper.ParseString(element["TextAheadLow"]);
            TextBehindLarge = SettingsHelper.ParseString(element["TextBehindLarge"]);
            TextBehindLow = SettingsHelper.ParseString(element["TextBehindLow"]);

            NumAheadHigh = SettingsHelper.ParseInt(element["NumAheadHigh"]);
            NumAheadLow = SettingsHelper.ParseInt(element["NumAheadLow"]);
            NumBehindHigh = SettingsHelper.ParseInt(element["NumBehindHigh"]);
            NumBehindLow = SettingsHelper.ParseInt(element["NumBehindLow"]);
        }

        private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }
    }
}
