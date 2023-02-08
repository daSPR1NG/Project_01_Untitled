namespace dnSR_Coding
{
    public interface ILevelable
    {
        public int Level { get; }

        public int InitialRequiredExperience { get; }
        public int ExperienceMultiplier { get; }

        public int CurrentExperience { get; }
        public int RequiredExperienceToNextLevel { get; }
        public float RequiredExperienceScalingFactor { get; }

        public abstract void AddExperience( int amount );
        public abstract void IncreaseRequiredExperienceToNextLevel();

        public abstract void AddLevel();
        public abstract void SetLevel( int value );
    }
}