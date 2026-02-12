using System;
using System.Collections.Generic;
using System.Linq;

namespace Bunbarlang
{
    public class Poker : IGame
    {
        private Deck deck;
        private List<Card> playerCards;
        private List<Card> dealerCards;
        private List<Card> community;

        private int pot;
        private int currentBet;
        private int playerBet;
        private int dealerBet;

        private int playerChips;
        private int dealerChips;

        private bool playerFolded;
        private bool dealerFolded;

        private int smallBlind = 25;
        private int bigBlind = 50;

        private Random rng = new Random();

        private bool abortRequested;

        public string Name => "Poker";
        public bool IsCompleted => playerChips <= 0 || dealerChips <= 0;

        public void Play(Player player)
        {
            StartGame(player.Chips);

            player.Chips = playerChips;
        }

        public void StartGame(int startingChips)
        {
            playerChips = startingChips;
            dealerChips = startingChips;
            abortRequested = false;

            while (playerChips > 0 && dealerChips > 0 && !abortRequested)
            {
                PlayRound();
            }

            Console.Clear();
            if (abortRequested)
            {
                Console.WriteLine("Returning to lobby...");
            }
            else
            {
                Console.WriteLine(playerChips > 0 ? "YOU WIN THE MATCH!" : "DEALER WINS THE MATCH!");
                Console.ReadKey();
            }
        }

        private void PlayRound()
        {
            ResetRound();

            deck = new Deck();
            deck.Shuffle();

            playerCards = new List<Card> { deck.Draw(), deck.Draw() };
            dealerCards = new List<Card> { deck.Draw(), deck.Draw() };
            community = new List<Card>();

            PostBlinds();

            BettingRound("Pre-Flop");
            if (RoundEnded() || abortRequested) return;

            Burn();
            community.AddRange(new[] { deck.Draw(), deck.Draw(), deck.Draw() });
            BettingRound("Flop");
            if (RoundEnded() || abortRequested) return;

            Burn();
            community.Add(deck.Draw());
            BettingRound("Turn");
            if (RoundEnded() || abortRequested) return;

            Burn();
            community.Add(deck.Draw());
            BettingRound("River");
            if (RoundEnded() || abortRequested) return;

            Showdown();
        }

        private void ResetRound()
        {
            pot = 0;
            currentBet = 0;
            playerBet = 0;
            dealerBet = 0;
            playerFolded = false;
            dealerFolded = false;
        }

        private void PostBlinds()
        {
            playerChips -= smallBlind;
            dealerChips -= bigBlind;

            playerBet = smallBlind;
            dealerBet = bigBlind;

            pot = smallBlind + bigBlind;
            currentBet = bigBlind;
        }

        private void BettingRound(string stage)
        {
            bool betting = true;

            while (betting && !abortRequested)
            {
                Render(stage);

                PlayerAction();
                if (abortRequested || RoundEnded()) return;

                DealerAction(stage);
                if (abortRequested || RoundEnded()) return;

                if (playerBet == dealerBet)
                    betting = false;
            }

            playerBet = 0;
            dealerBet = 0;
            currentBet = 0;
        }

