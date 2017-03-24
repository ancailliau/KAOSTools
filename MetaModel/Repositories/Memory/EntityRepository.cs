using System;
using System.Collections.Generic;
using KAOSTools.Core;
using System.Linq;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class EntityRepository : IEntityRepository 
    {
		IDictionary<string, Link> Links;
		IDictionary<string, Relation> Relations;
		IDictionary<string, GivenType> GivenTypes;
		IDictionary<string, Entity> Entities;
		IDictionary<string, EntityAttribute> EntityAttributes;

        public EntityRepository()
        {
			Links = new Dictionary<string, Link>();
			Relations = new Dictionary<string, Relation>();
			GivenTypes = new Dictionary<string, GivenType>();
			Entities = new Dictionary<string, Entity>();
			EntityAttributes = new Dictionary<string, EntityAttribute>();
        }

        public void Add(Link link)
		{
			if (Links.ContainsKey(link.Identifier))
			{
				throw new ArgumentException(string.Format("Link identifier already exist: {0}", link.Identifier));
			}

			Links.Add(link.Identifier, link);
        }

        public void Add(Relation relation)
		{
			if (Relations.ContainsKey(relation.Identifier))
			{
				throw new ArgumentException(string.Format("Relation identifier already exist: {0}", relation.Identifier));
			}

			Relations.Add(relation.Identifier, relation);
        }

        public void Add(GivenType givenType)
		{
			if (GivenTypes.ContainsKey(givenType.Identifier))
			{
				throw new ArgumentException(string.Format("Given type identifier already exist: {0}", givenType.Identifier));
			}

			GivenTypes.Add(givenType.Identifier, givenType);
        }

        public void Add(Entity entity)
		{
			if (Entities.ContainsKey(entity.Identifier))
			{
				throw new ArgumentException(string.Format("Entity identifier already exist: {0}", entity.Identifier));
			}

			Entities.Add(entity.Identifier, entity);
        }

		public void Add(EntityAttribute link)
		{
			if (EntityAttributes.ContainsKey(link.Identifier))
			{
				throw new ArgumentException(string.Format("Entity attribute identifier already exist: {0}", link.Identifier));
			}

			EntityAttributes.Add(link.Identifier, link);
        }

        public bool EntityAttributeExists(string identifier)
        {
            return EntityAttributes.ContainsKey(identifier);
        }

        public bool EntityExists(string identifier)
		{
            return Entities.ContainsKey(identifier);
        }

        public bool GivenTypeExists(string identifier)
		{
            return GivenTypes.ContainsKey(identifier);
        }

        public bool LinkExists(string identifier)
		{
            return Links.ContainsKey(identifier);
        }

        public bool RelationExists(string identifier)
		{
            return Relations.ContainsKey(identifier);
        }

        public EntityAttribute GetEntityAttribute(string identifier)
        {
            return EntityAttributes.ContainsKey(identifier) ? EntityAttributes[identifier] : null;
        }

        public Entity GetEntity(string identifier)
		{
			return Entities.ContainsKey(identifier) ? Entities[identifier] : null;
        }

        public GivenType GetGivenType(string identifier)
		{
			return GivenTypes.ContainsKey(identifier) ? GivenTypes[identifier] : null;
        }

        public Link GetLink(string identifier)
		{
			return Links.ContainsKey(identifier) ? Links[identifier] : null;
        }

        public Relation GetRelation(string identifier)
		{
			return Relations.ContainsKey(identifier) ? Relations[identifier] : null;
        }

        public EntityAttribute GetEntityAttribute(Predicate<EntityAttribute> predicate)
        {
            return EntityAttributes.Values.SingleOrDefault(x => predicate(x));
        }

        public Entity GetEntity(Predicate<Entity> predicate)
		{
            return Entities.Values.SingleOrDefault(x => predicate(x));
        }

        public GivenType GetGivenType(Predicate<GivenType> predicate)
		{
            return GivenTypes.Values.SingleOrDefault(x => predicate(x));
        }

        public Link GetLink(Predicate<Link> predicate)
		{
            return Links.Values.SingleOrDefault(x => predicate(x));
        }

        public Relation GetRelation(Predicate<Relation> predicate)
        {
            return Relations.Values.SingleOrDefault(x => predicate(x));
        }

        public IEnumerable<EntityAttribute> GetEntityAttributes()
		{
            return EntityAttributes.Values;
        }

        public IEnumerable<GivenType> GetGivenTypes()
		{
            return GivenTypes.Values;
        }

        public IEnumerable<Link> GetLinks()
		{
			return Links.Values;
        }

        public IEnumerable<Relation> GetRelations()
		{
			return Relations.Values;
        }

        public IEnumerable<Entity> GetEntities()
		{
            return Entities.Values;
        }

        public IEnumerable<EntityAttribute> GetEntityAttributes(Predicate<EntityAttribute> predicate)
		{
            return EntityAttributes.Values.Where (x => predicate(x));
        }

        public IEnumerable<GivenType> GetGivenTypes(Predicate<GivenType> predicate)
		{
            return GivenTypes.Values.Where(x => predicate(x));
        }

        public IEnumerable<Link> GetLinks(Predicate<Link> predicate)
		{
			return Links.Values.Where(x => predicate(x));
        }

        public IEnumerable<Relation> GetRelations(Predicate<Relation> predicate)
		{
            return Relations.Values.Where(x => predicate(x));
        }

        public IEnumerable<Entity> GetEntities(Predicate<Entity> predicate)
		{
            return Entities.Values.Where(x => predicate(x));
        }
    }
}
