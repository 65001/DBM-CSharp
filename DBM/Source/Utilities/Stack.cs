using System;
using System.Collections.Generic;

namespace DBM
{
    public static class Stack
    {
        public struct StackEntry
        {
            public string Trace;
            public DateTime StartTime;
            public DateTime ExitTime;
            public TimeSpan Duration;
        }

        static List<StackEntry> _StackEntries = new List<StackEntry>();

        public static IReadOnlyList<StackEntry> StackEntries
        {
            get { return _StackEntries.AsReadOnly(); }
        }

        public static int Add(string Entry)
        {
            var SE = new StackEntry
            {
                Trace = Entry,
                StartTime = DateTime.UtcNow,
                ExitTime = DateTime.MaxValue,
                Duration = TimeSpan.MinValue
            };
            _StackEntries.Add(SE);
            return _StackEntries.Count - 1;
        }

        public static void Exit(int Index)
        {
            var SE = _StackEntries[Index];
            SE.ExitTime = DateTime.UtcNow;
            SE.Duration = SE.ExitTime - SE.StartTime;
            _StackEntries[Index] = SE;
        }

        public static void Print()
        {
            for (int i = 0; i < StackEntries.Count; i++)
            {
                Console.WriteLine(StackEntries[i].Trace);
            }
        }
    }
}
