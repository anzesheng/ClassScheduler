using GaSchedule.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassScheduler
{
    class TestObserver : IScheduleObserver
    {
        private ToolStripStatusLabel statusLabel1;
        private ToolStripStatusLabel statusLabel2;
        private ToolStripProgressBar progressBar;

        public TestObserver(ToolStripStatusLabel lable1, ToolStripStatusLabel lable2, ToolStripProgressBar progressBar)
        {
            this.statusLabel1 = lable1;
            this.statusLabel2 = lable2;
            this.progressBar = progressBar;
        }

        public void EvolutionStateChanged(AlgorithmState newState)
        {
            //throw new NotImplementedException();
        }

        public void NewBestChromosome(Schedule newChromosome)
        {
            this.statusLabel1.Text = $"Fitness: {newChromosome.Fitness}";
            //this.statusLabel2.Text = $"Generation: {newChromosome.ge.Fitness}";
            //throw new NotImplementedException();
        }
    }
}
