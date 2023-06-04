using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Windows;

public class InApp : MonoBehaviour
{
    public class MyStoreProducts
    {
        public ShopProductNames productName;
        public bool isBuy;

        public MyStoreProducts(ShopProductNames name, bool buy)
        {
            productName = name;
            isBuy = buy;
        }
    }

    private List<MyStoreProducts> consumableProducts = new List<MyStoreProducts>();
    private List<MyStoreProducts> nonConsumableProducts = new List<MyStoreProducts>();

    //bool initializationInProgress = false;
    public IEnumerator Init()
    {
        if (!IAPManager.Instance.IsInitialized())
        {
            IAPManager.Instance.InitializeIAPManager(InitializeResult);
        }

        IAPManager.Instance.RestorePurchases(ProductBought, RestoreDone);

        yield return null;
    }

    public void BuyGoodsI()
    {
        IAPManager.Instance.BuyProduct(consumableProducts[0].productName, ProductBought);
    }

    public void BuyGoodsII()
    {
        IAPManager.Instance.BuyProduct(consumableProducts[1].productName, ProductBought);
    }

    public void BuyGoodsIII()
    {
        IAPManager.Instance.BuyProduct(consumableProducts[2].productName, ProductBought);
    }

    public void BuyGoodsIIII()
    {
        IAPManager.Instance.BuyProduct(consumableProducts[3].productName, ProductBought);
    }



    public void BuyAdPass()
    {
        if (GameDataManager.instance.GetSaveData.productPackage.buyAdPassPackage)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.HAVE_PACKAGE);


