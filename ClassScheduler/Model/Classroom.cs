using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassScheduler.Model
{
    public class Classroom
    {
        #region Fields

        // ID counter used to assign IDs automatically
        private static int nextRoomId = 0;

        #endregion

        #region Constructors and destructors

        // Initializes room data and assign ID to room
        public Classroom(string name, bool hasComputers, int numberOfSeats)
        {
            this.Id = nextRoomId++;
            this.Name = name;
            this.HasComputers = HasComputers;
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

        #region Static methods

        // Restarts ID assigments
        public static void RestartIDs()
        {
            nextRoomId = 0;
        }

        #endregion
    }
}
