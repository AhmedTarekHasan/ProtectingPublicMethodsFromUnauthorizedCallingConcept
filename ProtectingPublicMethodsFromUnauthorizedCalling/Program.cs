using System;

namespace ProtectingPublicMethodsFromUnauthorizedCalling
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj = new ReadWriteClass();
            Func1(obj);
            Func2(obj);

            Console.ReadLine();
        }

        public static void Func1(IRead read)
        {
            (read as IWrite).Write();
        }

        public static void Func2(IWrite write)
        {
            (write as IRead).Read();
        }
    }

    public interface IRead
    {
        void Read();
    }

    public interface IWrite
    {
        void Write();
    }

    public class ReadClass : IRead
    {
        public void Read()
        {
            Console.WriteLine("Read");
        }
    }

    public class WriteClass : IWrite
    {
        public void Write()
        {
            Console.WriteLine("Write");
        }
    }

    public class ReadWriteClass : IRead, IWrite
    {
        public void Read()
        {
            Console.WriteLine("Read");
        }

        public void Write()
        {
            Console.WriteLine("Write");
        }
    }
}