using GaSchedule.Algorithm;
using GaSchedule.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.SetStatusBar();
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

                //this.configuration = ConfigurationFactory.CreateFromJson(path);
                //MessageBox.Show("文件加载成功！");
            }
            catch (Exception)
            {
                MessageBox.Show($"解析'{path}'文件时发生错误。");
                return false;
            }

            this.SetStatusBar();
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
            if (this.configuration != null)
            {
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    backgroundWorker1.RunWorkerAsync();
                }
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

            this.ga = new GeneticAlgorithm(this.configuration, new TestObserver(this.toolStripStatusLabel1, this.toolStripStatusLabel2, this.toolStripProgressBar1));
            this.ga.Start();

            //for (int i = 1; i <= 10; i++)
            //{
            //    if (worker.CancellationPending == true)
            //    {
            //        e.Cancel = true;
            //        break;
            //    }
            //    else
            //    {
            //        // Perform a time consuming operation and report progress.
            //        System.Threading.Thread.Sleep(500);
            //        worker.ReportProgress(i * 10);
            //    }
            //}
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripStatusLabel1.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.SwitchView(true);
            this.resultPerStudentsGroup = ResultAnalyzer.GetResultByStudentsGroups(this.configuration, this.ga);
            this.BindResultControls();
            MessageBox.Show("Done");
        }
    }
}
