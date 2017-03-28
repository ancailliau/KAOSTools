using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    public static class Helpers
    {
        public static T ShallBeSingle<T>(this IEnumerable<T> collection)
        {
            Assert.That(collection.Count() == 1);
            return collection.Single();
		}

		public static IEnumerable<T> ShallContain<T>(this IEnumerable<T> collection, T item)
		{
            Assert.That(collection.Contains(item));
			return collection;
		}

		public static IEnumerable<T> ShallContain<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
            Assert.That(collection.Any(predicate));
            return collection.Where(predicate);
		}

        public static IEnumerable<T> ShallOnlyContain<T>(this IEnumerable<T> collection, T item)
        {
            Assert.That(collection.OnlyContains(new [] { item }));
            return collection;
		}

		public static IEnumerable<T> ShallOnlyContain<T>(this IEnumerable<T> collection, IEnumerable<T> item)
		{
            Assert.That(collection.OnlyContains(item), 
                        "Collection shall only contains {"+string.Join (",", item.Select (x => x.ToString ()))+"}" 
                        + " but contains {"+string.Join (",", collection.Select (x=> x.ToString ()))+"}");
			return collection;
		}

		public static T ShallBeSuchThat<T>(this T element, Func<T, bool> predicate)
		{
            Assert.That(predicate(element));
			return element;
		}

        public static IEnumerable<T> ShallBeSuchThat<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            Assert.That(collection.All(predicate));
            return collection;
		}

        public static bool ShallBeTrue(this bool value)
		{
            Assert.IsTrue(value);
			return value;
		}

		public static T ShallEqual<T>(this T value, T item)
		{
            Assert.AreEqual(item, value);
			return value;
		}
        public static bool OnlyContains<T>(this IEnumerable<T> collection, IEnumerable<T> collection2)
        {
            return !collection.Except(collection2).Any();
        }
    }
}
