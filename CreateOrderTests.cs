using NUnit.Framework;
using SpiceShop.Enums;
using SpiceShop.Helpers;
using System;
using SpiceShop.Util;
using SpiceShop.Models;

namespace SpiceShop;

public class CreateOrderTests : SpicesShopTestBase
{
    //Быстрый общий позитивный тест с валидными входными данными

    [Test, Category("Positive")]
    [TestCase("Shafran", UnitType.Grams, 1000, 1000, "Vadim Volkov")]
    [TestCase("Базалик", UnitType.Pieces, 1000, 1, "Вадим Волков")]
    [TestCase("Кориандр", UnitType.Grams, 1000, 100, "Майкл де Санта")]
    [TestCase("Кориандр", UnitType.Pieces, 1000, 10, "I Dont Know My Name")]
    public void CreateOrder_Smoke(string SpiceName, UnitType UnitType, int Volume, int OrderQuantity, string CustomerName)
    {
        // arrange
        var spiceId = Client.Spices.AddSpice(SpiceName, UnitType, Volume);
        // act
        var orderId = Client.Orders.CreateOrder(spiceId, OrderQuantity, CustomerName);
        // assert
        Assert.IsNotNull(orderId); // заказ успешно создан

        //output
        Console.WriteLine($"Created SpiceId: {spiceId}.");
        Console.WriteLine($"Created OrderId: {orderId}.");
        Console.WriteLine($"Order: {CustomerName} | {spiceId} ({SpiceName}) | {OrderQuantity}/{Volume} {UnitType}");
    }


    //Детальные тесты по категориям

    [Test, Category("Negative"), Category("Customer Name")]
    [TestCase(null)] // проверка на null
    [TestCase("")] // пустая строка
    [TestCase("   ")] // строка из нескольких пробелов подряд
    [TestCase("va")] // меньше символов чем допустимо min = 3
    [TestCase("VeryLongNameVeryLongName")] // больше символов чем допустимо max = 20
    [TestCase("123")] // цифры строкой
    [TestCase("!№;%:?*()_")] // различные спецсимволы
    [TestCase("Вад  им Волков")] //наличие двойного пробела (два пробела рядом)
    [TestCase(" Вадим")] // начало строки с пробела
    [TestCase("    Вадим")] // начало строки с табуляции. \t - отсекается из за наличия символа
    [TestCase("Ваdиm")] // смесь алфавитов в одной строке
    [TestCase("Вадим ")] // окончание строки пробелом
    [TestCase("Вадим Волков!")] // наличие одного спецсимвола
    [TestCase("Вадим 🙂")] // наличие смайла unicode
    [TestCase("바디미 볼코프")] // наличие символов не относящегося к кирилице и латинице (корейский)
    [TestCase("ভ্যাডিম ভলকভ")] // наличие символов не относящегося к кирилице и латинице (бенгальский)
    [TestCase("וואַדים וואָלקאָוו")] // наличие символов не относящегося к кирилице и латинице (идиш)
    [TestCase("ヴァディム・ヴォルコフ")] // наличие символов не относящегося к кирилице и латинице (японский)

