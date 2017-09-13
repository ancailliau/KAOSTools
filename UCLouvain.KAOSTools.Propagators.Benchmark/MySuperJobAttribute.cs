using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace UCLouvain.KAOSTools.Propagators.Benchmark
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
	public class MySuperJobAttribute : Attribute, IConfigSource
	{
	    public MySuperJobAttribute()
	    {
	        var job = new Job("QuickJob", RunMode.Default);
	        job.Env.Platform = Platform.X64;
			job.Env.Runtime = Runtime.Mono;
			job.Env.Jit = Jit.Llvm;

			job.Run.LaunchCount = 1;
			job.Run.WarmupCount = 3;
			job.Run.TargetCount = 5;
			job.Run.InvocationCount = 4;
			job.Run.UnrollFactor = 4;
			
	        Config = ManualConfig.CreateEmpty().With(job);
	    }
	
	    public IConfig Config { get; }
	}
}
