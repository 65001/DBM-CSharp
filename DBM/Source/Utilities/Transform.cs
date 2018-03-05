using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.SmallBasic.Library;

namespace DBM
{
    public static class Transform
    {
        public static Primitive ToPrimitiveArray<T>(this List<T> List)
        {
            int StackPointer = Stack.Add("Transform.ListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }
        
        public static Primitive ToPrimitiveArray<T>(this IReadOnlyList<T> List)
        {
            int StackPointer = Stack.Add("Transform.IReadOnlyListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }
        
        public static Primitive ToPrimitiveArray<T>(this ReadOnlyCollection<T> List)
        {
            int StackPointer = Stack.Add("Transform.ReadOnlyCollectionToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {

                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this IDictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.DictionaryToPrimitiveArray");
            StringBuilder Exporter = new StringBuilder();
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Exporter.Append(entry.Key + "=" + entry.Value + ";");
            }
            Stack.Exit(StackPointer);
            return Exporter.ToString();
        }

        public static void AddOrReplace<T>(this IDictionary<T, T> Dictionary, T Key, T Value)
        {
            int StackPointer = Stack.Add("Transform.AddorReplaceDictionary");
            if (Dictionary.ContainsKey(Key) == true)
            {
                Dictionary[Key] = Value;
                Stack.Exit(StackPointer);
                return;
            }
            Dictionary.Add(Key, Value);
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IList<T> List)
        {
            int StackPointer = Stack.Add("Transform.PrintList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IReadOnlyList<T> List)
        {
            int StackPointer = Stack.Add("Transform.PrintIReadOnlyList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IDictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.PrintDictionary");
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IReadOnlyDictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.PrintIReadOnlyDictionary");
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Stack.Exit(StackPointer);
        }

        public static string SanitizeFieldName(this string String)
        {
            return String?.Replace("\"", "").Replace("[", "").Replace("]", "");
        }

        public static bool IsInteger(this string text)
        {
            return int.TryParse(text, out int test);
        }

        public static bool IsDouble(this string text)
        {
            return double.TryParse(text, out double test);
        }

        public static bool IsFloat(this string text)
        {
            return float.TryParse(text, out float test);
        }
    }
}
