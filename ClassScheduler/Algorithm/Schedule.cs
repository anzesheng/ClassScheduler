using ClassScheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassScheduler.Algorithm
{
    /// <summary>
    /// 本遗传算法中的染色体（基因串）
    /// </summary>
    public class Schedule
    {
        // Number of working hours per day
        public const int DAY_HOURS = 12;

        // Number of days in week
        public const int DAYS_NUM = 5;

        private Random random = new Random();

        private int Rand()
        {
            return this.random.Next();
        }


        #region Fields

        // Number of crossover points of parent's class tables
        // 多点杂交中交换点的数量
        private int numberOfCrossoverPoints;

        // Number of classes that is moved randomly by single mutation operation
        // 一次突变中随机移动的课堂的数量
        private int mutationSize;

        // Probability that crossover will occure
        // 交叉发生的概率
        private int crossoverProbability;

        // Probability that mutation will occure
        // 突变发生的概率
        private int mutationProbability;

        // Fitness value of chromosome
        // 染色体的适应度
        public float Fitness { get; set; }

        private bool[] criteria;

        /// <summary>
        /// Flags of class requiroments satisfaction
        /// </summary>
        public bool[] Criteria
        {
            get
            {
                return this.criteria;
            }
            set
            {
                this.criteria = value;
            }
        }

        /// <summary>
        /// Time-space slots, one entry represent one hour in one classroom
        /// 时间槽列表。一个时间槽表示一间教室的一个小时。
        /// 在这里一个时间槽里可以放置多个课堂，因为计算过程中会出现冲突的情况。
        /// 该列表的长度 = 上课日数×每日课时数×教室数
        /// 某一节课的时间槽的序号 = (在第几个工作日-1)×教室总数×每日的课时数 + 在第几个工作日×在第几间教室×在第几时段
        /// </summary>
        private List<CourseClass>[] slots;

        public List<CourseClass>[] Slots
        {
            get
            {
                return this.slots;
            }
            set
            {
                this.slots = value;
            }
        }

        /// <summary>
        /// Class table for chromosome
        /// Used to determine first time-space slot used by class
        /// 排课总表词典，key是课堂，value是
        /// </summary>
        public Dictionary<CourseClass, int> Classes { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes chromosomes with configuration block (setup of chromosome)
        /// 初始化染色体
        /// </summary>
        /// <param name="numberOfCrossoverPoints">交换点的数量</param>
        /// <param name="mutationSize">突变的数量</param>
        /// <param name="crossoverProbability">杂交的可能性</param>
        /// <param name="mutationProbability">突变的可能性</param>
        public Schedule(int numberOfCrossoverPoints, int mutationSize,
            int crossoverProbability, int mutationProbability)
        {
            this.numberOfCrossoverPoints = numberOfCrossoverPoints;
            this.mutationSize = mutationSize;
            this.crossoverProbability = crossoverProbability;
            this.mutationProbability = mutationProbability;
            this.Fitness = 0;

            // reserve space for time-space slots in chromosomes code
            //_slots.resize(DAYS_NUM * DAY_HOURS * Configuration::GetInstance().GetNumberOfRooms());
            this.slots = new List<CourseClass>[DAYS_NUM * DAY_HOURS * Configuration.GetInstance().Classrooms.Count];

            // reserve space for flags of class requirements
            //_criteria.resize(Configuration::GetInstance().GetNumberOfCourseClasses() * 5);
            int num = Configuration.GetInstance().Courses.Count * 5;
            this.criteria = new bool[num];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// Copy constructor
        /// 拷贝构造
        /// </summary>
        /// <param name="c">被拷贝者</param>
        /// <param name="setupOnly">是否只拷贝size。True时只拷贝size，不拷贝内容。</param>
        public Schedule(Schedule c, bool setupOnly)
        {
            if (setupOnly)
            {
                // reserve space for time-space slots in chromosomes code
                // 准备好时间槽容器
                Array.Resize(ref this.slots, DAYS_NUM * DAY_HOURS * Configuration.GetInstance().Classrooms.Count);

                // reserve space for flags of class requirements
                // 准备好标准数组（为什么乘以5？）
                Array.Resize(ref this.criteria, Configuration.GetInstance().Classrooms.Count * 5);
            }
            else
            {
                // copy code
                // 拷贝染色体编码
                this.Slots = c.Slots;
                this.Classes = c.Classes;

                // copy flags of class requirements
                // 拷贝标准数组
                this.Criteria = c.Criteria;

                // copy fitness
                // 拷贝适应性
                this.Fitness = c.Fitness;
            }

            // copy parameters
            // 拷贝参数
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
        // 创建新染色体，拷贝设置但是随即排列课程
        public Schedule MakeNewFromPrototype()
        {
            // make new chromosome, copy chromosome setup
            // 创建新染色体，只拷贝当前染色体的size，不拷贝内容
            Schedule newChromosome = new Schedule(this, true);

            // place classes at random position
            // 获取所有需要上的课
            List<CourseClass> classes = Configuration.GetInstance().CourseClasses;

            // 将每一个需要上的课随机地排到一间教室的一个或多个时段里
            foreach (CourseClass c in classes)
            {
                // determine random position of class
                // 随机确定课堂的位置

                // 教室总数
                int numberOfRooms = Configuration.GetInstance().Classrooms.Count;

                // 当前课堂的时长
                int dur = c.Duration;

                // 随机确定当前是第几个上课日
                int day = this.Rand() % DAYS_NUM;

                // 随机确定使用第几间教室
                int room = this.Rand() % numberOfRooms;

                // 随机选择一个上课时间
                int time = this.Rand() % (DAY_HOURS + 1 - dur);

                // 计算出本课堂的时间槽需要，即在总课堂列表中的位置
                // 总课程列表：上课日数×每日课时数×教室数
                int pos = day * numberOfRooms * DAY_HOURS + room * DAY_HOURS + time;

                // fill time-space slots, for each hour of class
                // 在当前教室的指定时段上排课，有的课堂需要占用多个时段
                for (int i = dur - 1; i >= 0; i--)
                {
                    newChromosome.Slots[pos + i].Add(c);
                }

                // insert in class table of chromosome
                // 记录课程的起始时段
                newChromosome.Classes[c] = pos;
            }

            //计算新染色体的适应性
            newChromosome.CalculateFitness();

            // return smart pointer
            return newChromosome;
        }



        #endregion

        #region Properties

        #endregion

        #region Members

        // Performes crossover operation using to chromosomes and returns pointer to offspring
        // 执行均匀杂交
        public Schedule Crossover(Schedule parent2)
        {
            // check probability of crossover operation
            // 看看这次是否需要杂交，如果不需要
            if (this.Rand() % 100 > this.crossoverProbability)
            {
                // no crossover, just copy first parent
                // 不进行杂交，而是直接复制和返回第一个父本
                return new Schedule(this, false);
            }

            // new chromosome object, copy chromosome setup
            // 拷贝第一个父本创建新染色体。只拷贝配置信息，不拷贝内容。
            Schedule newSchedule = new Schedule(this, true);

            // 交换点数组
            bool[] cp = new bool[this.Classes.Count];

            // determine crossover point (randomly)
            // 随机选择指定数量的交换点
            for (int i = this.numberOfCrossoverPoints; i > 0; i--)
            {
                while (true)
                {
                    int pos = Rand() % this.Classes.Count;
                    if (!cp[pos])
                    {
                        cp[pos] = true;
                        break;
                    }
                }
            }

            // make new code by combining parent codes
            // 确定第一个基因从那个父亲染色体中获取，双亲各有一半的概率被选中。
            bool first = Rand() % 2 == 0;

            for (int i = 0; i < this.Classes.Count; i++)
            {
                // 选择父本
                Schedule parent = first ? this : parent2;

                CourseClass cc = parent.Classes.ElementAt(i).Key;
                int p = parent.Classes.ElementAt(i).Value;

                // insert class from first parent into new chromosome's calss table
                newSchedule.Classes[cc] = p;

                // all time-space slots of class are copied
                for (int j = cc.Duration - 1; j >= 0; j--)
                {
                    newSchedule.Slots[p + j].Add(cc);
                }

                // crossover point
                // 交叉点
                if (cp[i])
                {
                    // change soruce chromosome
                    // 切换父本
                    first = !first;
                }
            }

            // 计算新染色体的适应度
            newSchedule.CalculateFitness();

            // return smart pointer to offspring
            return newSchedule;
        }

        // Performs mutation on chromosome
        public void Mutation()
        {
            // check probability of mutation operation
            if (Rand() % 100 > mutationProbability)
            {
                return;
            }

            // number of classes
            int numberOfClasses = this.Classes.Count;

            // number of time-space slots
            int numberOfSlots = this.Slots.Count;

            // move selected number of classes at random position
            for (int i = mutationSize; i > 0; i--)
            {
                // select ranom chromosome for movement
                int mpos = Rand() % numberOfClasses;

                // current time-space slot used by class
                int pos1 = this.Classes.ElementAt(mpos).Value;

                CourseClass cc1 = this.Classes.ElementAt(mpos).Key;

                // determine position of class randomly
                int nr = Configuration.GetInstance().Classrooms.Count;
                int dur = cc1.Duration;
                int day = Rand() % DAYS_NUM;
                int room = Rand() % nr;
                int time = Rand() % (DAY_HOURS + 1 - dur);
                int pos2 = day * nr * DAY_HOURS + room * DAY_HOURS + time;

                // move all time-space slots
                for (int j = dur - 1; j >= 0; j--)
                {
                    // remove class hour from current time-space slot
                    List<CourseClass> cl = this.Slots[pos1 + i];
                    foreach (CourseClass c in cl)
                    {
                        if (c == cc1)
                        {
                            cl.Remove(c);
                            break;
                        }
                    }

                    // move class hour to new time-space slot
                    this.Slots.ElementAt(pos2 + i).Add(cc1);
                }

                // change entry of class table to point to new time-space slots
                this.Classes[cc1] = pos2;
            }

            CalculateFitness();
        }

        // Calculates fitness value of chromosome
        // 计算染色体的适应度
        public void CalculateFitness()
        {
            // chromosome's score
            // 得分
            int score = 0;

            // 课堂总数
            int numberOfRooms = Configuration.GetInstance().Classrooms.Count;

            // 每日可用课堂数（每日课时数*教室数）
            int daySize = DAY_HOURS * numberOfRooms;

            // 标准的序号，每个课堂有5个评价标准，所有课堂的所有指标被顺序放在一个数组中
            int ci = 0;

            // check criterias and calculate scores for each class in schedule
            // 计算每一节课的得分
            for (int idx = 0; idx < this.Classes.Count; idx++, ci += 5)
            {
                var pair = this.Classes.ElementAt(idx);
                CourseClass cc = pair.Key;    // 课堂
                int p = pair.Value;           // 每周的第几次课

                // coordinate of time-space slot
                int day = p / daySize;        // 周几
                int time = p % daySize;       // 每日的第几次课
                int room = time / DAY_HOURS;  // 教室数量
                time = time % DAY_HOURS;      // 每日的第几时段的课

                // check for room overlapping of classes
                // 检查教室重合
                bool ro = false;
                for (int i = cc.Duration - 1; i >= 0; i--)
                {
                    if (this.Slots[p + i].Count > 1)
                    {
                        ro = true;
                        break;
                    }
                }

                // on room overlaping
                // 如果没有发生教室重合，得1分
                if (!ro)
                {
                    score++;
                }

                this.Criteria[ci + 0] = !ro;


                Classroom r = Configuration.GetInstance().GetRoomById(room);

                // does current room have enough seats
                // 当前教室的座位是否足够，如果足够加1分
                this.Criteria[ci + 1] = r.NumberOfSeats >= cc.NumberOfStuduents;
                if (this.Criteria[ci + 1])
                {
                    score++;
                }

                // does current room have computers if they are required
                // 当前教室的设施（电脑）是否满足课堂需要，满足加1分
                this.Criteria[ci + 2] = !cc.RequireComputers || (cc.RequireComputers && r.HasComputers);
                if (this.Criteria[ci + 2])
                {
                    score++;
                }

                bool po = false, go = false;

                // check overlapping of classes for professors and student groups
                // 检查教师和学生是否同时上多于一个课堂
                for (int num = numberOfRooms, t = day * daySize + time; num > 0; num--, t += DAY_HOURS)
                {
                    // for each hour of class
                    for (int i = cc.Duration - 1; i >= 0; i--)
                    {
                        // check for overlapping with other classes at same time
                        List<CourseClass> cl = this.Slots[t + i];
                        foreach (CourseClass c in cl)
                        {
                            if (cc != c)
                            {
                                // professor overlaps?
                                if (!po && cc.ProfessorOverlaps(c))
                                {
                                    po = true;
                                }

                                // student group overlaps?
                                if (!go && cc.GroupsOverlap(c))
                                {
                                    go = true;
                                }

                                // both type of overlapping? no need to check more
                                if (po && go)
                                {
                                    goto total_overlap;
                                }
                            }
                        }
                    }
                }

                total_overlap:

                // professors have no overlaping classes?
                if (!po)
                    score++;
                this.Criteria[ci + 3] = !po;

                // student groups has no overlaping classes?
                if (!go)
                    score++;
                this.Criteria[ci + 4] = !go;
            }

            // calculate fitess value based on score
            this.Fitness = (float)score / (Configuration.GetInstance().CourseClasses.Count * DAYS_NUM);
        }

        #endregion
    }
}
