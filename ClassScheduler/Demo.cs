using GaSchedule.Algorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassScheduler
{
    public partial class Demo : Form
    {
        private Configuration configuration;
        public Demo()
        {
            InitializeComponent();
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            this.configuration = ConfigurationFactory.CreateFromJson(@"config.json");
            bool isReady = this.configuration.VerifyContent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (this.configuration != null)
            {
                GeneticAlgorithm ga = new GeneticAlgorithm(this.configuration, null);
                ga.Start();
            }
        }
    }
}
