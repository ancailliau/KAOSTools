using System;
using System.Linq;
using KAOSFormalTools.Domain.Tests;
using LtlSharp;

namespace Test
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            Console.WriteLine ();

            var pg = "[] (start -> X X delivered)";
            var g1 = "[] (start -> X try)";
            var g2 = "[] (try -> X delivered)";

            var sm = TestHelpers.BuildSimpleCommunicationProtocol ();

        }
    }
}
