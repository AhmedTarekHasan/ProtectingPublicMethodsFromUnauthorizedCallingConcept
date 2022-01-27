using System;
using System.Collections.Generic;

namespace ProtectingPublicMethodsFromUnauthorizedCalling.Method2.ProtectedThirdPartyOwnedShoppingCart
{
    #region Third-Party Library Members
    public interface IStoreShoppingItems
    {
        void AddShoppingItem(string itemName);
    }

    public interface IListShoppingItems
    {
        IReadOnlyCollection<string> GetShoppingItems();
    }

    public class ShoppingCart : IStoreShoppingItems, IListShoppingItems
    {
        private readonly List<string> m_ShoppingItems = new();

        public void AddShoppingItem(string itemName)
        {
            m_ShoppingItems.Add(itemName);
        }

        public IReadOnlyCollection<string> GetShoppingItems()
        {
            return m_ShoppingItems;
        }
    }

    public class Store
    {
        private readonly IStoreShoppingItems m_StoreShoppingItems;
        private readonly List<string> m_AllStoreProducts;

        public Store(IStoreShoppingItems storeShoppingItems)
        {
            m_StoreShoppingItems = storeShoppingItems;

            m_AllStoreProducts = new List<string>()
            {
                "Product 1",
                "Product 2",
                "Product 3"
            };
        }

        public void AddToCart(string product)
        {
            m_StoreShoppingItems.AddShoppingItem(product);
        }
    }

    public class CheckOutManager
    {
        private readonly IListShoppingItems m_ListShoppingItems;

        public CheckOutManager(IListShoppingItems listShoppingItems)
        {
            m_ListShoppingItems = listShoppingItems;
        }

        public IReadOnlyCollection<string> GetCheckoutItems()
        {
            return m_ListShoppingItems.GetShoppingItems();
        }
    }
    #endregion


    #region My Protected Wrappers
    public interface IProtectedStoreShoppingItems
    {
        void AddShoppingItem(string writingSecretKey, string itemName);
    }

    public interface IProtectedListShoppingItems
    {
        IReadOnlyCollection<string> GetShoppingItems(string readingSecretKey);
    }

    public class ProtectedShoppingCart : IProtectedStoreShoppingItems, IProtectedListShoppingItems
    {
        private readonly ShoppingCart m_ShoppingCart;
        private readonly string m_WritingSecretKey;
        private readonly string m_ReadingSecretKey;

        public ProtectedShoppingCart(string writingSecretKey, string readingSecretKey, ShoppingCart shoppingCart)
        {
            m_WritingSecretKey = writingSecretKey;
            m_ReadingSecretKey = readingSecretKey;
            m_ShoppingCart = shoppingCart;
        }

        public void AddShoppingItem(string writingSecretKey, string itemName)
        {
            if (writingSecretKey != m_WritingSecretKey)
                throw new UnauthorizedAccessException("The provided writing secret key is wrong.");

            m_ShoppingCart.AddShoppingItem(itemName);
        }

        public IReadOnlyCollection<string> GetShoppingItems(string readingSecretKey)
        {
            if (readingSecretKey != m_ReadingSecretKey)
                throw new UnauthorizedAccessException("The provided reading secret key is wrong.");

            return m_ShoppingCart.GetShoppingItems();
        }
    }

    public class ProtectedStore
    {
        private readonly Store m_Store;
        private readonly string m_WritingSecretKey;

        public ProtectedStore(string writingSecretKey, Store store)
        {
            m_WritingSecretKey = writingSecretKey;
            m_Store = store;
        }

        public void AddToCart(string writingSecretKey, string product)
        {
            if (writingSecretKey != m_WritingSecretKey)
                throw new UnauthorizedAccessException("The provided writing secret key is wrong.");

            m_Store.AddToCart(product);
        }
    }

    public class ProtectedCheckOutManager
    {
        private readonly CheckOutManager m_CheckOutManager;
        private readonly string m_ReadingSecretKey;

        public ProtectedCheckOutManager(string readingSecretKey, CheckOutManager checkOutManager)
        {
            m_ReadingSecretKey = readingSecretKey;
            m_CheckOutManager = checkOutManager;
        }

        public IReadOnlyCollection<string> GetCheckoutItems(string readingSecretKey)
        {
            if (readingSecretKey != m_ReadingSecretKey)
                throw new UnauthorizedAccessException("The provided reading secret key is wrong.");

            return m_CheckOutManager.GetCheckoutItems();
        }
    }
    #endregion


    public class Program
    {
        private string m_WritingSecretKey;
        private string m_ReadingSecretKey;

        public void Main()
        {
            m_WritingSecretKey = Guid.NewGuid().ToString();
            m_ReadingSecretKey = Guid.NewGuid().ToString();

            var (shoppingCart, store, checkOutManager) = GetStoreManagers();

            // do something cool
        }

        public (ProtectedShoppingCart ShoppingCart,
            ProtectedStore Store,
            ProtectedCheckOutManager CheckOutManager) GetStoreManagers()
        {
            var shoppingCart = new ShoppingCart();
            var protectedShoppingCart = new ProtectedShoppingCart(m_WritingSecretKey, m_ReadingSecretKey, shoppingCart);
            var store = new Store(shoppingCart);
            var protectedStore = new ProtectedStore(m_WritingSecretKey, store);
            var checkOutManager = new CheckOutManager(shoppingCart);
            var protectedCheckOutManager = new ProtectedCheckOutManager(m_ReadingSecretKey, checkOutManager);

            return (protectedShoppingCart, protectedStore, protectedCheckOutManager);
        }
    }
}