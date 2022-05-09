using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InputData
{
    private LinkedList<long> inputsCurrentAndPrevious;

    public void Init()
    {
        inputsCurrentAndPrevious = new LinkedList<long>();
    }

    // Handling of entire sets of inputs
    public void AddCurrentInputs(long newInputs)
    {
        inputsCurrentAndPrevious.AddLast(newInputs);
        if (inputsCurrentAndPrevious.Count > HFConstants.INPUT_PREVIOUS_STORED_MAX)
        {
            inputsCurrentAndPrevious.RemoveFirst();
        }
    }

    public long CurrentInputs => inputsCurrentAndPrevious.Last.Value;

    // Public (not static) methods for individual input queries
    public bool GetInputDown(int inputConstant)
    {
        if ((CurrentInputs & inputConstant) != 0)
        {
            return true;
        }
        return false;
    }
    public bool GetInputJustPressed(int inputConstant)
    {
        if ((CurrentInputs & inputConstant) != 0)
        {
            if ((inputsCurrentAndPrevious.Last.Previous.Value & inputConstant) == 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool GetInputHeldDown(int inputConstant, int durationInFrames)
    {
        // Validate duration parameter
        if (durationInFrames < 1)
        {
            return false;
        }

        LinkedListNode<long> currentInputsNode = inputsCurrentAndPrevious.Last;
        for (int i = 0; i < durationInFrames; i++)
        {
            if ((currentInputsNode.Value & inputConstant) == 0)
            {
                return false;
            }
            currentInputsNode = currentInputsNode.Previous;
        }
        return true;
    }

    public bool GetInputJustReleased(int inputConstant)
    {
        if ((CurrentInputs & inputConstant) == 0)
        {
            if ((inputsCurrentAndPrevious.Last.Previous.Value & inputConstant) != 0)
            {
                return true;
            }
        }
        return false;
    }
}

