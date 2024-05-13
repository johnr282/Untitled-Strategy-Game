using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Implements a priority queue using a SortedSet, doesn't support
// duplicates; Unity's version of .NET doesn't include a priority queue
// container
// ------------------------------------------------------------------

public class PriorityQueue<TElement, TPriority>
where TPriority : IComparable
{
    struct Element
    {
        public TElement Value { get; }
        public TPriority Priority { get; }

        public Element(TElement valueIn, 
            TPriority priorityIn)
        {
            Value = valueIn; 
            Priority = priorityIn;
        }
    }

    class PriorityComparer : IComparer<Element>
    {
        public int Compare(Element x, Element y) 
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }

    List<Element> _list = new();

    public int Count { get => _list.Count; }

    // Returns whether the queue is empty
    public bool Empty()
    {
        return Count == 0;
    }

    // Removes and returns the element with the lowest priority
    public TElement Dequeue()
    {
        Element minElement = _list[0];
        _list.Remove(minElement);
        return minElement.Value;
    }


    // Adds the given element with the given priority to the queue
    // Throws an ArgumentException if an element with the same priority
    // already exists in the queue
    public void Enqueue(TElement element, 
        TPriority priority)
    {
        _list.Add(new Element(element, priority));
        _list.Sort(new PriorityComparer());
    }
}