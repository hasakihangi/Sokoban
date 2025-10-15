using System;

public struct GridCell : ICell
{
    private bool enable;
    public bool Enable { get => enable; set => enable = value; }
    
    public void Init()
    {
        enable = false;
    }
    
}