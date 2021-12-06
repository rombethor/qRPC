using System;
using System.Collections.Generic;
using System.Text;

namespace qRPC.Test
{
    public class DoStuff : IDoStuff
    {
        public double Addition(double x, double y)
        {
            return x + y;
        }

        public string HiWorld(string p)
        {
            return $"Hi {p}";
        }

        public string WhatIsMyNumber(string name, int number)
        {
            return $"Hi {name}!  Your number is {number}";
        }

        public void NotImplemented()
        {
            //throw new NotImplementedException();
        }

        public string TryAGuid(Guid guid)
        {
            return $"Your guid: {guid}";
        }

        public long TryADatetime(DateTime date)
        {
            return date.Ticks;
        }

        public Guid GetAGuid()
        {
            return Guid.NewGuid();
        }

    }
}
