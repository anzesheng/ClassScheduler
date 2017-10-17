using GaSchedule.Algorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassScheduler
{
    class TestObserver : IScheduleObserver
    {
        private BackgroundWorker worker;

        public TestObserver(
            BackgroundWorker worker)
        {
            this.worker = worker;
        }

        public void EvolutionStateChanged(AlgorithmState newState)
        {
            //throw new NotImplementedException();
        }

        public void NewBestChromosome(int generation, float fitness)
        {
            this.worker.ReportProgress(generation, $"Generation: {generation}, Fitness: {fitness}");
        }
    }
}
