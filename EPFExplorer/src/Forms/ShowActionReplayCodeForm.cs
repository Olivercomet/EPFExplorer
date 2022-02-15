using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class ShowActionReplayCodeForm : Form
    {
        public ShowActionReplayCodeForm()
        {
            InitializeComponent();
        }

        public void SetInfo(string ARcode, string titleText, string forText, string sideEffectsText)
        {
            richTextBox1.Text = ARcode;
            TitleLabel.Text = titleText;
            ForLabel.Text = forText;
            sideEffectsLabel.Text = sideEffectsText;
        }
    }
}
