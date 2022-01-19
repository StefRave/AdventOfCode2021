using System.Text;

namespace AdventOfCode2018;

public class Day15 : IAdvent
{
    (int dy, int dx)[] moves= new[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
  
    public void Run()
    {
        var objects = Advent.ReadInputLines()
                    .Select((line, y) => line.Split(' ')[0].Select((c, x) => (y, x, c)))
                    .SelectMany(l => l)
                    .Where(l => l.c != '.')
                    .ToArray();

        var wall = objects.Where(l => l.c == '#')
            .Select(l => (y: l.y, x: l.x))
            .ToHashSet();

        int maxX = wall.Max(kv => kv.x);
        int maxY = wall.Max(kv => kv.y);
        var (turn, totalHitPoints, elvesLeft) = Fight();
        Console.WriteLine($"Total turns {turn}  points={totalHitPoints}");
        Advent.AssertAnswer1(turn * totalHitPoints, 248235, 27730);

        int elvesAtStart = objects.Where(o => o.c == 'E').Count();
        for (int elveAttackPoints = 3; ; elveAttackPoints++)
        {
            (turn, totalHitPoints, elvesLeft) = Fight(elveAttackPoints: elveAttackPoints);
            if (elvesLeft == elvesAtStart)
                break;
        }
        Console.WriteLine($"Total turns {turn}  points={totalHitPoints}");
        Advent.AssertAnswer2(turn * totalHitPoints, 46784, 4988);

        (int turn, int totalHitPoints, int elvesLeft) Fight(int elveAttackPoints = 3)
        {
            var fighters = objects.Where(l => l.c == 'G' || l.c == 'E')
                .ToDictionary(l => (l.y, l.x), l => new Fighter(l.c, 200, (l.y, l.x)));

            //Console.WriteLine($"Initial");
            //Print();

            turn = 0;
            while (fighters.Select(f => f.Value.C).Distinct().Count() > 1)
            {
                var fightersInOrder = fighters.OrderBy(f => f.Key.y).ThenBy(f => f.Key.x).Select(f => f.Value);
                bool breakEarly = false;
                foreach (var fighter in fightersInOrder)
                {
                    if (fighters.Select(f => f.Value.C).Distinct().Count() == 1)
                    {
                        breakEarly = true;
                        break;
                    }
                    if (fighter.HitPoints <= 0)
                        continue;
                    var newPosition = Move(fighter);
                    if (fighter.Position != newPosition)
                    {
                        fighters.Remove(fighter.Position);
                        fighter.Position = newPosition;
                        fighters.Add(newPosition, fighter);
                    }
                    var opponent = FindOpponent(fighter);
                    if (opponent != null)
                    {
                        opponent.HitPoints -= fighter.C == 'G' ? 3 : elveAttackPoints;
                        if (opponent.HitPoints <= 0)
                            fighters.Remove(opponent.Position);
                    }
                }
                if (!breakEarly)
                {
                    turn++;
                    //Console.WriteLine($"Turn {turn}");
                    //Print();
                }
            }

            int totalHitPoints = fighters.Sum(f => f.Value.HitPoints);
            int elvesLeft = fighters.Where(f => f.Value.C == 'E').Count();
            return (turn, totalHitPoints, elvesLeft);


#pragma warning disable CS8321 // Local function is declared but never used
            void Print()
            {
                var sb = new StringBuilder();
                for (int y = 0; y <= maxY; y++)
                {
                    var hpsb = new StringBuilder();
                    for (int x = 0; x <= maxY; x++)
                    {
                        char c = '.';
                        if (fighters.TryGetValue((y, x), out Fighter thing))
                        {
                            c = thing.C;
                            hpsb.Append($" {c}({thing.HitPoints})");
                        }
                        else if (wall.Contains((y, x)))
                            c = '#';
                        sb.Append(c);
                    }
                    sb.Append("  " + hpsb.ToString() + '\n');
                }
                Console.WriteLine(sb.ToString());
            }

            Fighter FindOpponent(Fighter fighter)
            {
                Fighter bestOpponent = null;
                foreach (var move in moves)
                {
                    (int, int) pos = (fighter.Position.y + move.dy, fighter.Position.x + move.dx);
                    if (fighters.TryGetValue(pos, out Fighter opponent))
                        if (opponent.C != fighter.C && (bestOpponent == null || opponent.HitPoints < bestOpponent.HitPoints))
                            bestOpponent = opponent;
                }
                return bestOpponent;
            }

            (int y, int x) Move(Fighter fighter)
            {
                var beenThere = new HashSet<(int y, int x)>();
                var queue = new Queue<((int y, int x) pos, int distance, (int y, int x) firstMove)>();

                QueueOptions(fighter.Position);
                beenThere.Add(fighter.Position);
                (int y, int x)? bestOponentPos = null;
                (int y, int x)? bestFirstMove = null;
                int bestDistance = 0;
                while (queue.Count > 0)
                {
                    var (pos, distance, firstMove) = queue.Dequeue();
                    if (beenThere.Contains(pos))
                        continue;
                    if (bestFirstMove != null && distance != bestDistance)
                        return bestFirstMove.Value;

                    beenThere.Add(pos);
                    if (wall.Contains(pos))
                        continue;
                    if (fighters.TryGetValue(pos, out Fighter opponent))
                    {
                        if (opponent.C == fighter.C)
                            continue;
                        if (distance == 1)
                            return fighter.Position;
                        if (bestFirstMove == null || pos.y < bestOponentPos.Value.y || (pos.y == bestOponentPos.Value.y && pos.x < bestOponentPos.Value.x) || firstMove.y < bestFirstMove.Value.y || (firstMove.y == bestFirstMove.Value.y && firstMove.x < bestFirstMove.Value.x))
                        {
                            bestOponentPos = pos;
                            bestFirstMove = firstMove;
                            bestDistance = distance;
                        }
                    }
                    QueueOptions(pos, distance + 1, firstMove);
                }
                return bestFirstMove ?? fighter.Position;

                void QueueOptions((int y, int x) pos, int newDistance = 1, (int y, int x)? firstMove = null)
                {
                    foreach (var move in moves)
                    {
                        (int, int) newPos = (pos.y + move.dy, pos.x + move.dx);
                        Add(newPos, newDistance, firstMove);
                    }
                    void Add((int y, int x) newPos, int newDistance, (int y, int x)? firstMove)
                        => queue.Enqueue((newPos, newDistance, firstMove ?? newPos));
                }
            }
        }
    }

    public class Fighter
    {
        public char C { get; }
        public int HitPoints { get; set; }
        public (int y, int x) Position { get; set; }

        public Fighter(char c, int hitPoints, (int y, int x) position)
        {
            C = c;
            HitPoints = hitPoints;
            Position = position;
        }
    }
}