using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI priceTxt;

    public TextMeshProUGUI infoTxt1;
    public TextMeshProUGUI infoTxt2;
    public TextMeshProUGUI infoTxt3;

    public Image infoIcon1;
    public Image infoIcon2;
    public Image infoIcon3;

    public Button runButton;

    private ProductInfo product;
    private ShopManager shopManager;

    private void Awake()
    {
        runButton.onClick.AddListener(ClickButton);
    }

    public void Init(ShopManager manager, ProductInfo info)
    {
        shopManager = manager;
        product = info;
        //nameTxt.text = product.name;
        nameTxt.text =  I2.Loc.LocalizationManager.GetTranslation(product.name);

        iconImage.sprite = Utils.GetUiSprite(product.type.ToString()).uiSprite;

        infoTxt1.text = product.infoText1;
        infoIcon1.sprite = Utils.GetUiSprite(product.infoIcon1).uiSprite;

        if (product.infoText2 == "0")
        {
            infoTxt2.transform.parent.gameObject.SetActive(false);
            infoTxt3.transform.parent.gameObject.SetActive(false);
            return;
        }

        infoTxt2.text = product.infoText2;
        infoIcon2.sprite = Utils.GetUiSprite(product.infoIcon2).uiSprite;

        if (product.infoText3 == "0")
        {
            infoTxt3.transform.parent.gameObject.SetActive(false);
            return;
        }

        infoTxt3.text = product.infoText3;
        infoIcon3.sprite = Utils.GetUiSprite(product.infoIcon3).uiSprite;




    }

    public void RefreshUI()
    {
        nameTxt.text =  I2.Loc.LocalizationManager.GetTranslation(product.name);
        priceTxt.text =  I2.Loc.LocalizationManager.GetTranslation(product.price.ToString());
    }

    private void ClickButton()
    {
        if (product == null) return;
        if (product.type == ShopProductType.AD_FREE_PASS || product.type == ShopProductType.FAST_PACKAGE || product.type == ShopProductType.GOOD_PACKAGE || product.type == ShopProductType.START_PACKAGE)
        {
            shopManager.RefreshHasPackage();
        } 
    }
}