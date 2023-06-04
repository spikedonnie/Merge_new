public class Stat
{
    private float mCalcDamage;
    private float mCalcCriticalChance;
    private float mCalcCriticalDamage;
    private float mAttackSpeed;

    public void SetDamage(float damage)
    {
        mCalcDamage = damage;
    }

    public float GetDamage()
    {
        return mCalcDamage;
    }

    public void SetCriticalChance(float chance)
    {
        mCalcCriticalChance = chance;
    }

    public float GetCriticalChance()
    {
        return mCalcCriticalChance;
    }

    public void SetCriticalDamage(float cri)
    {
        mCalcCriticalDamage = cri;
    }

    public float GetCriticalDamage()
    {
        return mCalcCriticalDamage;
    }

    public void SetAttackSpeed(float speed)
    {
        mAttackSpeed = speed;
    }

    public float GetAttackSpeed()
    {
        return mAttackSpeed;
    }
}