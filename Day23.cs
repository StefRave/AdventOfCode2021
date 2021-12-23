#nullable enable
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.EventHandlers;

namespace AdventOfCode2021;

public class Day23
{
    private readonly ITestOutputHelper output;
    public Day23(ITestOutputHelper output)
    {
        this.output = output;
    }

    private string[] space = Array.Empty<string>();

    private void Draw(State state, int moveCount)
    {
        output.WriteLine($"Move: {moveCount}");
        for (int y = 0; y < space.Length; y++)
        {
            var sb = new StringBuilder(space[y]);
            var amphis = new List<Amphi>();
            amphis.AddRange(state.InHall);
            amphis.AddRange(state.AmphisWaiting);
            foreach (var amphi in state.AmphisReady)
                amphis.Add(amphi with { Type = char.ToLower(amphi.Type) });

            foreach (Amphi amphi in amphis)
                if (amphi.Y == y)
                    sb[amphi.X] = amphi.Type;
            output.WriteLine(sb.ToString());
        }
    } 
    HashSet<State> hasTried = new HashSet<State>();
    [Fact]
    public void Run()
    {
        var input = Advent.ReadInputLines();
        Advent.AssertAnswer1(DoIt(input));

        input = input.ToImmutableArray().InsertRange(3, new [] {"  #D#C#B#A#", "  #D#B#A#C#"}).ToArray();
        Advent.AssertAnswer2(DoIt(input));
    }

    long DoIt(string[] input)
    {
        space = input.Select(line => Regex.Replace(line, "[ABCD]", ".")).ToArray();

        var amphisWaiting = new List<Amphi>().ToImmutableList();
        for (int y = 0; y < input.Length; y++)
            for (int x = 0; x < input[y].Length; x++)
                if (char.IsLetter(input[y][x]))
                    amphisWaiting = amphisWaiting.Add(new Amphi(input[y][x], y, x));

        var amphisReady = ImmutableList<Amphi>.Empty;
        foreach (var amphi in amphisWaiting.ToArray())
            if (amphi.X == State.GetDestinationX(amphi.Type) && amphi.Y == input.Length - 2)
                (amphisReady, amphisWaiting) = (amphisReady.Add(amphi), amphisWaiting.Remove(amphi));

        var state = new State(amphisWaiting, amphisReady, ImmutableHashSet<Amphi>.Empty);

        int depth = input.Length - 3;
        int moveCount = 0;
        long score = Try(state);
        return score;
    

        long Try(State state)
        {
            if (++moveCount == 1)
                Draw(state, moveCount);
            // if (moveCount > 100)
            //     return 0;

            long minScore = int.MaxValue;

            bool found = false;
            foreach (var amphi in state.InHall)
            {
                var newAmphi = state.PositionInRoomWhenPathIsFree(amphi, depth: depth);
                if (newAmphi != null)
                {
                    found = true;
                    Do(amphi.MoveEnergyTo(newAmphi), state with { InHall = state.InHall.Remove(amphi), AmphisReady = state.AmphisReady.Add(newAmphi) });
                }
            }
            if (!found)
            {
                if (state.AmphisWaiting.Count == 0)
                {
                    if (state.InHall.Count > 0)
                        return int.MaxValue;
                    return 0;
                }
                foreach (char hallType in "ABCD")
                {
                    Amphi? amphi = state.GetCandidateForHall(hallType);
                    if (amphi == null)
                        continue;
                    int min = amphi.Type == 'D' ? 3 : 1;
                    for (int x = State.GetDestinationX(hallType); x >= min; x--)
                    {
                        bool isBlocked = state.InHall.Any(a => a.X == x);
                        if (isBlocked)
                            break;
                        if (State.IsInFrontOfRoom(x))
                            continue;
                        var newAmphi = amphi with { X = x, Y = 1 };
                        Do(amphi.MoveEnergyTo(newAmphi), state with { InHall = state.InHall.Add(newAmphi), AmphisWaiting = state.AmphisWaiting.Remove(amphi) });
                    }
                    for (int x = State.GetDestinationX(hallType); x <= 11; x++)
                    {
                        bool isBlocked = state.InHall.Any(a => a.X == x);
                        if (isBlocked)
                            break;
                        if (State.IsInFrontOfRoom(x))
                            continue;
                        var newAmphi = amphi with { X = x, Y = 1 };
                        Do(amphi.MoveEnergyTo(newAmphi), state with { InHall = state.InHall.Add(newAmphi), AmphisWaiting = state.AmphisWaiting.Remove(amphi) });
                    }
                }
            }
            return minScore;

            void Do(int moveEnergy, State state) => minScore = Math.Min(minScore, moveEnergy + Try(state));
        } 
    }

    public record State(ImmutableList<Amphi> AmphisWaiting, ImmutableList<Amphi> AmphisReady, ImmutableHashSet<Amphi> InHall)
    {
        private static readonly (int dy, int dx)[] Moves = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) }; 
    
        public static int GetDestinationX(char amphiType) => 3 + (amphiType - 'A') * 2;

        public bool IsAtDestination(Amphi amphi)
        {
            if (amphi.X != GetDestinationX(amphi.Type))
                return false;
            if (amphi.Y == 1)
                return false;
            if (amphi.Y == 3)
                return true;
            return AmphisReady.Any(a => a.X == amphi.X && a.Type == amphi.Type);
        }

        public Amphi? GetCandidateForHall(char type)
        {
            int xTarget = GetDestinationX(type);
            return AmphisWaiting.FirstOrDefault(a => a.X == xTarget);
        }

        public bool IsDestinationFree(char type, int x) => !AmphisWaiting.Any(a => a.X == x && a.Type != type);
        public Amphi? PositionInRoomWhenPathIsFree(Amphi amphi, int depth)
        {
            int xTarget = GetDestinationX(amphi.Type);
            if(!IsDestinationFree(amphi.Type, xTarget))
                return null;

            int x = amphi.X;
            int step = (xTarget - x) / Math.Abs(x - xTarget); 
            while (x != xTarget)
            {
                x += step;
                if (InHall.Any(a => a.X == x))
                    return null;
            }
            int? minDepth = AmphisReady.Where(a => a.X == x).Min(a => (int?)a.Y);
            int targetY = minDepth.HasValue ? minDepth.Value - 1 : depth + 1;

            return new Amphi(amphi.Type, targetY, xTarget);
        }

        public static bool IsInFrontOfRoom(int x) => x >= 3 && x <= 9 && x % 2 == 1;
    }
    public record Amphi(char Type, int Y, int X)
    {
        static readonly int[] MoveEnergyPerType = new[] { 1, 10, 100, 1000 };
        public int MoveEnergy => MoveEnergyPerType[Type - 'A'];
        public int MoveEnergyTo(Amphi other) => (Math.Abs(other.X - X) + Math.Abs(other.Y - Y)) * MoveEnergy;
    }
}