        private void PlayerAction()
        {
            int callAmount = currentBet - playerBet;

            string[] options = {
                callAmount == 0 ? "Check" : $"Call ({callAmount})",
                "Raise",
                "Fold"
            };

            int selected = 0;

            for (int i = 0; i < options.Length + 2; i++)
                Console.WriteLine();

            int menuTop = Console.CursorTop - (options.Length + 2);
            if (menuTop < 0) menuTop = 0;

            bool chosen = false;
            while (!chosen)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    int targetTop = Math.Clamp(menuTop + i, 0, Console.BufferHeight - 1);
                    Console.SetCursorPosition(0, targetTop);
                    Console.Write(new string(' ', Math.Max(Console.WindowWidth - 1, 1)));
                    Console.SetCursorPosition(0, targetTop);

                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"  {options[i]}");
                    }
                }

                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + options.Length) % options.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % options.Length;
                        break;
                    case ConsoleKey.Enter:
                        chosen = true;
                        break;
                    case ConsoleKey.Escape:
                        // New: request abort to return to main menu
                        abortRequested = true;
                        return;
                    default:
                        break;
                }
            }

            int below = Math.Clamp(menuTop + options.Length + 1, 0, Console.BufferHeight - 1);
            Console.SetCursorPosition(0, below);

            if (selected == 0) 
            {
                Bet(ref playerChips, ref playerBet, callAmount);
            }
            else if (selected == 1) 
            {
                Console.Write("Raise amount: ");
                if (!int.TryParse(Console.ReadLine(), out int raise) || raise < 0)
                    raise = 0;

                int total = callAmount + raise;
                Bet(ref playerChips, ref playerBet, total);
                currentBet = playerBet;
            }
            else 
            {
                playerFolded = true;
            }
        }

        private void DealerAction(string stage)
        {
            if (playerFolded) return;

            int strength = EvaluateStrength(dealerCards.Concat(community).ToList());
            int callAmount = currentBet - dealerBet;

            if (strength >= 6 && dealerChips > callAmount + 50)
            {
                int raise = 50;
                Bet(ref dealerChips, ref dealerBet, callAmount + raise);
                currentBet = dealerBet;
            }
            else if (strength >= 3)
            {
                Bet(ref dealerChips, ref dealerBet, callAmount);
            }
            else
            {
                if (rng.Next(100) < 35)
                    dealerFolded = true;
                else
                    Bet(ref dealerChips, ref dealerBet, callAmount);
            }
        }

        private void Bet(ref int chips, ref int bet, int amount)
        {
            if (amount > chips)
                amount = chips; 

            chips -= amount;
            bet += amount;
            pot += amount;
        }

        private void Showdown()
        {
            Render("Showdown");

            var playerBest = TexasHoldemEvaluator.Evaluate(playerCards.Concat(community).ToList());
            var dealerBest = TexasHoldemEvaluator.Evaluate(dealerCards.Concat(community).ToList());

            int result = playerBest.CompareTo(dealerBest);

            if (result > 0)
            {
                Console.WriteLine("\nYou win the pot!");
                playerChips += pot;
            }
            else if (result < 0)
            {
                Console.WriteLine("\nDealer wins the pot!");
                dealerChips += pot;
            }
            else
            {
                Console.WriteLine("\nSplit pot!");
                playerChips += pot / 2;
                dealerChips += pot / 2;
            }

            Console.ReadKey();
        }

        private bool RoundEnded()
        {
            if (playerFolded)
            {
                dealerChips += pot;
                Render("Dealer Wins (Fold)");
                Console.ReadKey();
                return true;
            }

            if (dealerFolded)
            {
                playerChips += pot;
                Render("You Win (Dealer Folded)");
                Console.ReadKey();
                return true;
            }

            return false;
        }

        private int EvaluateStrength(List<Card> cards)
        {
            if (cards.Count < 5) return 1;
            return (int)TexasHoldemEvaluator.Evaluate(cards).Rank;
        }

        private void Burn() => deck.Draw();

        private void Render(string stage)
        {
            Console.Clear();
            Console.WriteLine($"===== {stage} =====\n");

            Console.WriteLine($"Dealer Chips: {dealerChips}");
            Console.WriteLine($"Pot: {pot}\n");

            Console.WriteLine("Dealer:");
            if (stage != "Showdown")
                RenderHiddenCards();
            else
                RenderCards(dealerCards);

            Console.WriteLine("\nCommunity:");
            RenderCards(community);

            Console.WriteLine("\nYou:");
            RenderCards(playerCards);

            Console.WriteLine($"\nYour Chips: {playerChips}");
        }

        // =========================
        // ASCII CARD SPRITES
        // =========================

        private void RenderCards(List<Card> cards)
        {
            if (cards.Count == 0) return;

            var sprites = cards.Select(BuildCardSprite).ToList();

            for (int i = 0; i < sprites[0].Length; i++)
            {
                foreach (var s in sprites)
                    Console.Write(s[i] + "  ");
                Console.WriteLine();
            }
        }

        private void RenderHiddenCards()
        {
            var hidden = new[] {
            "┌─────────┐",
            "|░░░░░░░░░|",
            "|░░░░░░░░░|",
            "|░░░░░░░░░|",
            "|░░░░░░░░░|",
            "|░░░░░░░░░|",
            "└─────────┘"
        };

            for (int i = 0; i < hidden.Length; i++)
                Console.WriteLine(hidden[i] + "  " + hidden[i]);
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