            return; 
        }
        IAPManager.Instance.BuyProduct(nonConsumableProducts[(int)ShopProductNames.ad_free_pass].productName, ProductBought);
    }

    public void BuyFastPackage()
    {
        if (GameDataManager.instance.GetSaveData.productPackage.buyFastPackage)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.HAVE_PACKAGE);

            return;
        }
        IAPManager.Instance.BuyProduct(nonConsumableProducts[(int)ShopProductNames.fast_package].productName, ProductBought);
    }

    public void BuyGoodPackage()
    {
        if (GameDataManager.instance.GetSaveData.productPackage.buyGoodPackage)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.HAVE_PACKAGE);

            return;
        }
        IAPManager.Instance.BuyProduct(nonConsumableProducts[(int)ShopProductNames.good_package].productName, ProductBought);
    }

    public void BuyStartPackage()
    {
        if (GameDataManager.instance.GetSaveData.productPackage.buyStartPackage)
        {
            UIController.instance.SendPopupMessage(AlarmTYPE.HAVE_PACKAGE);

            return;
        }
        IAPManager.Instance.BuyProduct(nonConsumableProducts[(int)ShopProductNames.start_package].productName, ProductBought);
    }

    private void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            var manager = GameDataManager.instance;

            GameController.instance.AudioManager.PlayEffect(ESoundEffect.GetCurrency);

            //each consumable gives coins in this example
            if (product.productType == ProductType.Consumable)
            {
                if (product.productName == "goods_1")
                {
                    var value = Int32.Parse(manager.SheetJsonLoader.GetProductData(ShopProductType.GOODS_1).INFO_TEXT_1);

                    manager.AddDiamondData(value);
                }
                else if (product.productName == "goods_2")
                {
                    var value = Int32.Parse(manager.SheetJsonLoader.GetProductData(ShopProductType.GOODS_2).INFO_TEXT_1);
                    manager.AddDiamondData(value);
                }
                else if (product.productName == "goods_3")
                {
                    var value = Int32.Parse(manager.SheetJsonLoader.GetProductData(ShopProductType.GOODS_3).INFO_TEXT_1);
                    manager.AddDiamondData(value);
                }
                else if (product.productName == "goods_4")
                {
                    var value = Int32.Parse(manager.SheetJsonLoader.GetProductData(ShopProductType.GOODS_4).INFO_TEXT_1);
                    manager.AddDiamondData(value);
                }
                else
                {

                }
            }

            if (product.productType == ProductType.NonConsumable)
            {
                if (product.productName == "ad_free_pass")
                {
                    if (manager.GetSaveData.productPackage.buyAdPassPackage) return;
                    var value = manager.SheetJsonLoader.GetProductData(ShopProductType.AD_FREE_PASS);
                    manager.GetSaveData.productPackage.buyAdPassPackage = true;
                    manager.AddDiamondData(Int32.Parse(value.INFO_TEXT_2));
                    manager.SaveDataCurrency(RewardTYPE.SpiritStone, Int32.Parse(value.INFO_TEXT_3));
                    var symbol = NumberToSymbol.ChangeNumber(manager.GetCurrency(RewardTYPE.SpiritStone));
                    UIController.instance.UpdateSpiritStoneText(symbol, NumberToSymbol.ChangeNumber(Int32.Parse(value.INFO_TEXT_3)), Color.white);
                    UIController.instance.ShowSimpleRewardPopUp("Complete Purchase", Utils.GetUiSprite("AD_FREE_PASS").uiSprite, null);

                    if (product.active)
                    {
                        nonConsumableProducts.First(cond => cond.productName.ToString() == product.productName).isBuy = true;
                    }
                }
                else if (product.productName == "fast_package")
                {
                    if (manager.GetSaveData.productPackage.buyFastPackage) return;
                    var value = manager.SheetJsonLoader.GetProductData(ShopProductType.FAST_PACKAGE);
                    manager.AddDiamondData(Int32.Parse(value.INFO_TEXT_1));
                    manager.SetTheBellOfCall(Int32.Parse(value.INFO_TEXT_3));
                    manager.SaveDataCurrency(RewardTYPE.SpiritStone, Int32.Parse(value.INFO_TEXT_2));
                    var symbol = NumberToSymbol.ChangeNumber(manager.GetCurrency(RewardTYPE.SpiritStone));
                    UIController.instance.UpdateSpiritStoneText(symbol, NumberToSymbol.ChangeNumber(Int32.Parse(value.INFO_TEXT_2)), Color.white);

                    UIController.instance.ShowSimpleRewardPopUp("Complete Purchase", Utils.GetUiSprite("FAST_PACKAGE").uiSprite, null);
                    manager.GetSaveData.productPackage.buyFastPackage = true;


                    if (product.active)
                    {
                        nonConsumableProducts.First(cond => cond.productName.ToString() == product.productName).isBuy = true;
                    }
                }
                else if (product.productName == "good_package")
                {
                    if (manager.GetSaveData.productPackage.buyGoodPackage) return;

                    var value = manager.SheetJsonLoader.GetProductData(ShopProductType.GOOD_PACKAGE);
                    manager.AddDiamondData(Int32.Parse(value.INFO_TEXT_1));
                    manager.SetTheBellOfCall(Int32.Parse(value.INFO_TEXT_3));

                    manager.SaveDataCurrency(RewardTYPE.SpiritStone, Int32.Parse(value.INFO_TEXT_2));

                    var symbol = NumberToSymbol.ChangeNumber(manager.GetCurrency(RewardTYPE.SpiritStone));
                    UIController.instance.UpdateSpiritStoneText(symbol, NumberToSymbol.ChangeNumber(Int32.Parse(value.INFO_TEXT_2)), Color.white);


                    UIController.instance.ShowSimpleRewardPopUp("Complete Purchase", Utils.GetUiSprite("GOOD_PACKAGE").uiSprite, null);
                    manager.GetSaveData.productPackage.buyGoodPackage = true;

                    if (product.active)
                    {
                        nonConsumableProducts.First(cond => cond.productName.ToString() == product.productName).isBuy = true;
                    }
                }
                else if (product.productName == "start_package")
                {
                    if (manager.GetSaveData.productPackage.buyStartPackage) return;

                    var value = manager.SheetJsonLoader.GetProductData(ShopProductType.START_PACKAGE);
                    manager.AddDiamondData(Int32.Parse(value.INFO_TEXT_1));
                    manager.SetTheBellOfCall(Int32.Parse(value.INFO_TEXT_3));

                    manager.SaveDataCurrency(RewardTYPE.SpiritStone, Int32.Parse(value.INFO_TEXT_2));

                    var symbol = NumberToSymbol.ChangeNumber(manager.GetCurrency(RewardTYPE.SpiritStone));
                    UIController.instance.UpdateSpiritStoneText(symbol, NumberToSymbol.ChangeNumber(Int32.Parse(value.INFO_TEXT_2)), Color.white);


                    UIController.instance.ShowSimpleRewardPopUp("Complete Purchase", Utils.GetUiSprite("START_PACKAGE").uiSprite, null);
                    manager.GetSaveData.productPackage.buyStartPackage = true;

                    if (product.active)
                    {
                        nonConsumableProducts.First(cond => cond.productName.ToString() == product.productName).isBuy = true;
                    }
                }
            }
            
            manager.SaveCloudData();

            manager.SaveData();

        }
    }

    private void InitializeResult(IAPOperationStatus status, string message, List<StoreProduct> shopProducts)
    {
        //initializationInProgress = false;
        consumableProducts = new List<MyStoreProducts>();
        nonConsumableProducts = new List<MyStoreProducts>();

        if (status == IAPOperationStatus.Success)
        {
            //loop through all products and check which one are bought to update our variables
            for (int i = 0; i < shopProducts.Count; i++)
            {
                if (shopProducts[i].productName == "ad_free_pass")
                {
                    //if a product is active, means that user had already bought that product so enable access
                    if (shopProducts[i].active)
                    {
                        //unlockLevel1 = true;
                    }
                }

                if (shopProducts[i].productName == "fast_package")
                {
                    if (shopProducts[i].active)
                    {
                        //unlockLevel2 = true;
                    }
                }
                if (shopProducts[i].productName == "good_package")
                {
                    if (shopProducts[i].active)
                    {
                        //unlockLevel2 = true;
                    }
                }
                if (shopProducts[i].productName == "start_package")
                {
                    if (shopProducts[i].active)
                    {
                        //unlockLevel2 = true;
                    }
                }

                //construct a different list of each category of products, for an easy display purpose, not required
                switch (shopProducts[i].productType)
                {
                    case ProductType.Consumable:
                        consumableProducts.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;

                    case ProductType.NonConsumable:
                        nonConsumableProducts.Add(new MyStoreProducts(IAPManager.Instance.ConvertNameToShopProduct(shopProducts[i].productName), shopProducts[i].active));
                        break;
                }
            }
        }
    }

    private void RestoreDone()
    {
        if (IAPManager.Instance.debug)
        {
            Debug.Log("Restore done");
            GleyEasyIAP.ScreenWriter.Write("Restore done");
        }
    }










}

