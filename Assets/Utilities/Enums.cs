public static class Enums
{
    public enum RainType 
    { 
        None, Low, Medium, High, Heavy 
    }

    public enum FogType
    {
        None, Low, Medium, High
    }

    public enum ThunderType
    {
        None, Low, Medium, High
    }

    public enum EnvironmentLightIntensityType
    {
        Low, Medium, High, Max
    }

    public enum Cursor_SelectionType
    {
        Default, Default_Pressed, 
        Interaction,
    }

    public enum LightType
    {
        None, Main, Additional, Thunder, Other,
    }

    public enum PlantType
    {
        None, Wheat, Carrot,
    }

    public enum Plant_GrowingState
    {
        Seed, Sprout, Plant
    }
}