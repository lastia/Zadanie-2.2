using System.Xml;

/// <summary>
/// Класс представляет товар в заказе
/// </summary>
public class Item
{
    /// <summary>
    /// Наименование товара
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Количество товара
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Цена товара
    /// </summary>
    public decimal Price { get; set; }
}

/// <summary>
/// Класс представляет заказ со списком товаров и информацией о доставке
/// </summary>
public class Order
{
    /// <summary>
    /// Класс представляет информацию о доставке
    /// </summary>
    public class ShipTo
    {
        /// <summary>
        /// Имя получателя заказа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Улица доставки
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Страна доставки
        /// </summary>
        public string Country { get; set; }
    }

    /// <summary>
    /// Информация о доставке
    /// </summary>
    public ShipTo ShipInfo { get; set; }

    /// <summary>
    /// Список товаров в заказе
    /// </summary>
    public List<Item> Items { get; set; }
}

/// <summary>
/// Статический класс для преобразования XML-документа в объекты классов Order и Item
/// </summary>
public static class XmlToOrderConverter
{
    /// <summary>
    /// Метод выполняет преобразование XML-строки в объект класса Order
    /// </summary>
    /// <param name="xmlString">XML-строка, содержащая информацию о заказе</param>
    /// <returns>Объект класса Order, представляющий заказ</returns>
    public static Order Convert(string xmlString)
    {
        Order order = new Order();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);

        /// Обработка информации о доставке ///
        XmlNode shipToNode = xmlDoc.SelectSingleNode("/shipOrder/shipTo");
        if (shipToNode != null)
        {
            order.ShipInfo = new Order.ShipTo
            {
                Name = GetInnerText(shipToNode, "name"),
                Street = GetInnerText(shipToNode, "street"),
                Address = GetInnerText(shipToNode, "address"),
                Country = GetInnerText(shipToNode, "country")
            };
        }

        /// Обработка списка товаров ///
        order.Items = new List<Item>();
        XmlNodeList itemNodes = xmlDoc.SelectNodes("/shipOrder/items/item");
        foreach (XmlNode itemNode in itemNodes)
        {
            Item item = new Item
            {
                Title = GetInnerText(itemNode, "title"),
                Quantity = GetIntValue(itemNode, "quantity"),
                Price = GetDecimalValue(itemNode, "price")
            };
            order.Items.Add(item);
        }

        return order;
    }

    /// Вспомогательный метод для получения текстового содержимого дочернего узла ///
    private static string GetInnerText(XmlNode parent, string childNodeName)
    {
        XmlNode childNode = parent.SelectSingleNode(childNodeName);
        return childNode != null ? childNode.InnerText : null;
    }

    /// Вспомогательный метод для получения целочисленного значения дочернего узла ///
    private static int GetIntValue(XmlNode parent, string childNodeName)
    {
        XmlNode childNode = parent.SelectSingleNode(childNodeName);
        return childNode != null && int.TryParse(childNode.InnerText, out int value) ? value : 0;
    }

    /// Вспомогательный метод для получения десятичного значения дочернего узла ///
    private static decimal GetDecimalValue(XmlNode parent, string childNodeName)
    {
        XmlNode childNode = parent.SelectSingleNode(childNodeName);
        return childNode != null && decimal.TryParse(childNode.InnerText.Replace('.', ','), out decimal value) ? value : 0.0m;
    }
}

/// <summary>
/// Класс для выполнения лабораторной работы 2.2
/// </summary>>
class Lab2_2
{
    /// <summary>
    /// Точка входа в программу
    /// </summary>
    static void Main()
    {
        string xmlString = @"
            <shipOrder>
                <shipTo>
                    <name>Tove Svendson</name>
                    <street>Ragnhildvei 2</street>
                    <address>4000 Stavanger</address>
                    <country>Norway</country>
                </shipTo>
                <items>
                    <item>
                        <title>Empire Burlesque</title>
                        <quantity>1</quantity>
                        <price>10.90</price>
                    </item>
                    <item>
                        <title>Hide your heart</title>
                        <quantity>1</quantity>
                        <price>9.90</price>
                    </item>
                </items>
            </shipOrder>";

        Order order = XmlToOrderConverter.Convert(xmlString);

        /// Пример использования данных ///
        Console.WriteLine("Информация о доставке:");
        Console.WriteLine($"Имя: {order.ShipInfo.Name}");
        Console.WriteLine($"Улица: {order.ShipInfo.Street}");
        Console.WriteLine($"Адрес: {order.ShipInfo.Address}");
        Console.WriteLine($"Страна: {order.ShipInfo.Country}");

        Console.WriteLine("\nТовары в заказе:");
        foreach (Item item in order.Items)
        {
            Console.WriteLine($"Наименование: {item.Title}, Количество: {item.Quantity}, Цена: {item.Price:C}");
        }
    }
}
