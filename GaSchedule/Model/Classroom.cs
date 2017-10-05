using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaSchedule.Model
{
    public class Classroom
    {
        #region Constructors and destructors

        public Classroom()
        {
            this.Id = -1;
            this.Name = string.Empty;
            this.HasComputers = false;
            this.NumberOfSeats = 0;
        }

        // Initializes room data and assign ID to room
        public Classroom(int id, string name, bool hasComputers, int numberOfSeats)
        {
            this.Id = id;
            this.Name = name;
            this.HasComputers = hasComputers;
            this.NumberOfSeats = numberOfSeats;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the class room.
        /// </summary>
        public int Id { get; set; }

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

        #endregion
    }
}
