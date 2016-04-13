using System.Windows.Forms;

namespace ProxyChecker
{
    public partial class TestProgressForm : Form
    {
        public TestProgressForm()
        {
            InitializeComponent();
        }

        public void SetProgress(int progress)
        {
            progressBar.Value = progress;
            progressLabel.Text = progress + @"%";
        }
    }
}