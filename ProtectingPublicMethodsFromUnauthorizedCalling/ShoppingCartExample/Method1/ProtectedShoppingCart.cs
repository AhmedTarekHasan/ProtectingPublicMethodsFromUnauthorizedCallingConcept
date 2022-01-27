using System.Collections.Generic;

namespace ProtectingPublicMethodsFromUnauthorizedCalling.Method1.ProtectedShoppingCart
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

            // If the developer does this, an exception would be thrown as the passed in m_ListShoppingItems is not IListShoppingItems
            // var itemsInCart = (m_StoreShoppingItems as IListShoppingItems).GetShoppingItems(???);
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
            // If the developer does this, an exception would be thrown as the passed in m_ListShoppingItems is not IStoreShoppingItems
            // (m_ListShoppingItems as IStoreShoppingItems).AddShoppingItem(???, "Product 8");

            return m_ListShoppingItems.GetShoppingItems();
        }
    }


    #region Added These
    public class ReadShoppingCart : IListShoppingItems
    {
        private readonly ShoppingCart m_ShoppingCart;

        public ReadShoppingCart(ShoppingCart shoppingCart)
        {
            m_ShoppingCart = shoppingCart;
        }

        public IReadOnlyCollection<string> GetShoppingItems()
        {
            return m_ShoppingCart.GetShoppingItems();
        }
    }

    public class WriteShoppingCart : IStoreShoppingItems
    {
        private readonly ShoppingCart m_ShoppingCart;

        public WriteShoppingCart(ShoppingCart shoppingCart)
        {
            m_ShoppingCart = shoppingCart;
        }

        public void AddShoppingItem(string itemName)
        {
            m_ShoppingCart.AddShoppingItem(itemName);
        }
    }
    #endregion


    public class Program
    {
        public void Main()
        {
            var shoppingCart = new ShoppingCart();
            var writeShoppingCart = new WriteShoppingCart(shoppingCart);
            var store = new Store(writeShoppingCart);
            var readShoppingCart = new ReadShoppingCart(shoppingCart);
            var checkOutManager = new CheckOutManager(readShoppingCart);

            // do something cool
        }
    }
}