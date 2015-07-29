/// <summary>
/// Sequence.cs
/// Authors: Kyle Dawson
/// Date Created:  July 21, 2015
/// Last Revision: July 23, 2015
/// 
/// Class for managing a list of events to occur sequentially.
/// 
/// NOTES: - This is not a static class, it is created similarly to a list.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class Sequence {

	List<SequenceSlot> sequence;	// List of things to be done in sequence.
	public bool isRunning;			// Whether or not the sequence is running.

	// AddSequence - Adds an element to the sequence.
	public void AddSequence(SequenceSlot slot) {
		sequence.Add(slot);
		sequence.Sort(); // Not sure whether its more efficient to keep the list sorted, or to sort it only once.
	}

	// StartSequence - Runs the sequence on a given MonoBehaviour.
	public void StartSequence(MonoBehaviour obj, bool clearOnFinish = false) {
		if (!isRunning) {
			obj.StartCoroutine(RunSequence(obj, clearOnFinish));
		}
	}

	// RunSequence - Iterates through the sequence.
	IEnumerator RunSequence(MonoBehaviour obj, bool clearOnFinish) {
		isRunning = true;

		// Iterates through slots.
		for (int i = 0; i < sequence.Count; i++) {
			// Execute instant functions.
			if (sequence[i].action != null) sequence[i].action();

			// Start coroutines and wait for them to finish before proceeding.
			if (sequence[i].routine != null)
				yield return obj.StartCoroutine(sequence[i].routine);
			else
				yield return null;
		}

		// Clears list if desired.
		if (clearOnFinish)
			sequence.Clear();

		isRunning = false;
	}

	// Constructor
	public Sequence() {
		isRunning = false;
		sequence = new List<SequenceSlot>();
	}
}

public class SequenceSlot : IComparable<SequenceSlot> {
	public int priority;			// Preference in sequence.
	public UnityAction action;		// Quick functions to execute.
	public IEnumerator routine;		// Routine to execute.

	// Constructors
	public SequenceSlot(int priority, IEnumerator routine) {
		this.priority = priority;
		this.routine = routine;
	}

	public SequenceSlot(int priority, UnityAction action) {
		this.priority = priority;
		this.action = action;
	}

	public SequenceSlot(int priority, UnityAction action, IEnumerator routine) {
		this.priority = priority;
		this.action = action;
		this.routine = routine;
	}

	// CompareTo - IComparable requirement for how to sort this class in a list.
	public int CompareTo(SequenceSlot seqSlot) {
		return this.priority.CompareTo(seqSlot.priority);
	}
}