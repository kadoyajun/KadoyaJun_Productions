namespace Genzan
{
    public static class LogText
    {
        public static string BattleText(string name)
        {
            return name + "�Ƃ���Ƃ��I";
        }

        public static string LevelDownByPoison = "�ǂ����������I";

        public static string ChangeLevel(int level)
        {
            return level > 0 ? level + "���x�����������I" : (-level).ToString() + "���x�����������I";
        }
        public static string OpenTreasureBox = "������΂����������I";
        public static string EmptyTreasureBox = "������΂��͂��炾�����I";

        public static string LevelDownByBoss(string name)
        {
            return name + "�̂��������I";
        }

        public static string DamagetoBoss(string name, uint level)
        {
            return name + "�ɂ��������I\n" + level + "�_���[�W���������I";
        }
        public static string DefeatBoss(string name)
        {
            return name + "�����������I";
        }

        public static string[] StageClearText = { "�X���C���̂���N���A", "�܂����̂���N���A" };
    }
}