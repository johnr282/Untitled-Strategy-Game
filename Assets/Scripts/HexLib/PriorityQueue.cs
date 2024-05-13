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

    SortedSet<Element> _set = new SortedSet<Element>(new PriorityComparer());

    public int Count { get => _set.Count; }

    // Returns whether the queue is empty
    public bool Empty()
    {
        return Count == 0;
    }

    // Removes and returns the element with the lowest priority
    public TElement Dequeue()
    {
        Element minElement = _set.Min;
        _set.Remove(minElement);
        return minElement.Value;
    }

    // Adds the given element with the given priority to the queue
    // Throws an ArgumentException if an element with the same priority
    // already exists in the queue
    public void Enqueue(TElement element, 
        TPriority priority)
    {
        if (!_set.Add(new Element(element, priority)))
            throw new ArgumentException(
                "Attempted to add duplicate element and priority to PriorityQueue");
    }

    // Returns whether the queue contains the given element with given priority
    public bool Contains(TElement element, 
        TPriority priority)
    {
        return _set.Contains(new Element(element, priority));
    }

    // Attempts to remove the given element with given priority from the queue
    // Returns true if it was successfully found and removed, false if not
    public bool TryRemove(TElement element,
        TPriority priority)
    {
        return _set.Remove(new Element(element, priority));
    }
}