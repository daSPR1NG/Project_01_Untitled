public static class Enums
{
    public enum Rain_Type 
    { 
        None, 
        Low, Medium, High, Heavy 
    }

    public enum Fog_Type
    {
        None, 
        Low, Medium, High
    }

    public enum Thunder_Type
    {
        None, 
        Low, Medium, High
    }

    public enum Environment_LightIntensity_Type
    {
        Low, 
        Medium, High, Max
    }

    public enum Cursor_SelectionType
    {
        Default, 
        Interaction,
    }

    public enum Light_Type
    {
        None, Main, Additional, Thunder, Other,
    }

    public enum Plant_Type
    {
        None, Wheat, Carrot,
    }

    public enum Plant_GrowingState
    {
        Seed, Sprout, Plant
    }
}