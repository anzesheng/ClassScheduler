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

        private Random random = new Random();

        private int Rand()
        {
            return this.random.Next();
        }

        private Schedule[] chromosomes;

        private bool[] bestFlags;

        private int[] bestChromosomes;

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
            if (instance == null)
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
            //CSingleLock lock (&_instanceSect, TRUE );

            // free memory used by global instance if it exists
            //if (instance != null)
            //{
            //    delete _instance->_prototype;
            //    delete _instance->_observer;
            //    delete _instance;

            //    _instance = NULL;
            //}
        }

        // Initializes genetic algorithm
        public GeneticAlgorithm(int numberOfChromosomes, int replaceByGeneration, int trackBest,
            Schedule prototype, ScheduleObserver observer)
        {
            this.replaceByGeneration = replaceByGeneration;
            this.currentBestSize = 0;
            this.prototype = prototype;
            this.observer = observer;
            this.currentGeneration = 0;
            this.state = AlgorithmState.AS_USER_STOPED;

            // there should be at least 2 chromosomes in population
            if (numberOfChromosomes < 2)
                numberOfChromosomes = 2;

            // and algorithm should track at least on of best chromosomes
            if (trackBest < 1)
                trackBest = 1;

            if (this.replaceByGeneration < 1)
            {
                this.replaceByGeneration = 1;
            }
            else if (replaceByGeneration > numberOfChromosomes - trackBest)
            {
                replaceByGeneration = numberOfChromosomes - trackBest;
            }

            // reserve space for population
            chromosomes = new Schedule[numberOfChromosomes];
            bestFlags = new bool[numberOfChromosomes];

            // reserve space for best chromosome group
            bestChromosomes = new int[trackBest];

            // clear population
            for (int i = chromosomes.Length - 1; i >= 0; --i)
            {
                chromosomes[i] = null;
                bestFlags[i] = false;
            }

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
            if (this.prototype == null)
            {
                return;
            }

            //CSingleLock lock (&_stateSect, TRUE );

            // do not run already running algorithm
            if (this.state == AlgorithmState.AS_RUNNING)
            {
                return;
            }

            this.state = AlgorithmState.AS_RUNNING;

            //lock.Unlock();

            if (this.observer != null)
                // notify observer that execution of algorithm has changed it state
                observer.EvolutionStateChanged(this.state);

            // clear best chromosome group from previous execution
            ClearBest();

            // initialize new population with chromosomes randomly built using prototype
            for (int i = 0; i < this.chromosomes.Length; i++)
            {
                this.chromosomes[i] = this.prototype.MakeNewFromPrototype();
                AddToBest(i);
            }

            this.currentGeneration = 0;

            while (true)
            {
                //lock.Lock();

                // user has stopped execution?
                if (this.state != AlgorithmState.AS_RUNNING)
                {
                    //lock.Unlock();
                    break;
                }

                Schedule best = GetBestChromosome();

                // algorithm has reached criteria?
                if (best.Fitness >= 1)
                {
                    this.state = AlgorithmState.AS_CRITERIA_STOPPED;
                    //lock.Unlock();
                    break;
                }

                //lock.Unlock();

                // produce offepsing
                Schedule[] offspring = new Schedule[this.replaceByGeneration];
                for (int j = 0; j < this.replaceByGeneration; j++)
                {
                    // selects parent randomly随机选取双亲
                    Schedule p1 = chromosomes[Rand() % chromosomes.Length];
                    Schedule p2 = chromosomes[Rand() % chromosomes.Length];

                    // 产生后代，并进行突变
                    offspring[j] = p1.Crossover(p2);
                    offspring[j].Mutation();
                }

                // replace chromosomes of current operation with offspring
                for (int j = 0; j < _replaceByGeneration; j++)
                {
                    int ci;
                    do
                    {
                        // select chromosome for replacement randomly
                        ci = rand() % (int)_chromosomes.size();

                        // protect best chromosomes from replacement
                    } while (IsInBest(ci));

                    // replace chromosomes
                    delete _chromosomes[ci];
                    _chromosomes[ci] = offspring[j];

                    // try to add new chromosomes in best chromosome group
                    AddToBest(ci);
                }

                // algorithm has found new best chromosome
                if (best != GetBestChromosome() && _observer)
                    // notify observer
                    _observer->NewBestChromosome(*GetBestChromosome());

                _currentGeneration++;
            }

            if (_observer)
                // notify observer that execution of algorithm has changed it state
                _observer->EvolutionStateChanged(_state);
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
