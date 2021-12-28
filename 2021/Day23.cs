#nullable enable
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2021;

public class Day23 : IAdvent
{
    private string[] space = Array.Empty<string>();

    public void Run()
    {
        var input = Advent.ReadInputLines();
        Advent.AssertAnswer1(DoIt(input));

        input = input.ToImmutableArray().InsertRange(3, new [] {"  #D#C#B#A#", "  #D#B#A#C#"}).ToArray();
        Advent.AssertAnswer2(DoIt(input));
    }

    int? DoIt(string[] input)
    {
        space = input.Select(line => Regex.Replace(line, "[ABCD]", ".")).ToArray();

        var rooms = new List<Amphi>().ToImmutableList();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (char.IsLetter(input[y][x]))
                    rooms = rooms.Add(new Amphi(input[y][x], y, x));

        var state = new State(rooms, ImmutableSortedSet<Amphi>.Empty);
        int depth = input.Length - 3;
        var dict = new Dictionary<State, (int? score, State? bestNextState)>();

        int? score = Try(state, null);

        int move = 0;
        while(true)
        {
            Draw(state, ++move);

            if (!dict.TryGetValue(state, out var nextState))
                break;
            state = nextState.bestNextState;
            if (state == null)
                break;
        }
        return score;
    

        int? Try(State state, char? focussedOnHall)
        {
            if (state.Hall.Count == 0)
            {
                if (state.AllRoomsInOrder())
                    return 0;
            }

            if (dict.TryGetValue(state, out var scoreState))
                return scoreState.score;

            State oldState = state;
            int? bestScore = null;
            State? bestNextState = null;
            bool tried = false;

            // try to move from hall to destination room
            foreach (var amphi in state.Hall)
            {
                var newAmphi = state.PositionInRoomWhenPathIsFree(amphi, depth: depth);
                if (newAmphi == null)
                    continue;
                Do(amphi.MoveEnergyTo(newAmphi), state.MoveFromHallToRoom(amphi, newAmphi), focussedOnHall);
                break;
            }
            if (!tried)
            {
                // try to move from room to destination room
                foreach (char hallType in "ABCD")
                {
                    Amphi? amphi = state.GetCandidateInRoom(hallType);
                    if (amphi == null)
                        continue;
                    var newAmphi = state.PositionInRoomWhenPathIsFree(amphi, depth: depth);
                    if (newAmphi == null)
                        continue;
                    Do(amphi.MoveEnergyTo(newAmphi), state.MoveFromRoomToRoom(amphi, newAmphi), focussedOnHall);
                    break;
                }
            }
            if (!tried)
            {
                string hallsToTry = "ABCD";
                if (focussedOnHall != null)
                    if (state.GetCandidateInRoom(focussedOnHall.Value) == null)
                        focussedOnHall = null;
                if (focussedOnHall != null)
                    hallsToTry = char.ToString(focussedOnHall.Value);
                foreach (char hallType in hallsToTry)
                {
                    Amphi? amphi = state.GetCandidateInRoom(hallType);
                    if (amphi == null)
                        continue;
                    int min = 1;
                    for (int x = State.GetDestinationX(hallType); x >= min; x--)
                    {
                        bool isBlocked = state.Hall.Any(a => a.X == x);
                        if (isBlocked)
                            break;
                        if (State.IsInFrontOfRoom(x))
                            continue;
                        var newAmphi = amphi with { X = x, Y = 1 };
                        Do(amphi.MoveEnergyTo(newAmphi), state.MoveFromRoomToHall(amphi, newAmphi), hallType);
                    }
                    for (int x = State.GetDestinationX(hallType); x <= 11; x++)
                    {
                        bool isBlocked = state.Hall.Any(a => a.X == x);
                        if (isBlocked)
                            break;
                        if (State.IsInFrontOfRoom(x))
                            continue;
                        var newAmphi = amphi with { X = x, Y = 1 };
                        Do(amphi.MoveEnergyTo(newAmphi), state.MoveFromRoomToHall(amphi, newAmphi), hallType);
                    }
                }
            }
            dict.Add(oldState, (bestScore, bestNextState));
            return bestScore;

            int? Do(int moveEnergy, State state, char? focussedOnRoom)
            {
                tried = true;
                int? energy = Try(state, focussedOnRoom);
                if (energy != null && (bestScore == null || bestScore > (moveEnergy + energy)))
                {
                    bestScore = moveEnergy + energy;
                    bestNextState = state;
                }
                return bestScore;
            }
        } 
    }

