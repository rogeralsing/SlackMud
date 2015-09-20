﻿using Akka.Actor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SlackMud
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("Mud"))
            {
                var output = system.ActorOf<OutputActor>();
                var room1 = system.ActorOf(Props.Create(() => new Thing("the kitchen")));
                var sword = system.ActorOf(Props.Create(() => new Weapon("a sword")));
                var backpack = system.ActorOf(Props.Create(() => new Thing("a backpack")));
                var player1 = system.ActorOf(Props.Create(() => new Player("Allan")));
                var player2 = system.ActorOf(Props.Create(() => new Player("Åke")));
                var goblin = system.ActorOf(Props.Create(() => new Goblin()));
                player1.Tell(new SetContainer(room1));
                player2.Tell(new SetContainer(room1));
                goblin.Tell(new SetContainer(room1));
                sword.Tell(new SetContainer(room1));
                backpack.Tell(new SetContainer(player1));
                player1.Tell(new SetOutput(output));
          //      player2.Tell(new SetOutput(output));
                Run(player1).Wait();
            }
        }

        private static async Task Run(IActorRef player1)
        {
            Console.WriteLine("type; look, where, name, inventory");
            while (true)
            {
                var input = Console.ReadLine();
                var parts = input.Split(new char[] {' '} , 2, StringSplitOptions.RemoveEmptyEntries);
                var command = parts.FirstOrDefault();
                switch (command)
                {
                    case "say":
                    case "s":
                        {
                            var message = parts.ElementAtOrDefault(1);
                            player1.Tell(new Say(message));
                            break;
                        }
                    case "where":
                        {
                            player1.Tell(new Where());
                            break;
                        }
                    case "name":
                        {
                            var name = await player1.Ask<string>(new GetName());
                            Console.WriteLine($"Your name is {name}.");
                            break;
                        }
                        
                    case "look":
                    case "l":
                        {
                            player1.Tell(new Look());
                            break;
                        }
                    case "inventory":
                    case "inv":
                        {
                            player1.Tell(new Inventory());
                            break;
                        }
                    case "take":
                    case "t":
                        {
                            var name = parts.ElementAtOrDefault(1);
                            player1.Tell(new Take(name));
                            break;
                        }
                    case "drop":
                    case "d":
                        {
                            var name = parts.ElementAtOrDefault(1);
                            player1.Tell(new Drop(name));
                            break;
                        }

                }
            }
        }       
    }
}
