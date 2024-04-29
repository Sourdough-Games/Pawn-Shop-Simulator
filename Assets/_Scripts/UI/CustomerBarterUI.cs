using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BarterData {
    public ProductWorldSlot slot;
    public float offerAmount;

    public int timesCountered = 0;

    public int timesWillingToBarter;

    public BarterData(ProductWorldSlot slot, float offer) {
        offerAmount = (float)Math.Round(offer);
        this.slot = slot;

        timesWillingToBarter = Random.Range(1, 2);
    }
}

public class CustomerBarterUI : Modal
{
    [SerializeField] Image productIcon;
    [SerializeField] TMP_InputField counterOfferInput;
    [SerializeField] TextMeshProUGUI counterOfferPlaceholder;
    [SerializeField] LocalizeStringEvent productNameText;
    [SerializeField] LocalizeStringEvent productTypeText;
    [SerializeField] LocalizeStringEvent productSizeText;
    [SerializeField] TextMeshProUGUI productPrice;
    [SerializeField] Button acceptOfferButton;
    [SerializeField] Button counterOfferButton;

    private ShopperNPC _shopper;

    public BarterData currentBarterData;

    private Dictionary<Product, BarterData> productOffers = new();

    public override void Draw() {
        
    }

    public BarterData GetBarterData(Product product) {
        return productOffers[product];
    }

    public void HandleCounterOfferChange()
    {
        float value = float.Parse(counterOfferInput.text);
        bool counterMatchesOffer = value == currentBarterData.offerAmount;

        Debug.LogWarning($"HandleCounterOfferChange] {value} | {currentBarterData.offerAmount} | {counterMatchesOffer}");

        acceptOfferButton.gameObject.SetActive(counterMatchesOffer);
        counterOfferButton.gameObject.SetActive(!counterMatchesOffer);
    }

    public void Setup(ShopperNPC shopper) {
        _shopper = shopper;

        Open();

        StartRandomProductBargain();
    }

    public void TriggerOfferAccepted() {

        Singleton<NotificationSystem>.Instance.ShowMessage("BarterAccepted", l_args: new() {
            { "Product", productNameText.StringReference.GetLocalizedString() },
            { "Amount", Helper.ConvertToDollarAmount(currentBarterData.offerAmount) },
        });

        Product product = currentBarterData.slot.ProductInSlot;

        Singleton<PlayerMoneyManager>.Instance.TryAddMoney(product.currentlySetPrice);

        RemoveReservation(product);

        Destroy(product.gameObject);

        StartRandomProductBargain();
    }

    public void TriggerOfferDispute()
    {
        Product product = currentBarterData.slot.ProductInSlot;
        float offer = float.Parse(counterOfferInput.text);

        if(currentBarterData.timesCountered >= currentBarterData.timesWillingToBarter) {
            Singleton<NotificationSystem>.Instance.ShowMessage("CustomerBarterGaveUp");
            TriggerDecline();
            return;
        }

        if (offer > product.currentlySetPrice)
        {
            Singleton<NotificationSystem>.Instance.ShowMessage("CustomerInsulted");
            TriggerDecline();
            return;
        }

        float maxPercentageIncrease = Mathf.Clamp(Random.Range(0.01f, 0.25f), 0.01f, 1f);

        float maxAcceptablePrice = (float)Math.Round(currentBarterData.offerAmount * (1 + maxPercentageIncrease));

        if (offer <= maxAcceptablePrice)
        {
            // Customer accepts the counteroffer
            currentBarterData.offerAmount = offer;
            TriggerOfferAccepted();
            return;
        }

        currentBarterData.offerAmount = maxAcceptablePrice;

        counterOfferInput.text = currentBarterData.offerAmount.ToString();
        counterOfferPlaceholder.text = counterOfferInput.text;

        Singleton<NotificationSystem>.Instance.ShowMessage("CustomerCountered");
        currentBarterData.timesCountered++;
    }

    public void TriggerDecline() {
        Product product = currentBarterData.slot.ProductInSlot;

        Singleton<NotificationSystem>.Instance.ShowMessage("BarterDeclined", l_args: new() {
            { "Product", productNameText.StringReference.GetLocalizedString() },
        });

        RemoveReservation(product);

        StartRandomProductBargain();
    }

    private void RemoveReservation(Product product) {
        currentBarterData.slot.Unreserve();
        _shopper.ReservedSlots.Remove(currentBarterData.slot);
        productOffers.Remove(product);
    }

    public void StartRandomProductBargain() {
        if(_shopper.ReservedSlots.Count == 0) {
            //shopper is finished shopping
            currentBarterData = null;
            _shopper.State = ShopperState.Leaving;
            Close();
            return;
        }

        ProductWorldSlot random_slot = _shopper.ReservedSlots[Random.Range(0, _shopper.ReservedSlots.Count)];
        StartProductBargain(random_slot);
    }

    public void StartProductBargain(ProductWorldSlot slot) {

        Product product = slot.ProductInSlot;

        if(productOffers.ContainsKey(product)) {
            currentBarterData = GetBarterData(product);
        } else {
            var random_discount = Helper.ExponentialBias(0, 0.35f, 2);

            currentBarterData = new BarterData(slot, (float)Math.Round(product.currentlySetPrice - (product.currentlySetPrice * random_discount), 2));
            productOffers.Add(product, currentBarterData);
        }

        productIcon.sprite = product.ProductData.sprite;

        productNameText.StringReference.TableEntryReference = product.ProductData.LocalizationKey;
        productTypeText.StringReference.TableEntryReference = $"Type{product.ProductData.productType}";
        productSizeText.StringReference.TableEntryReference = $"Size{product.ProductData.productSize}";
        productPrice.text = Helper.ConvertToDollarAmountNoCollapse(product.currentlySetPrice);

        counterOfferInput.text = currentBarterData.offerAmount.ToString();
        counterOfferPlaceholder.text = counterOfferInput.text;

        acceptOfferButton.gameObject.SetActive(true);
        counterOfferButton.gameObject.SetActive(false);

        StartCoroutine(ActivateInputField());
    }

    private IEnumerator ActivateInputField() {
        yield return new WaitForSeconds(0.1f);
        counterOfferInput.ActivateInputField();
    }
}
