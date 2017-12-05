using GaSchedule.Algorithm;
using GaSchedule.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace ClassScheduler
{
    public partial class Test : Form
    {
        private string configJsonFile;
        private Configuration configuration;

        private string resultPerStudentsGroup;
        private string resultPerTeacher;

        private GeneticAlgorithm ga;

        public Test()
        {
            InitializeComponent();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.panelInput.Dock = DockStyle.Fill;
            this.panelOutput.Dock = DockStyle.Fill;
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.SwitchView(false);
            this.configuration = new Configuration();
            this.BindInputControls();
        }

        private void SwitchView(bool showResult)
        {
            this.panelInput.Visible = !showResult;
            this.InputViewMenuItem.Checked = !showResult;
            this.panelOutput.Visible = showResult;
            this.OutputMenuItem.Checked = showResult;
            if (!showResult)
            {
                this.SetStatusBar();
            }
        }

        private bool OpenConfigFile(string path)
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    this.configuration = (Configuration)serializer.Deserialize(file, typeof(Configuration));
                }

                this.SwitchView(false);
            }
            catch (Exception)
            {
                MessageBox.Show($"解析'{path}'文件时发生错误。");
                return false;
            }

            return true;
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openJsonFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (this.OpenConfigFile(this.openJsonFileDialog.FileName))
                {
                    this.configJsonFile = this.openJsonFileDialog.FileName;
                    this.ResetWindowTitle();

                    this.SwitchView(false);
                    this.BindInputControls();
                }
            }
        }

        private void ResetWindowTitle()
        {
            this.Text = $"排课-{this.configJsonFile}";
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.BindInputControls();
        }

        private void BindInputControls()
        {
            if (this.configuration == null)
            {
                return;
            }

            switch (this.comboBox1.Text)
            {
                case "课程安排":
                    this.dataGridView1.DataSource = new BindingList<CourseClass>(this.configuration.CourseClasses);
                    break;

                case "老师":
                    this.dataGridView1.DataSource = new BindingList<Teacher>(this.configuration.Teachers);
                    break;

                case "班级":
                    this.dataGridView1.DataSource = new BindingList<StudentsGroup>(this.configuration.StudentsGroups);
                    break;

                case "学科":
                    this.dataGridView1.DataSource = new BindingList<Course>(this.configuration.Courses);
                    break;

                default:
                    break;
            }

            this.propertyGrid1.SelectedObject = this.configuration.Parameters;
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.configJsonFile))
            {
                this.SaveAs();
            }
            else
            {
                this.WriteConfigurationToJsonFile(this.configJsonFile);
            }
        }

        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveAs();
        }

        private void SaveAs()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.WriteConfigurationToJsonFile(this.saveFileDialog1.FileName))
                {
                    this.configJsonFile = this.saveFileDialog1.FileName;
                    this.ResetWindowTitle();
                }
            }
        }

        private bool WriteConfigurationToJsonFile(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, this.configuration);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            this.configuration = new Configuration();
            this.BindInputControls();
        }

        private void RunMenuItem_Click(object sender, EventArgs e)
        {
            if (this.configuration != null && !backgroundWorker1.IsBusy)
            {
                // Reset status bar
                this.toolStripStatusLabel2.Visible = true;
                this.toolStripProgressBar1.Visible = true;
                this.toolStripProgressBar1.Minimum = 0;
                this.toolStripProgressBar1.Maximum = this.configuration.Parameters.MaxGeneration;

                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindResultControls();
        }

        private void BindResultControls()
        {
            switch (this.comboBox2.Text)
            {
                case "按班级":
                    this.richTextBox1.Text = this.resultPerStudentsGroup;
                    break;

                case "按老师":
                    this.richTextBox1.Text = this.resultPerTeacher;
                    break;

                default:
                    break;
            }
        }

        private void InputViewMenuItem_Click(object sender, EventArgs e)
        {
            this.SwitchView(false);
        }

        private void OutputMenuItem_Click(object sender, EventArgs e)
        {
            this.SwitchView(true);
        }

        private void SetStatusBar()
        {
            this.toolStripStatusLabel2.Visible = false;
            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Text = (this.configuration != null && this.configuration.VerifyContent()) ? "Ready" : "Not ready";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            this.ga = new GeneticAlgorithm(this.configuration, new TestObserver(worker));
            this.ga.Start();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripStatusLabel1.Text = "Computing...";
            this.toolStripStatusLabel2.Text = e.UserState.ToString(); ;
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.SwitchView(true);
            this.resultPerStudentsGroup = ResultAnalyzer.GetResultByStudentsGroups(this.configuration, this.ga);
            this.BindResultControls();
            this.toolStripStatusLabel1.Text = "Complete";
            this.toolStripProgressBar1.Visible = false;
            MessageBox.Show("Done");
        }
    }
}