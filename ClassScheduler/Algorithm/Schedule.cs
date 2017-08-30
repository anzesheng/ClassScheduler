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
        /// </summary>
        //public List<List<CourseClass>> Slots { get; set; }

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
        /// 排课总表
        /// </summary>
        public Dictionary<CourseClass, int> Classes { get; set; }

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
            this.Fitness = 0;

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
                Array.Resize(ref this.slots, DAYS_NUM * DAY_HOURS * Configuration.GetInstance().Classrooms.Count);

                // reserve space for flags of class requirements
                Array.Resize(ref this.criteria, Configuration.GetInstance().Classrooms.Count * 5);
            }
            else
            {
                // copy code
                this.Slots = c.Slots;
                this.Classes = c.Classes;

                // copy flags of class requirements
                this.Criteria = c.Criteria;

                // copy fitness
                this.Fitness = c.Fitness;
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
        // 创建新染色体，拷贝设置但是随即排列课程
        public Schedule MakeNewFromPrototype()
        {
            // number of time-space slots
            // 时间曹数量
            int size = this.Slots.Length;

            // make new chromosome, copy chromosome setup
            // 创建新染色体，拷贝当前染色体的设置
            Schedule newChromosome = new Schedule(this, true);

            // place classes at random position
            // 随机排列课程
            List<CourseClass> classes = Configuration.GetInstance().CourseClasses;
            foreach (CourseClass c in classes)
            {
                // determine random position of class
                // 确定随机位置
                int numberOfRooms = Configuration.GetInstance().Classrooms.Count;
                int dur = c.Duration;
                int day = this.Rand() % DAYS_NUM;
                int room = this.Rand() % numberOfRooms;
                int time = this.Rand() % (DAY_HOURS + 1 - dur);
                int pos = day * numberOfRooms * DAY_HOURS + room * DAY_HOURS + time;

                // fill time-space slots, for each hour of class
                // 在时段上排课
                for (int i = dur - 1; i >= 0; i--)
                {
                    newChromosome.Slots[pos + i].Add(c);
                }

                // insert in class table of chromosome
                // 记录课程的时段
                newChromosome.Classes[c] = pos;
            }

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
            if (this.Rand() % 100 > this.crossoverProbability)
            {
                // no crossover, just copy first parent
                // 不杂交，直接复制和返回第一个父本
                return new Schedule(this, false);
            }

            // new chromosome object, copy chromosome setup
            // 新染色体，拷贝第一个父本，只要配置信息，不要排课信息
            Schedule newSchedule = new Schedule(this, true);

            // 交换点
            bool[] cp = new bool[this.Classes.Count];

            // determine crossover point (randomly)
            // 首先随机选择染色体上的交换点
            for (int i = this.numberOfCrossoverPoints; i > 0; i--)
            {
                while (true)
                {
                    int p = Rand() % this.Classes.Count;
                    if (!cp[p])
                    {
                        cp[p] = true;
                        break;
                    }
                }
            }

            // make new code by combining parent codes
            // 一半的概率
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
                    // 切换源染色体
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
