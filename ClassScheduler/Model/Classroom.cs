using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Model
{
    public class Classroom
    {
        /// <summary>
        /// Gets or sets the identifier of the class room.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the class room.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of seats.
        /// </summary>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this room has computers.
        /// </summary>
        public bool HasComputers { get; set; }
    }
}
