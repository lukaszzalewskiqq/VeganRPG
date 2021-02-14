using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeganRPG
{
    // Examples:
    // Ability causes player to do 3 times more damage, then
    // playerDmgMultipler should be set to 3 - 1 = 2, because 1 is basic value and it works additively
    //
    // Ability causes player to do 20 more damage, then
    // playerDmgMin, playerDmgMax should be set to 20

    class AbilityEffects
    {
        double playerDefenseAdded;
        double playerDefenseMultipler;

        double playerDmgMin, playerDmgMax;
        double playerDmgMultipler;


        double enemyDefenseAdded;
        double enemyDefenseMultipler;

        double enemyDmgMin, enemyDmgMax;
        double enemyDmgMultipler;

        double enemyGoldMin, enemyGoldMax;
        double enemyGoldMultipler;

        double enemyExperienceAdded;
        double enemyExperienceMultipler;

        public AbilityEffects()
        {
            PlayerDefenseAdded = 0;
            PlayerDefenseMultipler = 1;

            PlayerDmgMin = 0;
            PlayerDmgMax = 0;
            PlayerDmgMultipler = 1;


            EnemyDefenseAdded = 0;
            EnemyDefenseMultipler = 1;

            EnemyDmgMin = 0;
            EnemyDmgMax = 0;
            EnemyDmgMultipler = 1;

            EnemyGoldMin = 0;
            EnemyGoldMax = 0;
            EnemyGoldMultipler = 1;

            EnemyExperienceAdded = 0;
            EnemyExperienceMultipler = 1;
        }

        public AbilityEffects(double playerDefenseAdded, double playerDefenseMultipler, 
            double playerDmgMin, double playerDmgMax, double playerDmgMultipler,
            double enemyDefenseAdded, double enemyDefenseMultipler, 
            double enemyDmgMin, double enemyDmgMax, double enemyDmgMultipler,
            double enemyGoldMin, double enemyGoldMax, double enemyGoldMultipler,
            double enemyExperienceAdded, double enemyExperienceMultipler)
        {
            this.PlayerDefenseAdded = playerDefenseAdded;
            this.PlayerDefenseMultipler = playerDefenseMultipler;

            this.PlayerDmgMin = playerDmgMin;
            this.PlayerDmgMax = playerDmgMax;
            this.PlayerDmgMultipler = playerDmgMultipler;


            this.EnemyDefenseAdded = enemyDefenseAdded;
            this.EnemyDefenseMultipler = enemyDefenseMultipler;

            this.EnemyDmgMin = enemyDmgMin;
            this.EnemyDmgMax = enemyDmgMax;
            this.EnemyDmgMultipler = enemyDmgMultipler;

            this.EnemyGoldMin = enemyGoldMin;
            this.EnemyGoldMax = enemyGoldMax;
            this.EnemyGoldMultipler = enemyGoldMultipler;

            this.EnemyExperienceAdded = enemyExperienceAdded;
            this.EnemyExperienceMultipler = enemyExperienceMultipler;
        }

        public void ApplyEffects(Player player, int basePlayerDefense, Tuple<int, int> basePlayerDamage,
            Enemy enemy, int baseEnemyDefense, Tuple<int, int> baseEnemyDamage,
            Tuple<int, int> baseEnemyGold, int baseEnemyExperience)
        {
            player.Defense = Convert.ToInt32((basePlayerDefense + PlayerDefenseAdded) * PlayerDefenseMultipler);

            player.Damage = new Tuple<int, int>(
                Convert.ToInt32((basePlayerDamage.Item1 + PlayerDmgMin) * PlayerDmgMultipler),
                Convert.ToInt32((basePlayerDamage.Item2 + PlayerDmgMax) * PlayerDmgMultipler));


            enemy.Defense = Convert.ToInt32((baseEnemyDefense + EnemyDefenseAdded) * EnemyDefenseMultipler);

            enemy.Damage = new Tuple<int, int>(
                Convert.ToInt32((baseEnemyDamage.Item1 + EnemyDmgMin) * EnemyDmgMultipler),
                Convert.ToInt32((baseEnemyDamage.Item2 + EnemyDmgMax) * EnemyDmgMultipler));

            enemy.Gold = new Tuple<int, int>(
                Convert.ToInt32((baseEnemyGold.Item1 + EnemyGoldMin) * EnemyGoldMultipler),
                Convert.ToInt32((baseEnemyGold.Item2 + EnemyGoldMax) * EnemyGoldMultipler));

            enemy.Experience = Convert.ToInt32((baseEnemyExperience + EnemyExperienceAdded) * EnemyExperienceMultipler);
        }

        public double PlayerDefenseAdded { get => playerDefenseAdded; set => playerDefenseAdded = value; }
        public double PlayerDefenseMultipler { get => playerDefenseMultipler; set => playerDefenseMultipler = value; }
        public double PlayerDmgMin { get => playerDmgMin; set => playerDmgMin = value; }
        public double PlayerDmgMax { get => playerDmgMax; set => playerDmgMax = value; }
        public double PlayerDmgMultipler { get => playerDmgMultipler; set => playerDmgMultipler = value; }
        public double EnemyDefenseAdded { get => enemyDefenseAdded; set => enemyDefenseAdded = value; }
        public double EnemyDefenseMultipler { get => enemyDefenseMultipler; set => enemyDefenseMultipler = value; }
        public double EnemyDmgMin { get => enemyDmgMin; set => enemyDmgMin = value; }
        public double EnemyDmgMax { get => enemyDmgMax; set => enemyDmgMax = value; }
        public double EnemyDmgMultipler { get => enemyDmgMultipler; set => enemyDmgMultipler = value; }
        public double EnemyGoldMin { get => enemyGoldMin; set => enemyGoldMin = value; }
        public double EnemyGoldMax { get => enemyGoldMax; set => enemyGoldMax = value; }
        public double EnemyGoldMultipler { get => enemyGoldMultipler; set => enemyGoldMultipler = value; }
        public double EnemyExperienceAdded { get => enemyExperienceAdded; set => enemyExperienceAdded = value; }
        public double EnemyExperienceMultipler { get => enemyExperienceMultipler; set => enemyExperienceMultipler = value; }
    }
}
