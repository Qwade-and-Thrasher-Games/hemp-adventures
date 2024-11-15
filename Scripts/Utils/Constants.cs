public static class Constants
{
    public static class Game
    {
        public const string GAME_VERSION = "0.1.0-dev";
        public const decimal STARTING_MONEY = 1000;
        public const int GAME_SPEED = 100;

        public static class Save
        {
            public const string BACKUP_EXT = ".bak";
            public const string SAVE_FILE_NAME = "save.json";
            public const string SAVE_DIR = "user://saves/";
        }
    }
    public static class Growth
    {
        public const float BASE_GROWTH_RATE = 0.1f;
        public const float NUTRIENT_MULTIPLIER = 1.5f;
        public const float WATER_MULTIPLIER = 1.2f;
        public const float LIGHT_MULTIPLIER = 1.3f;
    }

    public static class Market
    {
        public const float MIN_PRICE_MULTIPLIER = 0.5f;
        public const float MAX_PRICE_MULTIPLIER = 2.0f;
        public const float REPUTATION_GAIN_RATE = 0.1f;
    }
}