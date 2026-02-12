using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Poker : IGame
{
    public string Name => "Poker";
    public bool IsCompleted { get; private set; }

    public void Play(Player player)
    {
        Console.Clear();
        Console.WriteLine("===== TEXAS HOLDEM =====");
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

        var playerCards = new List<Card> { deck.Draw(), deck.Draw() };
        var dealerCards = new List<Card> { deck.Draw(), deck.Draw() };

        var community = new List<Card>();

        Console.WriteLine("\nYour cards:");
        playerCards.ForEach(c => Console.WriteLine(c));

        Console.WriteLine("\nPress key for flop...");
        Console.ReadKey();
        community.Add(deck.Draw());
        community.Add(deck.Draw());
        community.Add(deck.Draw());
        ShowCommunity(community);

        Console.WriteLine("\nPress key for turn...");
        Console.ReadKey();
        community.Add(deck.Draw());
        ShowCommunity(community);

        Console.WriteLine("\nPress key for river...");
        Console.ReadKey();
        community.Add(deck.Draw());
        ShowCommunity(community);

        var playerBest = TexasHoldemEvaluator.Evaluate(
            playerCards.Concat(community).ToList());

        var dealerBest = TexasHoldemEvaluator.Evaluate(
            dealerCards.Concat(community).ToList());

        Console.WriteLine("\nDealer cards:");
        dealerCards.ForEach(c => Console.WriteLine(c));

        int result = playerBest.CompareTo(dealerBest);

        if (result > 0)
        {
            Console.WriteLine("\nYou win!");
            player.AddChips(bet * 2);
        }
        else if (result < 0)
        {
            Console.WriteLine("\nDealer wins!");
        }
        else
        {
            Console.WriteLine("\nIt's a tie!");
            player.AddChips(bet);
        }

        IsCompleted = true;
        Console.ReadKey();
    }

    private void ShowCommunity(List<Card> community)
    {
        Console.WriteLine("\nCommunity Cards:");
        community.ForEach(c => Console.WriteLine(c));
    }
}
