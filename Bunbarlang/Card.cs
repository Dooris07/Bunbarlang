using System;
using System.Collections.Generic;
using System.Linq;

#region ENUMS

public enum Suit { Hearts, Diamonds, Clubs, Spades }

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack = 11, Queen, King, Ace
}

public enum PokerHandRank
{
    HighCard = 1,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    RoyalFlush
}

#endregion

#region CARD

public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

#endregion

#region DECK

public class Deck
{
    private List<Card> cards;
    private Random rng = new Random();

    public Deck()
    {
        cards = new List<Card>();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                cards.Add(new Card(suit, rank));
    }

    public void Shuffle()
    {
        cards = cards.OrderBy(x => rng.Next()).ToList();
    }

    public Card Draw()
    {
        var c = cards[0];
        cards.RemoveAt(0);
        return c;
    }
}

#endregion

// ==========================
// BLACKJACK
// ==========================

public class BlackjackHand
{
    public List<Card> Cards { get; } = new List<Card>();

    public void AddCard(Card card) => Cards.Add(card);

    public int GetValue()
    {
        int value = 0;
        int aces = 0;

        foreach (var card in Cards)
        {
            if (card.Rank >= Rank.Jack && card.Rank <= Rank.King)
                value += 10;
            else if (card.Rank == Rank.Ace)
            {
                value += 11;
                aces++;
            }
            else
                value += (int)card.Rank;
        }

        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    public bool IsBust => GetValue() > 21;

    public override string ToString()
    {
        return string.Join(", ", Cards) + $" (Value: {GetValue()})";
    }
}

// ==========================
// TEXAS HOLDEM
// ==========================

public class PokerResult : IComparable<PokerResult>
{
    public PokerHandRank Rank { get; }
    public List<int> TieBreakers { get; }

    public PokerResult(PokerHandRank rank, List<int> tie)
    {
        Rank = rank;
        TieBreakers = tie;
    }

    public int CompareTo(PokerResult other)
    {
        if (Rank != other.Rank)
            return Rank.CompareTo(other.Rank);

        for (int i = 0; i < TieBreakers.Count; i++)
        {
            if (TieBreakers[i] != other.TieBreakers[i])
                return TieBreakers[i].CompareTo(other.TieBreakers[i]);
        }
        return 0;
    }
}

public static class TexasHoldemEvaluator
{
    public static PokerResult Evaluate(List<Card> sevenCards)
    {
        var combinations = GetCombinations(sevenCards, 5);
        PokerResult best = null;

        foreach (var combo in combinations)
        {
            var result = EvaluateFive(combo);
            if (best == null || result.CompareTo(best) > 0)
                best = result;
        }

        return best;
    }

    private static PokerResult EvaluateFive(List<Card> cards)
    {
        var ordered = cards.OrderByDescending(c => (int)c.Rank).ToList();
        bool flush = cards.All(c => c.Suit == cards[0].Suit);

        var ranks = ordered.Select(c => (int)c.Rank).ToList();
        bool straight = IsStraight(ranks);

        var groups = ranks.GroupBy(r => r)
                          .OrderByDescending(g => g.Count())
                          .ThenByDescending(g => g.Key)
                          .ToList();

        if (straight && flush && ranks.Max() == 14)
            return new PokerResult(PokerHandRank.RoyalFlush, ranks);

        if (straight && flush)
            return new PokerResult(PokerHandRank.StraightFlush, ranks);

        if (groups[0].Count() == 4)
            return new PokerResult(PokerHandRank.FourOfAKind,
                groups.Select(g => g.Key).ToList());

        if (groups[0].Count() == 3 && groups[1].Count() == 2)
            return new PokerResult(PokerHandRank.FullHouse,
                groups.Select(g => g.Key).ToList());

        if (flush)
            return new PokerResult(PokerHandRank.Flush, ranks);

        if (straight)
            return new PokerResult(PokerHandRank.Straight, ranks);

        if (groups[0].Count() == 3)
            return new PokerResult(PokerHandRank.ThreeOfAKind,
                groups.Select(g => g.Key).ToList());

        if (groups[0].Count() == 2 && groups[1].Count() == 2)
            return new PokerResult(PokerHandRank.TwoPair,
                groups.Select(g => g.Key).ToList());

        if (groups[0].Count() == 2)
            return new PokerResult(PokerHandRank.OnePair,
                groups.Select(g => g.Key).ToList());

        return new PokerResult(PokerHandRank.HighCard, ranks);
    }

    private static bool IsStraight(List<int> ranks)
    {
        var distinct = ranks.Distinct().OrderByDescending(x => x).ToList();
        if (distinct.Count < 5) return false;

        for (int i = 0; i <= distinct.Count - 5; i++)
        {
            if (distinct[i] - distinct[i + 4] == 4)
                return true;
        }

        // Ász kis sor check
        if (distinct.Contains(14) &&
            distinct.Contains(2) &&
            distinct.Contains(3) &&
            distinct.Contains(4) &&
            distinct.Contains(5))
            return true;

        return false;
    }

    private static IEnumerable<List<Card>> GetCombinations(List<Card> list, int length)
    {
        if (length == 1)
            return list.Select(t => new List<Card> { t });

        return GetCombinations(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)),
                (t1, t2) =>
                {
                    var combo = new List<Card>(t1) { t2 };
                    combo.Sort((a, b) => a.GetHashCode().CompareTo(b.GetHashCode()));
                    return combo;
                })
            .Distinct(new ListComparer<Card>());
    }
}

public class ListComparer<T> : IEqualityComparer<List<T>>
{
    public bool Equals(List<T> x, List<T> y)
        => x.SequenceEqual(y);

    public int GetHashCode(List<T> obj)
    {
        int hash = 17;
        foreach (var item in obj)
            hash = hash * 23 + item.GetHashCode();
        return hash;
    }
}