    public class State
    {
        public ImmutableList<Amphi> Rooms { get; }
        public ImmutableSortedSet<Amphi> Hall { get; }
        
        public State(ImmutableList<Amphi> rooms, ImmutableSortedSet<Amphi> hall)
        {
            Rooms = rooms;
            Hall = hall;
        }

        public bool AllRoomsInOrder()
            => Rooms.All(r => r.X == GetDestinationX(r.Type));
        
        public static int GetDestinationX(char amphiType) => 3 + (amphiType - 'A') * 2;

        public Amphi? GetCandidateInRoom(char type)
        {
            if (IsDestinationInOrder(type))
                return null;
            int xTarget = GetDestinationX(type);
            return Rooms.FirstOrDefault(a => a.X == xTarget);
        }

        public bool IsDestinationInOrder(char type)
        {
            int xTarget = GetDestinationX(type);
            return !Rooms.Any(a => a.X == xTarget && a.Type != type);
        }

        public Amphi? PositionInRoomWhenPathIsFree(Amphi amphi, int depth)
        {
            if(!IsDestinationInOrder(amphi.Type))
                return null;

            int xTarget = GetDestinationX(amphi.Type);
            int x = amphi.X;
            int step = (xTarget - x) / Math.Abs(x - xTarget); 
            while (x != xTarget)
            {
                x += step;
                if (Hall.Any(a => a.X == x))
                    return null;
            }
            int? minDepth = Rooms.Where(a => a.X == x).Min(a => (int?)a.Y);
            int targetY = minDepth.HasValue ? minDepth.Value - 1 : depth + 1;

            return new Amphi(amphi.Type, targetY, xTarget);
        }
        public static bool IsInFrontOfRoom(int x) => x >= 3 && x <= 9 && x % 2 == 1;

        public State MoveFromHallToRoom(Amphi amphi, Amphi newAmphi)
            => new State(Rooms.Add(newAmphi), Hall.Remove(amphi));

        public State MoveFromRoomToRoom(Amphi amphi, Amphi newAmphi)
            => new State(Rooms.Remove(amphi).Add(newAmphi), Hall);

        public State MoveFromRoomToHall(Amphi amphi, Amphi newAmphi)
            => new State(Rooms.Remove(amphi), Hall.Add(newAmphi));

        public override bool Equals(object? obj)
        {
            return obj is State state &&
                Rooms.SequenceEqual(state.Rooms) &&
                Hall.SequenceEqual(state.Hall);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Rooms.Aggregate(0, (total, next) => HashCode.Combine(total, next)),
                Hall.Aggregate(0, (total, next) => HashCode.Combine(total, next)));
        }
    }

    public record Amphi(char Type, int Y, int X) : IComparable<Amphi>
    {
        static readonly int[] MoveEnergyPerType = new[] { 1, 10, 100, 1000 };
        public int MoveEnergy => MoveEnergyPerType[Type - 'A'];

        public int CompareTo(Amphi? other)
        {
            if (other == null) return 1;
            int result = Type.CompareTo(other.Type);
            if (result != 0) return result;
            result = X.CompareTo(other.X);
            if (result != 0) return result;
            return Y.CompareTo(other.Y);
        }

        public int MoveEnergyTo(Amphi other) => (Math.Abs(other.X - X) + Y - 1 + other.Y - 1) * MoveEnergy;
    }

    private void Draw(State state, int moveCount)
    {
        Console.WriteLine($"Move: {moveCount}");
        for (int y = 0; y < space.Length; y++)
        {
            var sb = new StringBuilder(space[y]);
            var amphis = new List<Amphi>();
            amphis.AddRange(state.Hall);
            amphis.AddRange(state.Rooms);

            foreach (Amphi amphi in amphis)
                if (amphi.Y == y)
                    sb[amphi.X] = amphi.Type;
            Console.WriteLine(sb.ToString());
        }
    }
}