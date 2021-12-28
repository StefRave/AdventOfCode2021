#nullable enable
using System.Text.RegularExpressions;

namespace AdventOfCode2021;

public class Day23Alt : IAdvent
{
    private string[] space = Array.Empty<string>();
    const int hallWidth = 11;
    const int rooms = 4;
    static int depth;
    static readonly int[] MoveEnergyPerType = new[] { 1, 10, 100, 1000 };

    public void Run()
    {
        string part1State = Regex.Replace(Advent.ReadInput(), "[#\r\n ]", "");
        Advent.AssertAnswer1(DoIt(part1State));

        string part2State = part1State[0..(11 + 4)] + "DCBADBAC" + part1State[(11 + 4)..];
        Advent.AssertAnswer2(DoIt(part2State));
    }

    int? DoIt(string state)
    {
        depth = (state.Length - hallWidth) / 4;
        var dict = new Dictionary<string, (int? score, string? bestNextState)>();

        int? score = Try(state, null);

        int move = 0;
        while (true)
        {
            Draw(state, ++move);

            if (!dict.TryGetValue(state, out var nextState))
                break;
            if (nextState.bestNextState == null)
                break;
            state = nextState.bestNextState;
        }
        return score;
    

        int? Try(string state, char? focussedOnHall)
        {
            if (IsHallEmpty(state))
            {
                if (AllRoomsInOrder(state))
                    return 0;
            }

            if (dict.TryGetValue(state, out var scoreState))
                return scoreState.score;

            string oldState = state;
            int? bestScore = null;
            string? bestNextState = null;
            bool tried = false;

            // try to move from hall to destination room
            for (int x = 0; x < hallWidth; x++)
            {
                var amphi = state[x];
                if (amphi == '.')
                    continue;
                var (newstate, energy) = PositionInRoomWhenPathIsFree(state, amphi, x);
                if (newstate == null)
                    continue;
                Do(energy, newstate, focussedOnHall);
                break;
            }
            if (!tried)
            {
                // try to move from room to destination room
                foreach (char hallType in "ABCD")
                {
                    var (amphi, pos) = GetCandidateInRoom(state, hallType);
                    if (amphi == 0)
                        continue;
                    var (newstate, energy) = PositionInRoomWhenPathIsFree(state, amphi, pos);
                    if (newstate == null)
                        continue;
                    Do(energy, newstate, focussedOnHall);
                    break;
                }
            }
            if (!tried)
            {
                string hallsToTry = "ABCD";
                if (focussedOnHall != null)
                    if (GetCandidateInRoom(state, focussedOnHall.Value).type == 0)
                        focussedOnHall = null;
                if (focussedOnHall != null)
                    hallsToTry = char.ToString(focussedOnHall.Value);
                foreach (char hallType in hallsToTry)
                {
                    var (amphi, pos) = GetCandidateInRoom(state, hallType);
                    if (amphi == 0)
                        continue;
                    for (int x = GetHallX(hallType); x >= 0; x--)
                    {
                        bool isBlocked = state[x] != '.';
                        if (isBlocked)
                            break;
                        if (IsInFrontOfRoom(x))
                            continue;
                        var (newstate, energy) = MoveToHall(state, pos, x);
                        Do(energy, newstate, hallType);
                    }
                    for (int x = GetHallX(hallType); x < hallWidth; x++)
                    {
                        bool isBlocked = state[x] != '.';
                        if (isBlocked)
                            break;
                        if (IsInFrontOfRoom(x))
                            continue;
                        var (newstate, energy) = MoveToHall(state, pos, x);
                        Do(energy, newstate, hallType);
                    }
                }
            }
            dict.Add(oldState, (bestScore, bestNextState));
            return bestScore;

            int? Do(int moveEnergy, string state, char? focussedOnRoom)
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


    public static bool IsHallEmpty(string state)
        => state[0..hallWidth].All(c => c == '.');

    public static bool AllRoomsInOrder(string state)
    {
        for (int i = hallWidth; i < state.Length; i++)
            if (state[i] != 'A' + ((i - hallWidth) % rooms))
                return false;
        return true;
    }
    private static int GetRoomX(char type) => hallWidth + (type - 'A');
    private static int GetHallX(char type) => 2 + (type - 'A') * 2;


    public static int MoveEnergy(char type) => MoveEnergyPerType[type - 'A'];

    public static (string state, int energy) MoveToHall(string state, int posInRoom, int posInHall)
    {
        int depthInRoom = (posInRoom - hallWidth) / rooms + 1;
        int hallPos = ((posInRoom - hallWidth) % rooms) * 2 + 2;
        return (SwapRoomAndHall(state, posInHall, posInRoom), (Math.Abs(posInHall - hallPos) + depthInRoom) * MoveEnergy(state[posInRoom]));
    }

    public static (string? state, int energy) PositionInRoomWhenPathIsFree(string state, char type, int amphiPos)
    {
        int? storePos = GetPositionToStoreIn(state, type);
        if (storePos == null)
            return (null, 0);
        int amphiDepth = 0;
        int amphiHallX = amphiPos;
        if (amphiPos >= hallWidth)
        {
            amphiDepth = (amphiPos - hallWidth) / rooms + 1;
            amphiHallX = ((amphiPos - hallWidth) % rooms) * 2 + 2;
        }
        int hallX = GetHallX(type);
        int distance = Math.Abs(amphiHallX - hallX);
        int step = (hallX - amphiHallX) / distance;
        for (int x = hallX; x != amphiHallX; x -= step)
            if (state[x] != '.')
                return (null, 0);

        int storeDepth = (storePos.Value - hallWidth) / rooms + 1;
        distance += storeDepth + amphiDepth;
        return (SwapRoomAndHall(state, amphiPos, storePos.Value), distance * MoveEnergy(type));
    }

    public int PosToHallDistance(int pos) => (pos - hallWidth) / rooms;
    public (char type, int pos) GetCandidateInRoom(string state, char roomType)
    {
        int posResult = 0;
        char type = (char)0;

        for (int pos = GetRoomX(roomType) + ((depth - 1) * rooms); pos >= hallWidth; pos -= rooms)
        {
            if (state[pos] == '.')
                break;
            if (roomType != state[pos] || posResult != 0)
                (type, posResult) = (state[pos], pos); 
        }
        return (type, posResult);
    }
    public static int? GetPositionToStoreIn(string state, char roomType)
    {
        for (int pos = GetRoomX(roomType) + ((depth - 1) * rooms); pos >= hallWidth; pos -= rooms)
        {
            if (state[pos] == '.')
                return pos;
            if (roomType != state[pos])
                break;
        }
        return null;
    }

    public static bool IsInFrontOfRoom(int x) => x >= 2 && x <= 8 && x % 2 == 0;

    public static string SwapRoomAndHall(string state, int pos1, int pos2)
    {
        if (pos1 < pos2)
            return state[0..pos1] + state[pos2] + state[(pos1 + 1)..pos2] + state[pos1] + state[(pos2 + 1)..];
        return state[0..pos2] + state[pos1] + state[(pos2 + 1)..pos1] + state[pos2] + state[(pos1 + 1)..];
    }

    private void Draw(string state, int moveCount)
    {
        Console.WriteLine($"Move: {moveCount}");

        Console.WriteLine("".PadRight(hallWidth + 2, '#'));
        Console.WriteLine($"#{state[0..hallWidth]}#");
        for (int i = 0; i < (state.Length - hallWidth) / rooms; i++)
        {
            Console.Write(i == 0 ? "##" : "  ");
            for (int roomIndex = 0; roomIndex < rooms; roomIndex++)
                Console.Write("#" + state[hallWidth + i * rooms + roomIndex]);
            Console.Write("#");
            Console.Write(i == 0 ? "##" : "  ");
            Console.WriteLine("");
        }
        Console.WriteLine("".PadRight(hallWidth - 1 - 2 * rooms) + "".PadRight(rooms * 2 + 1, '#'));
    }
}