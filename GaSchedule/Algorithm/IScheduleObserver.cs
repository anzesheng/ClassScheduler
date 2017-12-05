namespace GaSchedule.Algorithm
{
    public interface IScheduleObserver
    {
        void NewBestChromosome(int generation, float fitness, float evenness);

        void EvolutionStateChanged(AlgorithmState newState);
    }
}