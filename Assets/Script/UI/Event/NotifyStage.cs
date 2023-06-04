using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifyStage : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI nameText;

    public void Notify(int stage, EnemyTYPE type)
    {

        textMesh.text = string.Format("{0}", stage);

        if (type.Equals(EnemyTYPE.Common))
        {
            nameText.text = string.Format("STAGE");
        }
        else
        {
            nameText.text = string.Format("BOSS");
        }

    }
}