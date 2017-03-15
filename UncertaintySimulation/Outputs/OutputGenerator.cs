using System;
using System.IO;

namespace UncertaintySimulation
{
    public interface OutputGenerator
    {
        void Output (UncertaintyComputation c, TextWriter stream);
    }
}

