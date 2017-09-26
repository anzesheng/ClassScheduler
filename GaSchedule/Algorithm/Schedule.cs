using GaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GaSchedule.Algorithm
{
    /// <summary>
    /// 本遗传算法中的染色体（基因串）
    /// </summary>
    public class Schedule
    {
        //// Number of working hours per day
        //public const int ClassNumberPerDay = 12;

        //// Number of days in week
        //public const int WorkingDaysNumber = 5;

        private Random random = new Random();

        private int Rand()
        {
            return this.random.Next();
        }


        #region Fields

        private readonly Configuration configuration;



        // Fitness value of chromosome
        // 染色体的适应度
        public float Fitness { get; set; }

        /// <summary>
        /// Flags of class requiroments satisfaction
        /// 总共的标准满足列表：课堂总数 × 评价标准个数
        /// 每节课有5个标准用于计算得分：
        /// (1) 教室不重合得1分
        /// (2) 教室座位够用得1分
        /// (3) 教室的设施能够满足课堂要求得1分
        /// (4) 教师同一时间上一堂课得1分
        /// (5) 学生组同一时间上一堂课得1分
        /// </summary>
        private bool[] criteria;

        /// <summary>
        /// Time-space slots, one entry represent one hour in one classroom
        /// 时间槽列表。一个时间槽表示一间教室的一个小时。
        /// (1) 时间槽的排列顺序如下：
        ///   [
        ///     {工作日1:[{时段1:[教室1，教室2,...教室n]}, {时段2:[教室1，教室2,...教室n]}, ... {时段12:[教室1，教室2,...教室n]}]},
        ///     {工作日2:[{时段1:[教室1，教室2,...教室n]}, {时段2:[教室1，教室2,...教室n]}, ... {时段12:[教室1，教室2,...教室n]}]},
        ///     ...,
        ///     {工作日5:[{时段1:[教室1，教室2,...教室n]}, {时段2:[教室1，教室2,...教室n]}, ... {时段12:[教室1，教室2,...教室n]}]}
        ///   ]
        /// (2) 在这里一个时间槽里允许可以放置多个课堂，因为计算过程中会出现冲突的情况。
        /// (3) 该列表的长度 = 上课日数×每日课时数×教室数
        /// (4) 某一节课的时间槽的序号 = (在第几个工作日-1)×每日的课时数×教室总数 + (在第几时段-1)×教室总数 + 在第几间教室
        /// </summary>
        private List<Slot> slots;

        /// <summary>
        /// Class table for chromosome
        /// Used to determine first time-space slot used by class
        /// 排课结果，key是课堂，value是课堂的时间槽序号。
        /// </summary>
        public Dictionary<CourseClass, int> ScheduledClasses { get; set; } = new Dictionary<CourseClass, int>();

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
        public Schedule(Configuration configuration)
        {
            this.configuration = configuration;
            this.Fitness = 0;

            // 准备好时间槽容器，数组初始化会调用Slot的默认构造函数
            this.InitialSlots();

            // TODO: 按需求冻结一些槽位

            // reserve space for flags of class requirements
            // 每堂课有5个衡量指标
            int criteriaNum = this.configuration.CourseClasses.Count * 5;
            this.criteria = new bool[criteriaNum];
        }

        /// <summary>
        /// 准备好时间槽容器。一个教学周期（一般是一周）内能够安排的所有课程。
        /// </summary>
        public void InitialSlots()
        {
            this.slots = new List<Slot>();

            for (int d = 0; d < this.configuration.WorkingDaysNumber; d++)
            {
                for (int r = 0; r < this.configuration.Classrooms.Count; r++)
                {
                    for (int c = 0; c < this.configuration.ClassNumberPerDay; c++)
                    {
                        this.slots.Add(new Slot(d, r, c));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// Copy constructor
        /// 拷贝构造
        /// </summary>
        /// <param name="schedule">被拷贝者</param>
        /// <param name="setupOnly">是否只拷贝size。True时只拷贝size，不拷贝内容。</param>
        public Schedule(Schedule schedule, bool setupOnly)
        {
            this.configuration = schedule.configuration;

            // reserve space for flags of class requirements
            // 准备好标准数组（因为每个课堂有5个评价标准，所以乘以5）
            int totalCriteriaItems = this.configuration.CourseClasses.Count * 5;
            this.criteria = new bool[totalCriteriaItems];


            // 拷贝排课内容
            if (setupOnly)
            {
                this.InitialSlots();

                // TODO: 按需求冻结一些槽位
            }
            else
            {
                // copy code
                // 拷贝槽位数据
                this.slots = new List<Slot>();
                foreach (var s in schedule.slots)
                {
                    this.slots.Add(s);
                }

                // 拷贝课堂
                this.ScheduledClasses = new Dictionary<CourseClass, int>();
                foreach (var pair in schedule.ScheduledClasses)
                {
                    this.ScheduledClasses[pair.Key] = pair.Value;
                }

                // copy flags of class requirements
                // 拷贝标准数组
                for (int i = 0; i < schedule.criteria.Length; i++)
                {
                    this.criteria[i] = schedule.criteria[i];
                }

                // copy fitness
                // 拷贝适应性
                this.Fitness = schedule.Fitness;
            }
        }

        // Makes copy ot chromosome
        public Schedule MakeCopy(bool setupOnly)
        {
            // make object by calling copy constructor and return smart pointer to new object
            return new Schedule(this, setupOnly);
        }

        // Makes new chromosome with same setup but with randomly chosen code
        // 创建新染色体，拷贝设置但是随机排列课程
        public Schedule MakeNewFromPrototype()
        {
            // make new chromosome, copy chromosome setup
            // 创建新染色体，只拷贝当前染色体的size，不拷贝内容
            Schedule newSchedule = new Schedule(this, true);

            // place classes at random position
            // 获取所有需要上的课
            List<CourseClass> classes = this.configuration.CourseClasses;

            // 将每一个需要上的课随机地排到一间教室的一个或多个时段里
            foreach (CourseClass c in classes)
            {
                // determine random position of class
                // 随机确定课堂的位置

                // 教室总数
                //int numberOfRooms = this.configuration.Classrooms.Count;

                // 当前课堂的时长
                int dur = c.Duration;

                // 随机确定在第几个工作日上课
                int day = this.Rand() % this.configuration.WorkingDaysNumber;

                // 随机确定使用第几间教室
                int room = this.Rand() % this.configuration.Classrooms.Count;

                // 随机选择一个上课时段
                int time = this.Rand() % (this.configuration.ClassNumberPerDay + 1 - dur);

                // 计算出本课堂的时间槽序号，即在总课堂列表中的位置
                // 总课程列表：上课日数×每日课时数×教室数
                int pos = day * this.configuration.Classrooms.Count * this.configuration.ClassNumberPerDay
                    + room * this.configuration.ClassNumberPerDay
                    + time;

                // fill time-space slots, for each hour of class
                // 在当前教室的指定时段上排课，有的课堂需要占用多个时段
                for (int i = dur - 1; i >= 0; i--)
                {
                    newSchedule.slots[pos + i].Classes.Add(c.Id);
                }

                // insert in class table of chromosome
                // 记录课程的起始时段
                newSchedule.ScheduledClasses[c] = pos;
            }

            //计算新染色体的适应性
            newSchedule.CalculateFitness();

            // return smart pointer
            return newSchedule;
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
            if (this.Rand() % 100 > this.configuration.Parameters.CrossoverProbability)
            {
                // no crossover, just copy first parent
                // 不进行杂交，而是直接复制和返回第一个父本
                return new Schedule(this, false);
            }

            // new chromosome object, copy chromosome setup
            // 拷贝第一个父本创建新染色体。只拷贝配置信息，不拷贝内容。
            Schedule newSchedule = new Schedule(this, true);

            // 交换点数组
            bool[] cp = new bool[this.ScheduledClasses.Count];

            // determine crossover point (randomly)
            // 随机选择指定数量的交换点
            for (int i = this.configuration.Parameters.NumberOfCrossoverPoints; i > 0; i--)
            {
                while (true)
                {
                    int pos = Rand() % this.ScheduledClasses.Count;
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

            for (int i = 0; i < this.ScheduledClasses.Count; i++)
            {
                // 选择父本
                Schedule parent = first ? this : parent2;

                CourseClass cc = parent.ScheduledClasses.ElementAt(i).Key;
                int p = parent.ScheduledClasses.ElementAt(i).Value;

                // insert class from first parent into new chromosome's calss table
                newSchedule.ScheduledClasses[cc] = p;

                // all time-space slots of class are copied
                for (int j = cc.Duration - 1; j >= 0; j--)
                {
                    newSchedule.slots[p + j].Classes.Add(cc.Id);
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
        // 在当前染色体上执行变异
        public void Mutation()
        {
            // check probability of mutation operation
            // 检查是否需要变异
            if (Rand() % 100 > this.configuration.Parameters.MutationProbability)
            {
                return;
            }

            // number of classes
            // 已经排好的课堂数
            int numberOfClasses = this.ScheduledClasses.Count;

            // move selected number of classes at random position
            // 随机移动指定数量课堂的时间槽
            for (int i = this.configuration.Parameters.MutationSize; i > 0; i--)
            {
                // select ranom chromosome for movement
                // 随机选择一个染色体
                int mpos = Rand() % numberOfClasses;

                // current time-space slot used by class
                // 获得改染色体的时间槽
                int pos1 = this.ScheduledClasses.ElementAt(mpos).Value;

                // 课堂
                CourseClass cc1 = this.ScheduledClasses.ElementAt(mpos).Key;

                // determine position of class randomly
                // 随机决定课堂新的时间槽

                // 课堂节数
                int dur = cc1.Duration;

                // 第几个工作日
                int day = Rand() % this.configuration.WorkingDaysNumber;

                // 第几个教室
                int room = Rand() % this.configuration.Classrooms.Count;

                // 第几节课开始上
                int time = Rand() % (this.configuration.ClassNumberPerDay + 1 - dur);

                // 得到新的时间槽
                int pos2 = day * this.configuration.Classrooms.Count * this.configuration.ClassNumberPerDay + room * this.configuration.ClassNumberPerDay + time;

                // move all time-space slots
                // 移动该课堂的每一节课
                for (int j = dur - 1; j >= 0; j--)
                {
                    // remove class hour from current time-space slot
                    // 将课堂从现在的时间槽里删除
                    this.slots[pos1 + j].RemoveClass(cc1.Id);

                    // move class hour to new time-space slot
                    // 放入新的时间槽
                    this.slots[pos2 + j].AddClass(cc1.Id);
                }

                // change entry of class table to point to new time-space slots
                // 保存课堂的新时间槽
                this.ScheduledClasses[cc1] = pos2;
            }

            // 重新计算适应度
            CalculateFitness();
        }

        // Calculates fitness value of chromosome
        // 计算染色体的适应度
        public void CalculateFitness()
        {
            // chromosome's score
            // 得分
            int score = 0;

            // 教室数
            int numberOfRooms = this.configuration.Classrooms.Count;

            // 每日可用课堂数（每日课时数*教室数）
            int daySize = this.configuration.ClassNumberPerDay * numberOfRooms;

            // 标准的序号，每个课堂有5个评价标准，所有课堂的所有指标被顺序放在一个数组中
            int ci = 0;

            // check criterias and calculate scores for each class in schedule
            // 计算每一节课的得分，每节课有5个得分
            // 教室不重合得1分
            // 教室座位够用得1分
            // 教室的设施能够满足课堂要求得1分
            // 教师同一时间上一堂课得1分
            // 学生组同一时间上一堂课得1分
            for (int idx = 0; idx < this.ScheduledClasses.Count; idx++, ci += 5)
            {
                var pair = this.ScheduledClasses.ElementAt(idx);
                CourseClass cc = pair.Key;    // 课堂
                int p = pair.Value;           // 课堂的大排行序号

                // coordinate of time-space slot
                int day = p / daySize;        // 第几个工作日
                int time = p % daySize;       // 当日的课次编号（按当日所有课堂排序）
                int room = time / this.configuration.ClassNumberPerDay;  // 第几间教室
                time = time % this.configuration.ClassNumberPerDay;      // 当日的第几时段

                // check for room overlapping of classes
                // 检查教室重合
                bool ro = false;
                for (int i = cc.Duration - 1; i >= 0; i--)
                {
                    if (this.slots[p + i].Classes.Count > 1)
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

                this.criteria[ci + 0] = !ro;


                Classroom r = this.configuration.Classrooms[room];

                // does current room have enough seats
                // 当前教室的座位是否足够，如果足够加1分
                this.criteria[ci + 1] = r.NumberOfSeats >= cc.NumberOfStuduents;
                if (this.criteria[ci + 1])
                {
                    score++;
                }

                // does current room have computers if they are required
                // 当前教室的设施（电脑）是否满足课堂需要，满足加1分
                this.criteria[ci + 2] = !cc.RequireComputers || (cc.RequireComputers && r.HasComputers);
                if (this.criteria[ci + 2])
                {
                    score++;
                }

                bool po = false, go = false;


                // 当日该时段第一节课的大排行序号
                int firstClassNo = day * daySize + time;

                // check overlapping of classes for professors and student groups
                // 检查教师和学生是否同时上多于一个课堂
                for (int num = numberOfRooms, t = firstClassNo; num > 0; num--, t += this.configuration.ClassNumberPerDay)
                {
                    // for each hour of class
                    // 遍历当前课堂的每一个时段
                    for (int i = cc.Duration - 1; i >= 0; i--)
                    {
                        // check for overlapping with other classes at same time
                        // 检查老师和班级是否与其它同一时段的课程有重合
                        Slot slot = this.slots[t + i];
                        foreach (int classId in slot.Classes)
                        {
                            if (cc.Id != classId)
                            {
                                var anotherCalss = this.configuration.CourseClasses[classId];

                                // teacher overlaps?
                                if (!po && cc.TeacherOverlaps(anotherCalss))
                                {
                                    po = true;
                                }

                                // student group overlaps?
                                if (!go && cc.StudentsGroupsOverlap(anotherCalss))
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
                // 如果教师没有冲突，加1分
                if (!po)
                {
                    score++;
                }
                this.criteria[ci + 3] = !po;

                // student groups has no overlaping classes?
                // 如果学生组上课时间没有冲突，加1分
                if (!go)
                {
                    score++;
                }
                this.criteria[ci + 4] = !go;
            }

            // calculate fitess value based on score
            this.Fitness = (float)score / (this.configuration.CourseClasses.Count * 5);
        }

        #endregion
    }
}
