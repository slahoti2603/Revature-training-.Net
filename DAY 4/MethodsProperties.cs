using System;

public class PropertiesDemo
{
    readonly string name;

    public int MyProperty { get; init; }

    public int MyAge { get; private set; }

    public PropertiesDemo()
    {
        name = "Sakshi";
        MyProperty = 42;
        MyAge = 22;
    }

    public void ModifyName()
    {
        MyAge = 23;
    }
}
