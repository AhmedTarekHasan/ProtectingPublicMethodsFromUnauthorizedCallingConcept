using System.Collections.Generic;

namespace ProtectingPublicMethodsFromUnauthorizedCalling.ShoppingCart
{
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

            // what stops the developer from doing this?
            // var itemsInCart = (m_StoreShoppingItems as IListShoppingItems).GetShoppingItems();
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
            // what stops the developer from doing this?
            // (m_ListShoppingItems as IStoreShoppingItems).AddShoppingItem("Product 8");

            return m_ListShoppingItems.GetShoppingItems();
        }
    }

    public class Program
    {
        public void Main()
        {
            var shoppingCart = new ShoppingCart();
            var store = new Store(shoppingCart);
            var checkOutManager = new CheckOutManager(shoppingCart);

            // do something cool
        }
    }
}