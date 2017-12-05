namespace GaSchedule.Algorithm
{
    public interface IScheduleObserver
    {
        void NewBestChromosome(int generation, float fitness);

        void EvolutionStateChanged(AlgorithmState newState);
    }
}