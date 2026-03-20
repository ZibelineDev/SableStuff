using Godot;

public static class ExportCheck
{
    public static void IsNull(object sender, object obj)
    {
        if (obj == null)
        {
            GD.PrintErr($"{sender.GetType().FullName} is missing an Export.");
        }
    }

    public static void IsNull(object sender, object[] objects)
    {
        foreach (object obj in objects)
        {
            if (obj == null)
            {
                GD.PrintErr($"{sender.GetType().FullName} is missing an Export.");
            }
        }
    }
}