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

        // Population of chromosomes
        // 染色体族群，染色体数量由算法用户指定，不能小于2个
        public Schedule[] Chromosomes { get; set; }

        // Inidicates wheahter chromosome belongs to best chromosome group
        // 用于标记最优染色体的数组，长度等于族群中染色体的总数
        public bool[] BestFlags;

        // Indices of best chromosomes
        // 最优染色体的索引表，长度是由算法的用户指定的，是固定的
        private int[] bestChromosomes;

        // Number of best chromosomes currently saved in best chromosome group
        // 当前选出的最优染色体的数量，初值为0，在计算过程中会发生变化
        private int currentBestSize;

        // Number of chromosomes which are replaced in each generation by offspring
        // 每代中被后代替换的染色体的数量
        public int ReplaceByGeneration { get; set; }

        private ScheduleObserver observer;

        // Prototype of chromosomes in population
        // 种群中染色体的原型
        private Schedule prototype;

        // Current generation
        // 当代
        private int currentGeneration;

        // State of execution of algorithm
        // 算法的执行状态
        private AlgorithmState state;

        // Synchronization of algorithm's state
        //private CCriticalSection _stateSect;

        // Pointer to global instance of algorithm
        // 本算法类的全局实例
        static GeneticAlgorithm instance;

        // Synchronization of creation and destruction of global instance
        //static CCriticalSection _instanceSect;

        #endregion

        #region Constructors and Destructors

        // Returns reference to global instance of algorithm
        // 返回算法的全局实例
        public static GeneticAlgorithm GetInstance()
        {
            if (instance == null)
            {
                // set seed for random generator
                //srand(GetTickCount());

                // make prototype of chromosomes
                // 创建染色体原型:
                //  (1) 交换点数: 2
                //  (2) 突变点数: 2
                //  (3) 杂交概率: 80
                //  (4) 突变概率: 3
                Schedule prototype = new Schedule(2, 2, 80, 3);

                // make new global instance of algorithm using chromosome prototype
                // 使用染色体原型创建算法实例：
                //  (1) 族群中有100个染色体
                //  (2) 每次有8个染色体会被后代替换掉
                //  (3) 只追踪5个最优解
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

        /// <summary>
        /// Initializes a new instance of the genetic algorithm class.
        /// 初始化算法
        /// </summary>
        /// <param name="numberOfChromosomes">染色体数量</param>
        /// <param name="replaceByGeneration">The replace by generation.每代替换的数量</param>
        /// <param name="trackBest">The track best.</param>
        /// <param name="prototype">The prototype.原型染色体</param>
        /// <param name="observer">The observer.观察者</param>
        public GeneticAlgorithm(int numberOfChromosomes, int replaceByGeneration, int trackBest,
            Schedule prototype, ScheduleObserver observer)
        {
            this.ReplaceByGeneration = replaceByGeneration;
            this.currentBestSize = 0;
            this.prototype = prototype;
            this.observer = observer;
            this.currentGeneration = 0;
            this.state = AlgorithmState.AS_USER_STOPED;

            // there should be at least 2 chromosomes in population
            // 族群中至少要有两个染色体
            if (numberOfChromosomes < 2)
            {
                numberOfChromosomes = 2;
            }

            // and algorithm should track at least on of best chromosomes
            // 至少要跟踪一个最优解
            if (trackBest < 1)
            {
                trackBest = 1;
            }

            // 每代至少替换一个染色体
            if (this.ReplaceByGeneration < 1)
            {
                this.ReplaceByGeneration = 1;
            }
            else if (replaceByGeneration > numberOfChromosomes - trackBest)
            {
                // 如果每代替换的染色体数大于染色体总数与最优染色体数之差，则将替换数修改为差
                replaceByGeneration = numberOfChromosomes - trackBest;
            }

            // reserve space for population
            // 为族群保留空间
            this.Chromosomes = new Schedule[numberOfChromosomes];
            this.BestFlags = new bool[numberOfChromosomes];

            // reserve space for best chromosome group
            // 初始化最优染色体索引
            this.bestChromosomes = new int[trackBest];

            // clear population
            // 清空族群
            //for (int i = Chromosomes.Length - 1; i >= 0; --i)
            //{
            //    Chromosomes[i] = null;
            //    BestFlags[i] = false;
            //}

        }

        // Frees used resources
        ~GeneticAlgorithm()
        {

        }

        #endregion

        #region Properties

        // Returns pointer to best chromosomes in population
        // 获得最优染色体
        public Schedule GetBestChromosome()
        {
            // 返回最优组里的第一个染色体
            return this.Chromosomes[this.bestChromosomes[0]];
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
        // 开始执行算法
        public void Start()
        {
            // 如果没有原型染色体，说明当前算法未被初始化，不能开始计算
            if (this.prototype == null)
            {
                return;
            }

            //CSingleLock lock (&_stateSect, TRUE );

            // do not run already running algorithm
            // 如果正在运算则返回
            if (this.state == AlgorithmState.AS_RUNNING)
            {
                return;
            }

            // 标记为正在计算
            this.state = AlgorithmState.AS_RUNNING;

            //lock.Unlock();

            // 通知观察者执行状态变化
            if (this.observer != null)
            {
                // notify observer that execution of algorithm has changed it state
                observer.EvolutionStateChanged(this.state);
            }

            // clear best chromosome group from previous execution
            // 清空上次执行的最优染色体组
            this.ClearBest();

            // initialize new population with chromosomes randomly built using prototype
            // 使用染色体初始化新族群，使用原型创建出所有的染色体
            for (int i = 0; i < this.Chromosomes.Length; i++)
            {
                // 通过原型创建染色体
                this.Chromosomes[i] = this.prototype.MakeNewFromPrototype();

                // 尝试加入最优组，初始化新族群时，最优解组也被填充了
                this.AddToBest(i);
            }

            // 当前代设为0
            this.currentGeneration = 0;

            // 开始运算，直到用户停止或算法找到最优解
            while (true)
            {
                //lock.Lock();

                // user has stopped execution?
                // 用户停止执行
                if (this.state != AlgorithmState.AS_RUNNING)
                {
                    //lock.Unlock();
                    break;
                }

                // 获得当前最优染色体
                Schedule best = this.GetBestChromosome();

                // algorithm has reached criteria?
                // 如果当前最优染色体已经是最优，结束计算
                if (best.Fitness >= 1 || this.currentGeneration > 100000)
                {
                    this.state = AlgorithmState.AS_CRITERIA_STOPPED;
                    //lock.Unlock();
                    break;
                }

                //lock.Unlock();

                // produce offspring
                // 按需要替换的染色体生产相应数量的后代
                Schedule[] offspring = new Schedule[this.ReplaceByGeneration];
                for (int j = 0; j < this.ReplaceByGeneration; j++)
                {
                    // selects parent randomly
                    // 在当前的族群中随机选取两个染色体作为双亲
                    Schedule p1 = Chromosomes[Rand() % Chromosomes.Length];
                    Schedule p2 = Chromosomes[Rand() % Chromosomes.Length];

                    // 进行杂交
                    offspring[j] = p1.Crossover(p2);

                    // 进行突变
                    offspring[j].Mutation();
                }

                // replace chromosomes of current operation with offspring
                // 用后代替换当前操作中的染色体
                foreach (var os in offspring)
                {
                    // 随机取得一个槽位用于替换
                    int ci;
                    do
                    {
                        // select chromosome for replacement randomly
                        // 随机选择一个将被替换的染色体
                        ci = this.Rand() % (int)this.Chromosomes.Length;

                        // protect best chromosomes from replacement
                        // 保证最后的染色体不被替换掉
                    } while (IsInBest(ci));

                    // replace chromosomes
                    // 用后代替换染色体
                    this.Chromosomes[ci] = os;

                    // try to add new chromosomes in best chromosome group
                    // 尝试将新染色体放入最优组中
                    AddToBest(ci);
                }

                // algorithm has found new best chromosome
                // 如果算法获得了更好的染色体
                var newBest = this.GetBestChromosome();
                if (best != newBest && this.observer!=null)
                    // notify observer
                    this.observer.NewBestChromosome(newBest);

                this.currentGeneration++;
            }

            if (this.observer!=null)
                // notify observer that execution of algorithm has changed it state
                this.observer.EvolutionStateChanged(state);
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
            // 如果新染色体已经是最优解
            if (this.BestFlags[chromosomeIndex])
            {
                return;
            }

            // 新染色体
            var newChrom = this.Chromosomes[chromosomeIndex];

            // don't add if new chromosome hasn't fitness big enough for best chromosome group
            // or it is already in the group?
            // 如果当前最优解组是满的，并且新染色体没有更好的适应性，则不加入最优解组
            if (this.currentBestSize == this.bestChromosomes.Length)
            {
                var lastBest = this.Chromosomes[this.bestChromosomes[this.currentBestSize - 1]];
                if (lastBest.Fitness >= newChrom.Fitness)
                {
                    return;
                }
            }


            // find place for new chromosome
            // 为最新的染色体在最优组里找一个位置
            int i = this.currentBestSize;
            for (; i > 0; i--)
            {
                // group is not full?
                // 如果最优解组不满
                if (i < this.bestChromosomes.Length)
                {
                    // position of new chromosomes is found?
                    var c = this.Chromosomes[this.bestChromosomes[i - 1]];
                    if (c.Fitness > newChrom.Fitness)
                    {
                        break;
                    }

                    // move chromosomes to make room for new
                    // 冒泡
                    this.bestChromosomes[i] = this.bestChromosomes[i - 1];
                }
                else
                {
                    // group is full remove worst chromosomes in the group
                    // 如果最优组已经满了，将最差的最优解移除掉
                    this.BestFlags[this.bestChromosomes[i - 1]] = false;
                }
            }

            // store chromosome in best chromosome group
            // 将新染色体放入最优组
            this.bestChromosomes[i] = chromosomeIndex;
            this.BestFlags[chromosomeIndex] = true;

            // increase current size if it has not reached the limit yet
            // 当前最优解的数量加1
            if (this.currentBestSize < this.bestChromosomes.Length)
            {
                this.currentBestSize++;
            }
        }

        // Returns TRUE if chromosome belongs to best chromosome group
        private bool IsInBest(int chromosomeIndex)
        {
            return this.BestFlags[chromosomeIndex];
        }

        // Clears best chromosome group
        private void ClearBest()
        {
            for (int i = 0; i < this.BestFlags.Length; i++)
            {
                this.BestFlags[i] = false;
            }

            this.currentBestSize = 0;
        }

        #endregion
    }
}
