using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Algorithm
{
    public class ScheduleObserver
    {
        // Handles event that is raised when state of execution of algorithm is changed
        public void EvolutionStateChanged(AlgorithmState newState)
        {
            //if (_window)
            //    _window->SetNewState(newState);

            //newState != AS_RUNNING ? ReleaseEvent() : BlockEvent();
            throw new NotImplementedException();
        }
    }
}
