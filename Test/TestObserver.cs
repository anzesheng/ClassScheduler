using GaSchedule.Algorithm;
using System.ComponentModel;

namespace ClassScheduler
{
    internal class TestObserver : IScheduleObserver
    {
        private BackgroundWorker worker;

        public TestObserver(
            BackgroundWorker worker)
        {
            this.worker = worker;
        }

        public void EvolutionStateChanged(AlgorithmState newState)
        {
            // throw new NotImplementedException();
        }

        public void NewBestChromosome(int generation, float fitness, float evenness)
        {
            this.worker.ReportProgress(generation, $"Generation: {generation}, Fitness: {fitness}, Evenness: {evenness}");
        }
    }
}