namespace GaSchedule.Algorithm
{
    public class AlgorithmParameters
    {
        // Number of working hours per day
        public int NumberOfClassPerDay { get; set; } = 6;

        // Number of days in week
        public int WorkingDaysNumber { get; set; } = 5;

        public int MaxGeneration { get; set; } = 10000;

        /// <summary>
        /// 每代族群中染色体的数量
        /// </summary>
        public int NumberOfChromosomes { get; set; } = 100;

        /// <summary>
        /// Number of chromosomes which are replaced in each generation by offspring
        // 每代族群中被后代替换的染色体的数量
        /// </summary>
        public int ReplaceByGeneration { get; set; } = 8;

        /// <summary>
        /// 最优组的长度
        /// </summary>
        public int TrackBestNumber { get; set; } = 5;

        // Number of crossover points of parent's class tables
        // 多点杂交中交换点的数量
        public int NumberOfCrossoverPoints { get; set; } = 2;

        // Number of classes that is moved randomly by single mutation operation
        // 一次突变中随机移动的课堂的数量
        public int MutationSize { get; set; } = 2;

        // Probability that crossover will occure
        // 交叉发生的概率
        public int CrossoverProbability { get; set; } = 80;

        // Probability that mutation will occure
        // 突变发生的概率
        public int MutationProbability { get; set; } = 3;

        public void VerifyParameters()
        {
            // there should be at least 2 chromosomes in population
            // 族群中至少要有两个染色体
            if (this.NumberOfChromosomes < 2)
            {
                this.NumberOfChromosomes = 2;
            }

            // and algorithm should track at least on of best chromosomes
            // 至少要跟踪一个最优解
            if (this.TrackBestNumber < 1)
            {
                this.TrackBestNumber = 1;
            }

            // 每代至少替换一个染色体
            if (this.ReplaceByGeneration < 1)
            {
                this.ReplaceByGeneration = 1;
            }
            else if (this.ReplaceByGeneration > this.NumberOfChromosomes - this.TrackBestNumber)
            {
                // 如果每代替换的染色体数大于染色体总数与最优染色体数之差，则将替换数修改为差
                this.ReplaceByGeneration = this.NumberOfChromosomes - this.TrackBestNumber;
            }
        }
    }
}