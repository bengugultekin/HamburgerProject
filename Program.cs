using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamburgerProject
{
    class Program
    {
        static void Main(string[] args)
        {
            IMenuFactory menuFactory = null;
            IPayment paymentMethod = null;
            Customer customer = new Customer();
            Console.Write("isminiz : ");
            string name = Console.ReadLine();
            customer.Name = name;
            Console.WriteLine("hoşgeldiniz " + customer.Name);
            Console.WriteLine("ne alırsınız?\n1.Big King Menu\n2. King Chicken Menu");
            char result = (char)Console.ReadKey(true).KeyChar;
            if (result == '1')
            {
                Console.WriteLine("Seçiminiz Big King Menu. Hazırlanıyor!");
                menuFactory = new BigKingMenuFactory();

            }
            else if (result == '2')
            {
                Console.WriteLine("\nSeçiminiz King Chicken Menu. Hazırlanıyor!");
                menuFactory = new KingChickenMenuFactory();
            }
            else
            {
                Console.WriteLine("hata sebebiyle işleme devam edilemiyor.");
                Console.Read();
                return;
            }
            Menu menu = (Menu)menuFactory;
            menu.Attach(customer);
            var price = customer.Order(menuFactory);
            Console.WriteLine("Ödemeniz gereken tutar : {0} TL", price);
            Console.WriteLine("ödemenizi nasıl yapmak istersiniz?");
            Console.WriteLine("1. Nakit olarak (%10 indirim)");
            Console.WriteLine("2. Kredi Kartı ile (%2 komisyon)");
            Console.WriteLine("3. Yemek Kuponu ile");
            result = Console.ReadKey(true).KeyChar;
            if (result == '1')
            {
                paymentMethod = new CashPayment();
            }
            else if (result == '2')
            {
                paymentMethod = new CreditCardPayment();
            }
            else if (result == '3')
            {
                paymentMethod = new TicketPayment();
            }
            else
            {
                Console.WriteLine("hata sebebiyle işleme devam edilemiyor.");
                Console.Read();
                return;
            }
            bool paymentIsOkay = customer.Pay(paymentMethod, price);
            if (paymentIsOkay)
            {
                menu.Notify();
            }

            Console.WriteLine("Teşekkürler ! Afiyet olsun...");
            Console.Read();
        }
    }

    // General Class for Inheritance
    public interface IProduct
    {
        decimal GetPrice();
    }

    // Product A inretface
    public interface IBurger : IProduct
    {
        string GetBurger();
    }

    // Product B interface
    public interface IDrink : IProduct
    {
        string GetDrink();
    }

    // Product C interface
    public interface IPotato : IProduct
    {
        string GetPotato();
    }

    // Product A1
    public class ChickenBurger : IBurger
    {
        public string GetBurger()
        {
            return "Chicken Burger";
        }

        public decimal GetPrice()
        {
            return 21.90m;
        }
    }

    // Product A2
    public class BeefBurger : IBurger
    {
        public string GetBurger()
        {
            return "Beef Burger";
        }

        public decimal GetPrice()
        {
            return 27.90m;
        }
    }

    // Product B1
    public class DrinkCola : IDrink
    {
        public string GetDrink()
        {
            return "Drink Cola";
        }

        public decimal GetPrice()
        {
            return 5.95m;
        }
    }

    // Product B2
    public class DrinkFanta : IDrink
    {
        public string GetDrink()
        {
            return "Drink Fanta";
        }

        public decimal GetPrice()
        {
            return 4.95m;
        }
    }

    // Product C1
    public class NormalSlicePotato : IPotato
    {
        public string GetPotato()
        {
            return "Normal Slice Potato";
        }

        public decimal GetPrice()
        {
            return 8.50m;
        }
    }

    //Product C2
    public class AppleSlicePotato : IPotato
    {
        public string GetPotato()
        {
            return "Apple Slice Potato";
        }

        public decimal GetPrice()
        {
            return 9.50m;
        }
    }

    // Abstract Factory
    public interface IMenuFactory
    {
        IBurger CreateBurger();
        IPotato CreatePotato();
        IDrink CreateDrink();
        decimal CreateMenu();
    }


    //Concrete Factory X
    public class KingChickenMenuFactory : Menu, IMenuFactory
    {
        public IBurger CreateBurger()
        {
            return new ChickenBurger();
        }

        public IDrink CreateDrink()
        {
            return new DrinkCola();
        }

        public IPotato CreatePotato()
        {
            return new NormalSlicePotato();
        }

        public decimal CreateMenu()
        {
            Name = "King Chicken Menu";
            Price = CreateBurger().GetPrice() + CreateDrink().GetPrice() + CreatePotato().GetPrice();
            return Price;
        }

    }


    //Concrete Factory Y
    public class BigKingMenuFactory : Menu, IMenuFactory
    {
        public IBurger CreateBurger()
        {
            return new BeefBurger();
        }

        public IDrink CreateDrink()
        {
            return new DrinkFanta();
        }

        public IPotato CreatePotato()
        {
            return new AppleSlicePotato();
        }

        public decimal CreateMenu()
        {
            Name = "Big King Menu";
            Price = CreateBurger().GetPrice() + CreateDrink().GetPrice() + CreatePotato().GetPrice();
            return Price;
        }
    }


    // Subject for Observer Pattern
    public abstract class Menu
    {
        public string Name;
        public decimal Price;
        public bool isReady;

        List<Observer> observers = new List<Observer>();
        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            observers.ForEach(observer => { observer.GetNotification(String.Format("{0} 'nüz hazırlanmıştır {1}, gelip alabilirsiniz! ",  Name, observer.Name)); });
        }
    }

    // Abstract Observer Class
    public abstract class Observer
    {
        public string Name;

        public abstract void GetNotification(string msg);

        public abstract decimal Order(IMenuFactory menu);

        public abstract bool Pay(IPayment paymentMethod, decimal price);
    }

    // Concrete Observer Class
    public class Customer : Observer
    {
        public override void GetNotification(string msg)
        {
            Console.WriteLine(msg);
        }

        public override decimal Order(IMenuFactory menu)
        {
            return menu.CreateMenu();
        }

        public override bool Pay(IPayment paymentMethod, decimal price)
        {
            return new PaymentOperation(paymentMethod, price).MakePayment();
        }
    }


    // Interface for Strategy Pattern
    public interface IPayment
    {
        bool MakePayment(decimal totalPrice);
    }


    // A Strategy for Payment
    public class CashPayment : IPayment
    {
        public bool MakePayment(decimal totalPrice)
        {
            Console.WriteLine("Nakit ödemelerde %10 indiriminiz bulunmaktadır!");
            totalPrice -= totalPrice * 0.1m;
            totalPrice = Math.Round(totalPrice, 2);
            Console.WriteLine(totalPrice + " TL Nakit Odeme Yapildi");
            return true;
        }
    }

    // A Strategy for Payment
    public class CreditCardPayment : IPayment
    {
        public bool MakePayment(decimal totalPrice)
        {
            Console.WriteLine("Kredi kartı ile ödemelerde %2 fark alınmaktadır!");
            totalPrice += totalPrice * 0.02m;
            totalPrice = Math.Round(totalPrice, 2);
            Console.WriteLine(totalPrice + " TL Kredi Kartiyla Odeme Yapildi");
            return true;
        }
    }

    // A Strategy for Payment
    public class TicketPayment : IPayment
    {
        public bool MakePayment(decimal totalPrice)
        {
            Console.WriteLine(totalPrice + " TL Yemek Kuponuyla Odeme Yapildi");
            return true;
        }
    }

    // Content Class for Payment Strategies Operation
    public class PaymentOperation
    {
        private IPayment _payment;
        private decimal _totalPrice;
        public PaymentOperation(IPayment _paymentMethod, decimal totalPrice)
        {
            this._payment = _paymentMethod;
            this._totalPrice = totalPrice;
        }

        public bool MakePayment()
        {
            return this._payment.MakePayment(_totalPrice);
        }
    }




}

