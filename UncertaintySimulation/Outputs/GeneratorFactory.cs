using System;

namespace UncertaintySimulation.Outputs
{
    public class GeneratorFactory
    {
        public static OutputGenerator Build (string identifier, GeneratorOptions options)
        {
            if (identifier == "shortReport") {
                return new ShortTextReport (options.RootGoal);
            }

            if (identifier == "SUpng") {
                if (string.IsNullOrEmpty (options.Filename)) {
                    throw new ArgumentException ("Filename shall be specified when exporting image");
                }
                return new SatisfactionUncertaintyPNG (options.RootGoal, options.Filename);
            }

            throw new NotImplementedException ();
        }
    }
}

