using System;
using System.Collections.Generic;
using System.Text;

namespace qRPC.Test
{
    public interface IDoStuff
    {
        string HiWorld(string p);

        double Addition(double x, double y);

        public string WhatIsMyNumber(string name, int number);

        public void NotImplemented();

        public string TryAGuid(Guid guid);

        public long TryADatetime(DateTime date);

        public Guid GetAGuid();

        public DateTime GetNow();
    }
}
