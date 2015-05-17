using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindHelper
{
    class CollectionUtility
    {
        /// <summary>
        /// Checks if any of the item in collection is contained in the Value given.
        /// </summary>
        /// <param name="Value">The data to be checked.</param>
        /// <param name="CollectionToBeSearched">String collections to be checked if contained in the value</param>
        /// <returns>True if any of the Item is contained in the value given, otherwise False.</returns>
        public static bool CollectionItemContainsInValue(string Value, string[] CollectionToBeSearched)
        {
            bool ItemContainsInExpression = false;

            foreach (string Character in CollectionToBeSearched)
            {
                if (Value.Contains(Character))
                {
                    ItemContainsInExpression = true;
                    break;
                }
            }

            return ItemContainsInExpression;
        }

        public static bool CollectionItemContainsTheValue<T>(T Value, List<List<T>> CollectionList)
        {
            bool ItemContainsInCollection = true;

            foreach (List<T> List in CollectionList)
            {
                if (!List.Contains(Value))
                {
                    ItemContainsInCollection = false;
                    break;
                }
            }

            return ItemContainsInCollection;
        }

        /// <summary>
        /// Merge two dictionary collection.
        /// </summary>
        /// <typeparam name="TKey">Key Type for the Dictionary.</typeparam>
        /// <typeparam name="TValue">Value Type for the Dictionary.</typeparam>
        /// <param name="FirstCollection">Main collection to be merged with the second collection.</param>
        /// <param name="SecondCollection">Second collection to be merged with the first.</param>
        /// <returns>Merged Dictionary Collection.</returns>
        public static Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(Dictionary<TKey, TValue> FirstCollection, Dictionary<TKey, TValue> SecondCollection)
        {
            foreach (KeyValuePair<TKey, TValue> Item in SecondCollection)
            {
                if (!FirstCollection.ContainsKey(Item.Key))
                {
                    FirstCollection.Add(Item.Key, Item.Value);
                }
            }

            return FirstCollection;
        }
    }
}
