using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
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

		EntityAttribute GetEntityAttribute(string identifier);
		Entity GetEntity(string identifier);
		GivenType GetGivenType(string identifier);
		Link GetLink(string identifier);
		Relation GetRelation(string identifier);

		EntityAttribute GetEntityAttribute(Predicate<EntityAttribute> predicate);
		Entity GetEntity(Predicate<Entity> predicate);
		GivenType GetGivenType(Predicate<GivenType> predicate);
		Link GetLink(Predicate<Link> predicate);
		Relation GetRelation(Predicate<Relation> predicate);

		IEnumerable<EntityAttribute> GetEntityAttributes();
		IEnumerable<GivenType> GetGivenTypes();
		IEnumerable<Link> GetLinks();
		IEnumerable<Relation> GetRelations();
		IEnumerable<Entity> GetEntities();

		IEnumerable<EntityAttribute> GetEntityAttributes(Predicate<EntityAttribute> predicate);
		IEnumerable<GivenType> GetGivenTypes(Predicate<GivenType> predicate);
		IEnumerable<Link> GetLinks(Predicate<Link> predicate);
		IEnumerable<Relation> GetRelations(Predicate<Relation> predicate);
		IEnumerable<Entity> GetEntities(Predicate<Entity> predicate);
	}
    
}
