using System;
using System.Collections.Generic;

namespace ProtectingPublicMethodsFromUnauthorizedCalling.Method2.ProtectedSelfOwnedShoppingCart
{
    public interface IStoreShoppingItems
    {
        void AddShoppingItem(string writingSecretKey, string itemName);
    }

    public interface IListShoppingItems
    {
        IReadOnlyCollection<string> GetShoppingItems(string readingSecretKey);
    }

    public class ShoppingCart : IStoreShoppingItems, IListShoppingItems
    {
        private readonly List<string> m_ShoppingItems = new();
        private readonly string m_WritingSecretKey;
        private readonly string m_ReadingSecretKey;

        public ShoppingCart(string writingSecretKey, string readingSecretKey)
        {
            m_WritingSecretKey = writingSecretKey;
            m_ReadingSecretKey = readingSecretKey;
        }

        public void AddShoppingItem(string writingSecretKey, string itemName)
        {
            if (writingSecretKey != m_WritingSecretKey)
                throw new UnauthorizedAccessException("The provided writing secret key is wrong.");

            m_ShoppingItems.Add(itemName);
        }

        public IReadOnlyCollection<string> GetShoppingItems(string readingSecretKey)
        {
            if (readingSecretKey != m_ReadingSecretKey)
                throw new UnauthorizedAccessException("The provided reading secret key is wrong.");

            return m_ShoppingItems;
        }
    }

    public class Store
    {
        private readonly IStoreShoppingItems m_StoreShoppingItems;
        private readonly List<string> m_AllStoreProducts;
        private readonly string m_WritingSecretKey;

        public Store(string writingSecretKey, IStoreShoppingItems storeShoppingItems)
        {
            m_WritingSecretKey = writingSecretKey;

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
            m_StoreShoppingItems.AddShoppingItem(m_WritingSecretKey, product);

            // The developer will not be able to do this as he doesn't have the reading secret key
            // var itemsInCart = (m_StoreShoppingItems as IListShoppingItems).GetShoppingItems(???);
        }
    }

    public class CheckOutManager
    {
        private readonly IListShoppingItems m_ListShoppingItems;
        private readonly string m_ReadingSecretKey;

        public CheckOutManager(string readingSecretKey, IListShoppingItems listShoppingItems)
        {
            m_ReadingSecretKey = readingSecretKey;
            m_ListShoppingItems = listShoppingItems;
        }

        public IReadOnlyCollection<string> GetCheckoutItems()
        {
            // The developer will not be able to do this as he doesn't have the writing secret key
            // (m_ListShoppingItems as IStoreShoppingItems).AddShoppingItem(???, "Product 8");

            return m_ListShoppingItems.GetShoppingItems(m_ReadingSecretKey);
        }
    }

    public class Program
    {
        private string m_WritingSecretKey;
        private string m_ReadingSecretKey;

        public void Main()
        {
            m_WritingSecretKey = Guid.NewGuid().ToString();
            m_ReadingSecretKey = Guid.NewGuid().ToString();

            var shoppingCart = new ShoppingCart(m_WritingSecretKey, m_ReadingSecretKey);
            var store = new Store(m_WritingSecretKey, shoppingCart);
            var checkOutManager = new CheckOutManager(m_ReadingSecretKey, shoppingCart);

            // do something cool
        }
    }
}