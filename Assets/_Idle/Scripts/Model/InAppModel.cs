using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Idle.Scripts.Analytics;
using _Idle.Scripts.Balance;
using GeneralTools.Model;
using GeneralTools.Tools;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Idle.Scripts.Model
{
    public class InAppModel : BaseModel, IStoreListener
    {
        public bool StoreControllerAvailable => _storeController != null;
        //private const string PREFIX = "com.toy.stream.fluffy.brawl.";

        private GameModel _game;
        
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        
        private Action<string> _onPurchased;
        private Action _onPurchaseFailed;

        public override BaseModel Start()
        {
            _game = Models.Get<GameModel>();
            
            var configuration = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            var offers = GameBalance.Instance.Offers
                .FindAll(o => !string.IsNullOrEmpty(o.ShopId))
                .ToList();
            
            AddProductsToBuilder(configuration, offers);
            
            UnityPurchasing.Initialize(this, configuration);
            
            return base.Start();
        }

        public bool IsBought(string id)
        {
            return _storeController.products.WithID(id).hasReceipt;
        }

        public (ProductMetadata MetaData, int Amount) GetProductData(string token)
        {
            var productId = token;
            var product = _storeController.products.WithID(productId);
        
            if (productId == null) return default;
        
            var amountStr = token.Split('.').LastValue();
            var amount = !string.IsNullOrEmpty(amountStr) && int.TryParse(amountStr, out var a)
                 ? a
                 : 1;
        
            return (product.metadata, amount);
        }
        
        private void AddProductsToBuilder(ConfigurationBuilder builder, List<OfferConfig> tokens)
        {
            foreach (var token in tokens)
            {
                AddProductToConfiguration(builder, token.ShopId, token.NonConsumable);
            }
        }
        
        private void AddProductToConfiguration(ConfigurationBuilder builder, string token, bool nonConsumable)
        {
            builder.AddProduct(token, nonConsumable ? ProductType.NonConsumable : ProductType.Consumable);
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Log($"InApp initialization failed: {error}");
        }

        public void Purchase(string token, Action<string> onPurchased = null, Action onFailed = null)
        {
            _onPurchased = onPurchased;
            _onPurchaseFailed = onFailed;

            var product = token;
            _storeController.InitiatePurchase(product);
        }
        
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            var product = e.purchasedProduct;
            var token = product.definition.id;
            
            if (!IsShopItemPurchased(token))
            {
                Log($"ProcessPurchase: FAIL. Unrecognized product: '{e.purchasedProduct.definition}'");
            }
            else
            {
                var purchaseData = new InAppPurchaseData()
                {
                    Currency = product.metadata.isoCurrencyCode,
                    Revenue = product.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture),
                    Quantity = "1",
                    ID = product.definition.id,
                    Transaction = product.transactionID
                };
                
                AppEventsProvider.TriggerEvent(GameEvents.InAppPurchased, purchaseData);
                
                Log($"ProcessPurchase: SUCCESS. Product: '{e.purchasedProduct.definition}'");
            }
            
            return PurchaseProcessingResult.Complete;
        }
        
        private bool IsShopItemPurchased(string token)
        {
            OnSuccessPurchase(token);
            return true;
        }

        private void OnSuccessPurchase(string token)
        {
            ConfirmPendingPurchase(token);
            
            _onPurchased?.Invoke(token);
            _onPurchased = null;
        }
        
        private void ConfirmPendingPurchase(string token)
        {
            var product = _storeController.products.WithID(token);
            _storeController.ConfirmPendingPurchase(product);
        }
        
        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            _onPurchaseFailed?.Invoke();
        }

        private void Log(string message)
        {
            Debug.Log(message);
        }
    }
}