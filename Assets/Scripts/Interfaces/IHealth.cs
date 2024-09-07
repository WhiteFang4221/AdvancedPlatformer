using System;

public interface IHealth
{
    float CurrentValue { get;}
    float MaxValue { get;}

    event Action<float, float> Changed;
}
