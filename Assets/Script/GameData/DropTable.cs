using System;
[Serializable]
public class DropTable
{
    public RewardTYPE item1;
    public RewardTYPE item2;
    public float typeValue_1;
    public float typeValue_2;

    public DropTable(RewardTYPE i1, RewardTYPE i2, float value1, float value2)
    {
        item1 = i1;
        item2 = i2;
        typeValue_1 = value1;
        typeValue_2 = value2;
    }

    public DropTable(RewardTYPE i1, float value1)
    {
        item1 = i1;
        typeValue_1 = value1;
    }

}
