using System;
namespace UCLouvain.KAOSTools.Utils.Monitor
{
	public interface ICommand
	{
		void Execute(string command);
	}
}
