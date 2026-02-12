using System;

namespace Bunbarlang
{
    public class Blackjack : IGame
    {
        public string Name => "Blackjack";
        public bool IsCompleted { get; private set; }

        public void Play(Player player)
        {
            Console.Clear();
            Console.WriteLine("===== BLACKJACK =====");
            Console.WriteLine($"Chips: {player.Chips}");

            Console.Write("Enter bet: ");
            int bet = int.Parse(Console.ReadLine());

            if (!player.RemoveChips(bet))
            {
                Console.WriteLine("Not enough chips!");
                Console.ReadKey();
                return;
            }

            Deck deck = new Deck();
            deck.Shuffle();

            BlackjackHand playerHand = new BlackjackHand();
            BlackjackHand dealerHand = new BlackjackHand();

            playerHand.AddCard(deck.Draw());
            playerHand.AddCard(deck.Draw());

            dealerHand.AddCard(deck.Draw());
            dealerHand.AddCard(deck.Draw());

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Dealer shows: " + dealerHand.Cards[0]);
                Console.WriteLine("Your hand: " + playerHand);

                if (playerHand.IsBust)
                {
                    Console.WriteLine("Bust!");
                    break;
                }

                Console.Write("Hit or Stand (h/s): ");
                string input = Console.ReadLine();

                if (input == "h")
                    playerHand.AddCard(deck.Draw());
                else
                    break;
            }

            while (dealerHand.GetValue() < 17)
                dealerHand.AddCard(deck.Draw());

            Console.WriteLine("\nDealer: " + dealerHand);
            Console.WriteLine("Player: " + playerHand);

            if (playerHand.IsBust)
            {
                Console.WriteLine("Dealer wins!");
            }
            else if (dealerHand.IsBust || playerHand.GetValue() > dealerHand.GetValue())
            {
                Console.WriteLine("You win!");
                player.AddChips(bet * 2);
            }
            else if (playerHand.GetValue() == dealerHand.GetValue())
            {
                Console.WriteLine("Push!");
                player.AddChips(bet);
            }
            else
            {
                Console.WriteLine("Dealer wins!");
            }

            IsCompleted = true;
            Console.ReadKey();
        }
    }
}
