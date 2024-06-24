internal class EnemyAnimationStrings
{
    public static class Params
    {
        public const string IsRunning = "IsRunning";
        public const string HitTrigger = "HitTrigger";
        public const string IsAttacking = "IsAttacking";
        public const string IsDead = "IsDead";
    }

    public static class States
    {
        public const string Idle = nameof(Idle);
        public const string WizardRun = nameof(WizardRun);
        public const string WizardAttack = nameof(WizardAttack);
        public const string TakeHit = nameof(TakeHit);
        public const string WizardDeath = nameof(WizardDeath);
    }
}