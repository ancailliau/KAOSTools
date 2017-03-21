using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IEntityRepository
	{
		void Add(EntityAttribute goal);
		void Add(Entity goal);
		void Add(GivenType goal);
		void Add(Link goal);
		void Add(Relation goal);

		bool EntityAttributeExists(string identifier);
		bool EntityExists(string identifier);
		bool GivenTypeExists(string identifier);
		bool LinkExists(string identifier);
		bool RelationExists(string identifier);
	}
    
}
