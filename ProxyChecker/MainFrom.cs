using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyChecker
{
    public partial class MainFrom : Form
    {
        private readonly ProxyDataModel _proxyDataModel = new ProxyDataModel();
        public static int MaxParallelism { set; get; } = 32;

        private TestProgressForm _testProgressForm;

        public MainFrom()
        {
            InitializeComponent();

            ProxyDataGridView.DataSource = _proxyDataModel.Table;
        }

        private void Log(string message)
        {
            textBoxLog.Text = message + @"
" + textBoxLog.Text;
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            var form = new AddProxyForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                _proxyDataModel.AddProxyList(form.GetData());
            }
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            var proxyList = _proxyDataModel.ProxyList;
            _testProgressForm = new TestProgressForm();
            _testProgressForm.Show();
            TestProxyList();
        }

        private void TestProxyList()
        {
            var proxyNum = _proxyDataModel.ProxyList.Count;
            var proxyTested = 0;

            Enabled = false;
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(_proxyDataModel.ProxyList, new ParallelOptions {MaxDegreeOfParallelism = MaxDegreeOfParallelism}, proxy =>
                {
                    proxy.PerformTest();
                    ++proxyTested;
                    BeginInvoke(
                        (MethodInvoker)
                            delegate { UpdateTestProgress(Convert.ToInt16(proxyTested*100.0/proxyNum)); });
                });

                BeginInvoke((MethodInvoker) delegate
                {
                    _testProgressForm.Close();
                    _testProgressForm = null;
                    Enabled = true;
                });
            });
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            _proxyDataModel.RemoveAll();
        }

        private void UpdateTestProgress(int progress)
        {
            Log(progress.ToString());
            _testProgressForm.SetProgress(progress);
            _proxyDataModel.UpdateTable();
        }

        private void butSettings_Click(object sender, EventArgs e) {
            var form = new SettingsFrom();
            form.ShowDialog();
            

        }
        }

    }

