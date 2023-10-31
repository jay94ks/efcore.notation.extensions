using Efcore.Notation.Extensions.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Efcore.Notations.Extensions.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var Context = new ProgramContext();

            if (args.Length > 0)
            {
                return;
            }

            Context.Database.Migrate();

            var Set = Context.Set<SomeEntityA>();

            if (Set.Count() < 5)
            {
                var Algorithm = Password.Algorithm.All
                    .OrderBy(_ => Guid.NewGuid())
                    .First();

                Set.Add(new SomeEntityA
                {
                    Guid = Guid.NewGuid(),
                    Number = (int)(DateTime.Now.Ticks % int.MaxValue),
                    RemoteAddress = IPAddress.Loopback,
                    Text = "Hello World",
                    Password = Password.Make(Guid.NewGuid().ToString(), Algorithm)
                });

                Context.SaveChanges();
            }

            foreach (var Each in Set)
            {
                Console.WriteLine($"Key: {Each.Guid}, {Each.Number}");
                Console.WriteLine($"Remote Address: {Each.RemoteAddress}");
                Console.WriteLine($"Text: {Each.Text}");
                Console.WriteLine($"Password (Hash): {Each.Password}");
            }

        }
    }
}