using ClassScheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Algorithm
{
    public enum AlgorithmState
    {
        AS_USER_STOPED,
        AS_CRITERIA_STOPPED,
        AS_RUNNING
    };

    public class GeneticAlgorithm
    {
        #region Fields

        private List<Schedule> chromosomes;

        private List<bool> bastFlags;

        private List<int> bastChromosomes;

        private int currentBestSize;

        private int replaceByGeneration;

        private ScheduleObserver observer;

        // Prototype of chromosomes in population
        private Schedule prototype;

        // Current generation
        private int currentGeneration;

        // State of execution of algorithm
        private AlgorithmState state;

        // Synchronization of algorithm's state
        //private CCriticalSection _stateSect;

        // Pointer to global instance of algorithm
        static GeneticAlgorithm instance;

        // Synchronization of creation and destruction of global instance
        //static CCriticalSection _instanceSect;

        #endregion

        #region Constructors and Destructors

        // Returns reference to global instance of algorithm
        public static GeneticAlgorithm GetInstance()
        {
            if(instance == null)
            {
                // set seed for random generator
                //srand(GetTickCount());

                // make prototype of chromosomes
                Schedule prototype = new Schedule(2, 2, 80, 3);

                // make new global instance of algorithm using chromosome prototype
                instance = new GeneticAlgorithm(100, 8, 5, prototype, new ScheduleObserver());
            }

            return instance;
        }

        // Frees memory used by gloval instance
        public static void FreeInstance()
        {

        }

        // Initializes genetic algorithm
        public GeneticAlgorithm(int numberOfChromosomes, int replaceByGeneration, int trackBest,
            Schedule prototype, ScheduleObserver observer)
        {

        }

        // Frees used resources
        ~GeneticAlgorithm()
        {

        }

        #endregion

        #region Properties

        // Returns pointer to best chromosomes in population
        public Schedule GetBestChromosome()
        {
            throw new NotImplementedException();
        }

        // Returns current generation
        public int GetCurrentGeneration()
        {
            return this.currentGeneration;
        }

        // Returns pointe to algorithm's observer
        public ScheduleObserver GetObserver()
        {
            return this.observer;
        }

        #endregion

        #region Public Methods

        // Starts and executes algorithm
        public void Start()
        {
            throw new NotImplementedException();
        }

        // Stops execution of algoruthm
        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        // Tries to add chromosomes in best chromosome group
        private void AddToBest(int chromosomeIndex)
        {
            throw new NotImplementedException();
        }

        // Returns TRUE if chromosome belongs to best chromosome group
        private bool IsInBest(int chromosomeIndex)
        {
            throw new NotImplementedException();
        }

        // Clears best chromosome group
        private void ClearBest()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
