using System;
using System.Collections.Generic;
using System.Linq;

namespace Bunbarlang
{
    public class Blackjack : IGame
    {
        public string Name => "Blackjack";
        public bool IsCompleted { get; private set; }

        public void Play(Player player)
        {
            if (player is null) throw new ArgumentNullException(nameof(player));

            IsCompleted = false;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== BLACKJACK =====");
                Console.WriteLine($"Chips: {player.Chips:N0}");
                Console.WriteLine();

                Console.Write("Enter bet: ");
                string? betInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(betInput) || !int.TryParse(betInput, out int bet) || bet <= 0)
                {
                    Console.WriteLine("Invalid bet.");
                    Console.WriteLine("Press ESC to exit or any other key to try again...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        IsCompleted = true;
                        Console.CursorVisible = true;
                        return;
                    }
                    continue;
                }

                if (!player.RemoveChips(bet))
                {
                    Console.WriteLine("Not enough chips!");
                    Console.WriteLine("Press ESC to exit or any other key to try again...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        IsCompleted = true;
                        Console.CursorVisible = true;
                        return;
                    }
                    continue;
                }

                // Setup round
                var deck = new Deck();
                deck.Shuffle();

                var playerHand = new BlackjackHand();
                var dealerHand = new BlackjackHand();

                playerHand.AddCard(deck.Draw());
                playerHand.AddCard(deck.Draw());

                dealerHand.AddCard(deck.Draw());
                dealerHand.AddCard(deck.Draw());

                bool playerTurn = true;
                bool revealDealer = false;

                // ===== PLAYER TURN =====
                while (playerTurn)
                {
                    Render(player, bet, playerHand, dealerHand, revealDealer);

                    if (playerHand.IsBust)
                    {
                        Console.WriteLine("\n💥 Bust!");
                        break;
                    }

                    Console.Write("\n(H)it or (S)tand: ");
                    string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

                    if (input == "h")
                    {
                        playerHand.AddCard(deck.Draw());
                    }
                    else if (input == "s")
                    {
                        playerTurn = false;
                    }
                }

                // ===== DEALER TURN =====
                revealDealer = true;

                while (!playerHand.IsBust && dealerShouldHit(dealerHand))
                {
                    Render(player, bet, playerHand, dealerHand, revealDealer);
                    System.Threading.Thread.Sleep(800);
                    dealerHand.AddCard(deck.Draw());
                }

                Render(player, bet, playerHand, dealerHand, revealDealer);

                // ===== RESULT =====
                Resolve(player, bet, playerHand, dealerHand);

                if (player.Chips <= 0)
                {
                    Console.WriteLine("\nYou're out of chips. Press any key to return to lobby...");
                    Console.ReadKey(true);
                    IsCompleted = true;
                    Console.CursorVisible = true;
                    return;
                }

                Console.WriteLine("\nPress ESC to exit or any other key to play again...");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    IsCompleted = true;
                    Console.CursorVisible = true;
                    return;
                }
            }
        }

        private bool dealerShouldHit(BlackjackHand hand)
        {
            int value = hand.GetValue();

            // Dealer stands on soft 17 (simplified)
            return value < 17;
        }

        private void Resolve(Player player, int bet,
            BlackjackHand playerHand,
            BlackjackHand dealerHand)
        {
            Console.WriteLine();

            if (playerHand.IsBust)
            {
                Console.WriteLine("Dealer wins!");
                return;
            }

            if (dealerHand.IsBust)
            {
                Console.WriteLine("Dealer busts! You win!");
                player.AddChips(bet * 2);
                return;
            }

            int playerValue = playerHand.GetValue();
            int dealerValue = dealerHand.GetValue();

            // Natural Blackjack (3:2)
            if (playerValue == 21 && playerHand.Cards.Count == 2)
            {
                Console.WriteLine("BLACKJACK! 🃏");
                player.AddChips((int)(bet * 2.5));
                return;
            }

            if (playerValue > dealerValue)
            {
                Console.WriteLine("You win!");
                player.AddChips(bet * 2);
            }
            else if (playerValue == dealerValue)
            {
                Console.WriteLine("Push!");
                player.AddChips(bet);
            }
            else
            {
                Console.WriteLine("Dealer wins!");
            }
        }

        // =========================
        // RENDER SYSTEM
        // =========================

        private void Render(Player player,
            int bet,
            BlackjackHand playerHand,
            BlackjackHand dealerHand,
            bool revealDealer)
        {
            Console.Clear();

            Console.WriteLine("===== BLACKJACK =====");
            Console.WriteLine($"Chips: {player.Chips:N0}");
            Console.WriteLine($"Bet: {bet}");
            Console.WriteLine();

            Console.WriteLine("Dealer:");

            if (!revealDealer)
            {
                if (dealerHand.Cards.Count > 0)
                    RenderCards(new List<Card> { dealerHand.Cards[0] });
                RenderHiddenCard();
            }
            else
            {
                RenderCards(dealerHand.Cards);
                Console.WriteLine($"Value: {dealerHand.GetValue()}");
            }

            Console.WriteLine("\nYour Hand:");
            RenderCards(playerHand.Cards);
            Console.WriteLine($"Value: {playerHand.GetValue()}");
        }

        // =========================
        // ASCII CARD SPRITES
        // =========================

        private void RenderCards(List<Card> cards)
        {
            if (cards is null || cards.Count == 0) return;

            var sprites = cards.Select(BuildCardSprite).ToList();

            for (int i = 0; i < sprites[0].Length; i++)
            {
                foreach (var s in sprites)
                    Console.Write(s[i] + "  ");
                Console.WriteLine();
            }
        }

        private void RenderHiddenCard()
        {
            var hidden = new[]
            {
                "┌─────────┐",
                "|░░░░░░░░░|",
                "|░░░░░░░░░|",
                "|░░░░░░░░░|",
                "|░░░░░░░░░|",
                "|░░░░░░░░░|",
                "└─────────┘"
            };

            foreach (var line in hidden)
                Console.WriteLine(line);
        }

        private string[] BuildCardSprite(Card card)
        {
            string rank = GetRank(card);
            string suit = GetSuit(card);

            return new[]
            {
                "┌─────────┐",
                $"|{rank,-2}       |",
                "|         |",
                $"|    {suit}    |",
                "|         |",
                $"|       {rank,2}|",
                "└─────────┘"
            };
        }

        private string GetSuit(Card c)
        {
            return c.Suit switch
            {
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                Suit.Spades => "♠",
                _ => "?"
            };
        }

        private string GetRank(Card c)
        {
            return c.Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                Rank.Ten => "10",
                _ => ((int)c.Rank).ToString()
            };
        }
    }
}
