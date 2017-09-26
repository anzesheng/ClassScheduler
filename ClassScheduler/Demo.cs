using GaSchedule.Algorithm;
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
    public partial class Demo : Form
    {
        private Configuration configuration;
        private string resultPerStudentsGroup;
        private string resultPerTeacher;
        private string resultPerClassRoom;

        internal bool IsReady
        {
            set
            {
                this.buttonStart.Enabled = value;
            }
        }

        public Demo()
        {
            InitializeComponent();
            this.IsReady = false;
            this.textBoxJsonFile.Text = @"config.json";
            this.comboBoxResultBy.SelectedIndex = 0;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (this.configuration != null)
            {
                GeneticAlgorithm ga = new GeneticAlgorithm(this.configuration, null);
                ga.Start();

                this.resultPerStudentsGroup = ResultAnalyzer.GetResultByStudentsGroups(this.configuration, ga);
                this.textBoxResult.Text = this.resultPerStudentsGroup;
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBoxJsonFile.Text.Trim()) || !File.Exists(this.textBoxJsonFile.Text))
            {
                if (this.openJsonFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.textBoxJsonFile.Text = this.openJsonFileDialog.FileName;
                }
            }

            this.OpenConfigFile(this.textBoxJsonFile.Text);
        }

        private void OpenConfigFile(string path)
        {
            try
            {
                this.configuration = ConfigurationFactory.CreateFromJson(path);
                this.IsReady = this.configuration.VerifyContent();
                MessageBox.Show("文件加载成功！");
            }
            catch (Exception)
            {
                this.IsReady = false;
                MessageBox.Show($"解析'{path}'文件时发生错误。");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputForm inputForm = new InputForm(this.textBoxJsonFile.Text, this.configuration);
            inputForm.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBoxResultBy.Text)
            {
                case "按班级":
                    this.textBoxResult.Text = this.resultPerStudentsGroup;
                    break;
                case "按老师":
                    this.textBoxResult.Text = this.resultPerTeacher;
                    break;
                case "按教室":
                    this.textBoxResult.Text = this.resultPerClassRoom;
                    break;
                default:
                    break;
            }

        }
    }
}
