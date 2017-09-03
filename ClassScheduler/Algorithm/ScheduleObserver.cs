using System;

namespace ClassScheduler.Algorithm
{
    public class ScheduleObserver
    {
        // Handles event that is raised when algorithm finds new best chromosome
        public void NewBestChromosome(Schedule newChromosome)
        {
            //if (_window)
            //    _window->SetSchedule(&newChromosome);
            Console.WriteLine("New best chromosome was found.");
        }

        // Handles event that is raised when state of execution of algorithm is changed
        public void EvolutionStateChanged(AlgorithmState newState)
        {
            //if (_window)
            //    _window->SetNewState(newState);

            //newState != AS_RUNNING ? ReleaseEvent() : BlockEvent();
            Console.WriteLine($"The state of execution of algorithm is changed: {newState.ToString()}");
        }
    }
}