    public void CreateOrder_WithInvalidCustomerName(string customerName)
    {
        // arrange
        var spiceName = "Петрушка";
        var unitType = UnitType.Grams;
        var volume = int.MaxValue;
        var orderQuantity = 200;

        var spiceId = Client.Spices.AddSpice(spiceName, unitType, volume);

        // act & assert

        var ex = Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, customerName));
        Console.WriteLine(ex);
    }

    [Test, Category("Negative"), Category("Order Quantity")]
    [TestCase(null, UnitType.Grams)]
    [TestCase(null, UnitType.Pieces)]
    [TestCase(0, UnitType.Grams)]
    [TestCase(0, UnitType.Pieces)]
    [TestCase(-5, UnitType.Grams)]
    [TestCase(-5, UnitType.Pieces)]
    [TestCase(9, UnitType.Grams)]
    [TestCase(1001, UnitType.Grams)]
    [TestCase(11, UnitType.Pieces)]
    [TestCase(int.MinValue, UnitType.Grams)]
    [TestCase(int.MinValue, UnitType.Pieces)]
    [TestCase(int.MaxValue, UnitType.Grams)]
    [TestCase(int.MaxValue, UnitType.Pieces)]
    [TestCase(int.MaxValue, UnitType.Grams)]
    [TestCase(int.MaxValue, UnitType.Pieces)]
    //Проверка на переполнение типа делается на уровне IDE

    public void TestCreateOrder_Negative_InvalidOrderQuantityTowardsUnitType(int orderQuantity, UnitType unitType)
    {
        // Arrange
        var spiceId = Client.Spices.AddSpice("Перец", unitType, int.MaxValue);

        // Act & Assert
        var ex = Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, "Вадим Волков"));
        Console.WriteLine(ex);

    }


    [Test, Category("Negative"), Category("Order Quantity")]
    [TestCase(501, UnitType.Grams, 500)] // +1 к верхней границе обьема в граммах
    [TestCase(100, UnitType.Grams, 0)] // объем 0, но в заказе валидное значение в граммах
    [TestCase(5, UnitType.Pieces, 4)] // +1 к верхней границе обьема в штуках
    [TestCase(5, UnitType.Pieces, 0)] // объем 0, но в заказе валидное значение в штуках

    public void TestCreateOrder_Negative_InvalidOrderQuantityTowardsVolumeOfUnitType(int orderQuantity, UnitType unitType, int volume)
    {
        // Arrange
        var spiceId = Client.Spices.AddSpice("Перец", unitType, volume);

        // Act & Assert
        var ex = Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, "Вадим Волков"));
        Console.WriteLine(ex);

    }


    [Test, Category("Positive"), Category("Order Quantity")]
    [TestCase(1000, UnitType.Grams, 1000)] // 1 к 1 по верхней границе в граммах
    [TestCase(558, UnitType.Grams, 768)] // среднее в граммах
    [TestCase(10, UnitType.Grams, 10)] // 1 к 1 по нижней границе в граммах
    [TestCase(5, UnitType.Pieces, 5)]  // 1 к 1 по верхней границе в штуках
    [TestCase(2, UnitType.Pieces, 4)] // среднее в штуках
    [TestCase(1, UnitType.Pieces, 1)]  // 1 к 1 по нижней границе в штуках

    public void TestCreateOrder_Negative_ValidOrderQuantityTowardsVolumeOfUnitType(int orderQuantity, UnitType unitType, int volume)
    {
        // Arrange
        var spiceId = Client.Spices.AddSpice("Перец", unitType, volume);

        // act
        var orderId = Client.Orders.CreateOrder(spiceId, orderQuantity, "Customer");
        // assert
        Assert.IsNotNull(orderId); // заказ успешно создан

        //output
        Console.WriteLine($"Created SpiceId: {spiceId}.");
        Console.WriteLine($"Created OrderId: {orderId}.");
        Console.WriteLine($"Order:  | {spiceId} | {orderQuantity}/{volume} {unitType}");
    }

    [Test, Category("Negative"), Category("SpiceId")]
    public void TestCreateOrder_Negative_InvalidSpiceId()
    {
        // Arrange
        var randomNewGuid = new Guid(Guid.NewGuid().ToString());
        // Act & Assert
        var ex = Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(randomNewGuid, 10, "Customer"));
        Console.WriteLine(ex);

    }





















    /*

        [Test, Category("Exception")]
        public void CreateOrder_WithShortCustomerName()
        {
            // arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);
            var invalidCustomerName = "Ив";

            // act
            TestDelegate createOrderAction = () => Client.Orders.CreateOrder(spiceId, 10, invalidCustomerName);

            // assert
            Assert.Throws<SpiceShopException>(createOrderAction);
        }
        [Test, Category("Exception")]
        public void CreateOrder_WithLongCustomerName()
        {
            // arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);
            var invalidCustomerName = "Ивафвывфывфывфывфывфвыфвфыввфывфывфывфв";

            // act
            TestDelegate createOrderAction = () => Client.Orders.CreateOrder(spiceId, 10, invalidCustomerName);

            // assert
            Assert.Throws<SpiceShopException>(createOrderAction);
        }


        [Test, Category("Exception")]
        [TestCase("")]
        [TestCase(null)]
        public void CreateOrder_CustomerNameIsNullOrEmpty(string invalidCustomerName)
        {
            // arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);

            // act
            TestDelegate createOrderAction = () => Client.Orders.CreateOrder(spiceId, 10, invalidCustomerName);

            // assert
            Assert.Throws<SpiceShopException>(createOrderAction);
        }
        [Test, Category("Exception")]
        [TestCase("!@#$%^&**()_=+-")]

        public void CreateOrder_CustomerNameIsSymbols(string invalidCustomerName)
        {
            // arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);

            // act
            TestDelegate createOrderAction = () => Client.Orders.CreateOrder(spiceId, 10, invalidCustomerName);

            // assert
            Assert.Throws<SpiceShopException>(createOrderAction);
        }



        [Test]
        public void CreateOrder_InvalidSpiceId_Failure()
        {
            // arrange
            var invalidSpiceId = Guid.Parse("invalid_id");
            var customerName = "John Doe";
            var orderQuantity = 500;

            // act and assert
            TestDelegate testDelegate = () => Client.Orders.CreateOrder(invalidSpiceId, orderQuantity, customerName);
            Assert.Throws<FormatException>(testDelegate);
        }

        [Test]
        public void CreateOrder_InvalidCustomerName_TooShort_Failure()
        {
            // Arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);
            var customerName = "Jo"; // Too short name
            var orderQuantity = 500;

            // Act & Assert
            Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, customerName));
        }

        [Test]
        public void CreateOrder_InvalidCustomerName_TooLong_Failure()
        {
            // Arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);
            var customerName = "John Doe John Doe John Doe"; // Too long name
            var orderQuantity = 500;

            // Act & Assert
            Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, customerName));
        }

        [Test]
        public void CreateOrder_InvalidQuantity_TooHigh_Failure()
        {
            // Arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Grams, 100);
            var customerName = "John Doe";
            var orderQuantity = 2000; // Too high quantity

            // Act & Assert
            Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, customerName));
        }

        [Test]
        public void CreateOrder_InvalidQuantity_TooLow_Failure()
        {
            // Arrange
            var spiceId = Client.Spices.AddSpice("Перец", UnitType.Pieces, 10); // Piece type
            var customerName = "John Doe";
            var orderQuantity = 0; // Too low quantity

            // Act & Assert
            Assert.Throws<SpiceShopException>(() => Client.Orders.CreateOrder(spiceId, orderQuantity, customerName));
        }*/
}