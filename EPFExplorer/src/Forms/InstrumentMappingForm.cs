using System.Windows.Forms;

namespace EPFExplorer
{
    public partial class InstrumentMappingForm : Form
    {
        public InstrumentMappingForm()
        {
            InitializeComponent();
        }

        public void InitializeLists(xmfile xm)
        {

            sourceInstrumentsBox.Items.Clear();

            for (int i = 0; i < xm.samples.Count; i++)
            {
                sourceInstrumentsBox.Items.Add(xm.samples[i].name);
            }

            IngameInstrumentsBox.Items.Clear();

            for (int i = 0; i < xm.parentbinfile.samplecount; i++)
            {
                IngameInstrumentsBox.Items.Add("Instrument_" + i);
            }
        }
    }
}