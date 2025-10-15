using System;

public static class LayerMaskUtils
{
    public static bool IsMaskContainIndex(int mask, int layerIndex)
    {
        return IsMaskContainAny(mask, 1<<layerIndex);
    }


    public static bool IsMaskContainAll(int mask, int other)
    {
        return (mask & other) == other;
    }


    public static bool IsMaskContainAny(int mask, int other)
    {
        if (other == 0)
        {
            return true;
        }

        return (mask & other) != 0;
    }
}