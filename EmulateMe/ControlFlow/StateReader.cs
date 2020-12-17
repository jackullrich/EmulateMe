using System;
using System.Collections.Generic;
using System.Linq;

namespace EmulateMe.ControlFlow
{
    public class StateReader
    {
        private List<uint> states;

        private uint? current;
        private uint? previous;
        private uint? next;

        public StateReader(List<uint> States)
        {
            if (States.Distinct().Count() != States.Count)
                throw new Exception("States must be distinct.");

            this.states = States;
            this.current = states.First();
            this.next = states[states.IndexOf(current.Value) + 1];
            this.previous = null;
        }

        public uint First => states.First();
        public uint Last => states.Last();

        public uint CurrentValue => current != null ? current.Value : throw new NullReferenceException();
        public uint NextValue => next != null ? next.Value : throw new NullReferenceException();
        public uint PreviousValue => previous != null ? previous.Value : throw new NullReferenceException();

        public bool HasCurrent => current != null;
        public bool HasNext => next != null;
        public bool HasPrevious => previous != null;

        public bool IsAtEnd => states.IndexOf(current.Value) == states.Count - 1 ? true : false;
        public bool IsAtBeginning => states.IndexOf(current.Value) == 0 ? true : false;

        public void NextState()
        {
            if (!current.HasValue)
            {
                throw new Exception("No current node.");
            }

            int index = states.IndexOf(current.Value);
            index++;

            // Current not found
            if (index == 0)
            {
                throw new Exception("Current value not found.");
            }

            // We are at the end of the list, don't update
            if (index == states.Count)
            {
                throw new Exception("No next state.");
            }

            uint updatedCurrent = states[index];
            current = updatedCurrent;

            // Assign previous
            if (index - 1 > -1)
            {
                previous = states[index - 1];
            }
            else
            {
                previous = null;
            }

            // Assign next
            if (index + 1 < states.Count)
            {
                next = states[index + 1];
            }
            else
            {
                next = null;
            }
        }

        public void PreviousState()
        {
            if (!current.HasValue)
            {
                throw new Exception("No current node.");
            }

            int index = states.IndexOf(current.Value);

            // Current not found
            if (index == -1)
            {
                throw new Exception("Current value not found.");
            }

            // We are at the beginning of the list, don't update
            if (index == 0)
            {
                throw new Exception("No previous state.");
            }

            index--;
            uint updatedCurrent = states[index];
            current = updatedCurrent;

            // Assign previous
            if (index - 1 > -1)
            {
                previous = states[index - 1];
            }
            else
            {
                previous = null;
            }

            // Assign next
            if (index + 1 < states.Count)
            {
                next = states[index + 1];
            }
            else
            {
                next = null;
            }
        }
    }
}
