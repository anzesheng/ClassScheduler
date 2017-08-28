using ClassScheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Algorithm
{
    public class Schedule
    {
        // Number of working hours per day
        public const int DAY_HOURS = 12;

        // Number of days in week
        public const int DAYS_NUM = 5;

        private Random random = new Random();

        private int Rand()
        {
            return this.random.Next(0, 100);
        }


        #region Fields

        // Number of crossover points of parent's class tables
        private int numberOfCrossoverPoints;

        // Number of classes that is moved randomly by single mutation operation
        private int mutationSize;

        // Probability that crossover will occure
        private int crossoverProbability;

        // Probability that mutation will occure
        private int mutationProbability;

        /// <summary>
        /// Fitness value of chromosome
        /// </summary>
        float fitness;

        /// <summary>
        /// Flags of class requiroments satisfaction
        /// </summary>
        List<bool> criteria;

        /// <summary>
        /// Time-space slots, one entry represent one hour in one classroom
        /// </summary>
        public List<List<CourseClass>> Slots { get; set; }

        /// <summary>
        /// Class table for chromosome
        /// Used to determine first time-space slot used by class
        /// </summary>
        private Dictionary<CourseClass, int> classes;

        #endregion

        #region Constructors and Destructors

        // Initializes chromosomes with configuration block (setup of chromosome)
        public Schedule(int numberOfCrossoverPoints, int mutationSize,
            int crossoverProbability, int mutationProbability)
        {
            this.mutationSize = mutationSize;
            this.numberOfCrossoverPoints = numberOfCrossoverPoints;
            this.crossoverProbability = crossoverProbability;
            this.mutationProbability = mutationProbability;
            this.fitness = 0;

            // reserve space for time-space slots in chromosomes code
            //_slots.resize(DAYS_NUM * DAY_HOURS * Configuration::GetInstance().GetNumberOfRooms());

            // reserve space for flags of class requirements
            //_criteria.resize(Configuration::GetInstance().GetNumberOfCourseClasses() * 5);
        }

        // Copy constructor
        public Schedule(Schedule c, bool setupOnly)
        {
            if (setupOnly)
            {
                // reserve space for time-space slots in chromosomes code
                //_slots.resize(DAYS_NUM * DAY_HOURS * Configuration::GetInstance().GetNumberOfRooms());

                // reserve space for flags of class requirements
                //_criteria.resize(Configuration::GetInstance().GetNumberOfCourseClasses() * 5);
            }
            else
            {
                // copy code
                this.Slots = c.Slots;
                this.classes = c.classes;

                // copy flags of class requirements
                this.criteria = c.criteria;

                // copy fitness
                this.fitness = c.fitness;
            }

            // copy parameters
            this.numberOfCrossoverPoints = c.numberOfCrossoverPoints;
            this.mutationSize = c.mutationSize;
            this.crossoverProbability = c.crossoverProbability;
            this.mutationProbability = c.mutationProbability;
        }

        // Makes copy ot chromosome
        public Schedule MakeCopy(bool setupOnly)
        {
            // make object by calling copy constructor and return smart pointer to new object
            return new Schedule(this, setupOnly);
        }

        // Makes new chromosome with same setup but with randomly chosen code
        public Schedule MakeNewFromPrototype()
        {
            // number of time-space slots
            int size = this.slots.Count;

            // make new chromosome, copy chromosome setup
            Schedule newChromosome = new Schedule(this, true);

            // place classes at random position
            List<CourseClass> classes = Configuration.GetInstance().GetCourseClasses();
            foreach (CourseClass c in classes)
            {
                // determine random position of class
                int nr = Configuration.GetInstance().GetNumberOfRooms();
                int dur = c.Duration;
                int day = this.Rand() % DAYS_NUM;
                int room = this.Rand() % nr;
                int time = this.Rand() % (DAY_HOURS + 1 - dur);
                int pos = day * nr * DAY_HOURS + room * DAY_HOURS + time;

                // fill time-space slots, for each hour of class
                for (int i = dur - 1; i >= 0; i--)
                {
                    newChromosome.._slots.at(pos + i).a.push_back(c);
                }

                // insert in class table of chromosome
                newChromosome->_classes.insert(pair<CourseClass*, int>(*c, pos));
            }

            newChromosome->CalculateFitness();

            // return smart pointer
            return newChromosome;
        }

        // Performes crossover operation using to chromosomes and returns pointer to offspring
        public Schedule Crossover(Schedule parent)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        // Returns reference to table of classes
        public Dictionary<CourseClass, int> GetClasses()
        {
            return this.classes;
        }

        // Returns array of flags of class requiroments satisfaction
        public List<bool> GetCriteria()
        {
            return this.criteria;
        }

        // Return reference to array of time-space slots
        public List<List<CourseClass>> GetSlots()
        {
            return this.slots;
        }

        #endregion

        #region Members

        // Performs mutation on chromosome
        public void Mutation()
        {
            throw new NotImplementedException();
        }

        // Calculates fitness value of chromosome
        public void CalculateFitness()
        {
            throw new NotImplementedException();
        }

        // Returns fitness value of chromosome
        public float GetFitness()
        {
            return this.fitness;
        }

        #endregion
    }
}
