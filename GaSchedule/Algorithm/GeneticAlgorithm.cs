using System;

namespace GaSchedule.Algorithm
{
    public class GeneticAlgorithm
    {
        #region Fields

        /// <summary>
        /// 最优染色体的索引表，长度是由算法的用户指定的，是固定的。
        /// </summary>
        private int[] bestChromosomes;

        /// <summary>
        /// 当前选出的最优染色体的数量，初值为0，在计算过程中会发生变化。
        /// </summary>
        private int currentBestSize;

        /// <summary>
        /// 观察者。
        /// </summary>
        private IScheduleObserver observer;

        /// <summary>
        /// 种群中染色体的原型。
        /// </summary>
        private Schedule prototype;

        /// <summary>
        /// 随机整数生成器。
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// 算法的当前执行状态。
        /// </summary>
        private AlgorithmState state;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 创建一个 <see cref="GeneticAlgorithm"/> 类实例。
        /// </summary>
        /// <param name="configuration">算法参数</param>
        /// <param name="observer">观察者</param>
        public GeneticAlgorithm(Configuration configuration, IScheduleObserver observer)
        {
            this.Configuration = configuration;
            this.observer = observer;

            this.currentBestSize = 0;
            this.CurrentGeneration = 0;
            this.state = AlgorithmState.AS_USER_STOPED;

            // 创建染色体原型
            this.prototype = new Schedule(this.Configuration);

            // 为族群保留空间
            this.Chromosomes = new Schedule[this.Configuration.Parameters.NumberOfChromosomes];

            // 初始化最优染色体索引
            this.BestFlags = new bool[this.Configuration.Parameters.NumberOfChromosomes];
            this.bestChromosomes = new int[this.Configuration.Parameters.TrackBestNumber];
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 用于标记最优染色体的数组，长度等于族群中染色体的总数。
        /// </summary>
        public bool[] BestFlags { get; set; }

        /// <summary>
        /// 染色体族群，染色体数量由算法用户指定，不能小于2个。
        /// </summary>
        public Schedule[] Chromosomes { get; set; }

        /// <summary>
        /// 算法的参数集合
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// 当代。
        /// </summary>
        public int CurrentGeneration { get; private set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        private AlgorithmState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.state = value;

                // 通知观察者执行状态变化
                this.observer?.EvolutionStateChanged(this.state);
            }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// 获得最优染色体。
        /// </summary>
        /// <returns>当前最优染色体</returns>
        public Schedule GetBestChromosome()
        {
            // 返回最优组里的第一个染色体
            return this.Chromosomes[this.bestChromosomes[0]];
        }

        // 开始执行算法
        public void Start()
        {
            // 如果没有原型染色体，说明当前算法未被初始化，不能开始计算
            if (this.prototype == null)
            {
                throw new Exception("没有原型染色体，当前算法未被初始化，不能开始计算。");
            }

            // 如果正在运算则返回
            if (this.state == AlgorithmState.AS_RUNNING)
            {
                return;
            }

            // 将状态修改为正在计算
            this.state = AlgorithmState.AS_RUNNING;

            // 清空上次执行的最优染色体组
            this.ClearBest();

            // 使用染色体初始化新族群，使用原型创建出所有的染色体
            for (int i = 0; i < this.Chromosomes.Length; i++)
            {
                // 通过原型创建染色体
                this.Chromosomes[i] = this.prototype.MakeNewFromPrototype();

                // 尝试加入最优组，初始化新族群时，最优解组也被填充了
                this.AddToBest(i);
            }

            // 当前代设为0
            this.CurrentGeneration = 0;

            // 开始运算，直到用户停止或算法找到最优解
            while (true)
            {
                // 用户停止了执行？
                if (this.state != AlgorithmState.AS_RUNNING)
                {
                    break;
                }

                // 获得当前最优染色体
                Schedule best = this.GetBestChromosome();

                // 如果当前最优染色体已经是最优，结束计算
                if ((best.Fitness >= this.Configuration.Parameters.MinFitness && best.Evenness >= this.Configuration.Parameters.MinEvenness) ||
                    this.CurrentGeneration > this.Configuration.Parameters.MaxGeneration)
                {
                    this.state = AlgorithmState.AS_CRITERIA_STOPPED;
                    break;
                }

                // 按需要替换的染色体生产相应数量的后代
                Schedule[] offspring = new Schedule[this.Configuration.Parameters.ReplaceByGeneration];
                for (int j = 0; j < this.Configuration.Parameters.ReplaceByGeneration; j++)
                {
                    // selects parent randomly
                    // 在当前的族群中随机选取两个染色体作为双亲
                    Schedule p1 = this.Chromosomes[this.Rand() % this.Chromosomes.Length];
                    Schedule p2 = this.Chromosomes[this.Rand() % this.Chromosomes.Length];

                    // 进行杂交
                    offspring[j] = p1.Crossover(p2);

                    // 进行突变
                    offspring[j].Mutation();
                }

                // 用后代替换当前操作中的染色体
                foreach (var os in offspring)
                {
                    // 随机取得一个槽位用于替换
                    int ci;
                    do
                    {
                        // 随机选择一个将被替换的染色体
                        ci = this.Rand() % this.Chromosomes.Length;
                    }
                    while (this.IsInBest(ci)); // 保证最后的染色体不被替换掉

                    // 用后代替换染色体
                    this.Chromosomes[ci] = os;

                    // 尝试将新染色体放入最优组中
                    this.AddToBest(ci);
                }

                // 如果算法获得了更好的染色体
                var newBest = this.GetBestChromosome();
                if (best != newBest && this.observer != null)
                {
                    this.observer.NewBestChromosome(this.CurrentGeneration, newBest.Fitness, newBest.Evenness);
                }

                this.CurrentGeneration++;
            }
        }

        /// <summary>
        /// Stops execution of algoruthm.
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Tries to add chromosomes in best chromosome group.
        /// </summary>
        /// <param name="chromosomeIndex">Index of the chromosome.</param>
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
                if (lastBest.Fitness >= newChrom.Fitness && lastBest.Evenness >= newChrom.Evenness)
                {
                    return;
                }

                //if (lastBest.Fitness + lastBest.Evenness >= newChrom.Fitness + newChrom.Evenness)
                //{
                //    return;
                //}
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
                    if (c.Fitness >= newChrom.Fitness && c.Evenness >= newChrom.Evenness)
                    {
                        break;
                    }

                    //if (c.Fitness + c.Evenness >= newChrom.Fitness + newChrom.Evenness)
                    //{
                    //    break;
                    //}

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

        /// <summary>
        /// Clears best chromosome group.
        /// </summary>
        private void ClearBest()
        {
            for (int i = 0; i < this.BestFlags.Length; i++)
            {
                this.BestFlags[i] = false;
            }

            this.currentBestSize = 0;
        }

        /// <summary>
        /// 判断染色体是否在最优列表中。
        /// </summary>
        /// <param name="chromosomeIndex">Index of the chromosome.</param>
        /// <returns>
        ///   <c>true</c> if [is in best] [the specified chromosome index]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsInBest(int chromosomeIndex)
        {
            return this.BestFlags[chromosomeIndex];
        }

        /// <summary>
        /// 获得一个随机整数
        /// </summary>
        /// <returns>一个随机整数</returns>
        private int Rand()
        {
            return this.random.Next();
        }

        #endregion Private Methods
    }
}