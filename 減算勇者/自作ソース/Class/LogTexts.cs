namespace Genzan
{
    public static class LogText
    {
        public static string BattleText(string name)
        {
            return name + "とせんとう！";
        }

        public static string LevelDownByPoison = "どくをうけた！";

        public static string ChangeLevel(int level)
        {
            return level > 0 ? level + "レベルあがった！" : (-level).ToString() + "レベルさがった！";
        }
        public static string OpenTreasureBox = "たからばこをあけた！";
        public static string EmptyTreasureBox = "たからばこはからだった！";

        public static string LevelDownByBoss(string name)
        {
            return name + "のこうげき！";
        }

        public static string DamagetoBoss(string name, uint level)
        {
            return name + "にこうげき！\n" + level + "ダメージあたえた！";
        }
        public static string DefeatBoss(string name)
        {
            return name + "をたおした！";
        }

        public static string[] StageClearText = { "スライムのもりクリア", "まおうのしろクリア" };
    }
}