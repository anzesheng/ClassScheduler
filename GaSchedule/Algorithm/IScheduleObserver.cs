using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Algorithm
{
    public interface IScheduleObserver
    {
        void NewBestChromosome(Schedule newChromosome);
        void EvolutionStateChanged(AlgorithmState newState);
    }
}
