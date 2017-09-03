using ClassScheduler.Algorithm;
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
        public Demo()
        {
            InitializeComponent();
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            Configuration.GetInstance().ParseFile(@"config.json");
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            GeneticAlgorithm.GetInstance().Start();
        }
    }
}
