using System.Text;

namespace AdventOfCode2018;

public class Day15 : IAdvent
{
    (int dy, int dx)[] moves= new[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
  
    public void Run()
    {
        var objects = Advent.ReadInputLines()
                    .Select((line, y) => line.Select((c, x) => (y, x, c)))
                    .SelectMany(l => l)
                    .Where(l => l.c != '.')
                    .ToArray();

        Dictionary<(int y, int x), Fighter> fighters = objects.Where(l => l.c == 'G' || l.c == 'E')
            .ToDictionary(l => (l.y, l.x), l => new Fighter(l.c, 200, (l.y, l.x)));
        var wall = objects.Where(l => l.c == '#')
            .Select(l => (y: l.y, x: l.x))
            .ToHashSet();

        int maxX = wall.Max(kv => kv.x);
        int maxY = wall.Max(kv => kv.y);

        Console.WriteLine($"Initial");
        Print();
        int turn = 0;
        while (fighters.Select(f => f.Value.C).Distinct().Count() > 1)
        {
            var fightersInOrder = fighters.OrderBy(f => f.Key.y * 1000 + f.Key.x).Select(f => f.Value);
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
                var (closestFighter, isInFightingDistance, newPosition) = GetClosestFighter(fighter);
                if (isInFightingDistance)
                {
                    closestFighter.HitPoints -= 3;
                    if (closestFighter.HitPoints <= 0)
                        fighters.Remove(closestFighter.Position);
                }
                if (fighter.Position != newPosition)
                {
                    fighters.Remove(fighter.Position);
                    fighter.Position = newPosition;
                    fighters.Add(newPosition, fighter);
                }
            }
            if (!breakEarly)
            {
                Console.WriteLine($"Turn {++turn}");
                Print();
            }
        }

        int totalHitPoints = fighters.Sum(f => f.Value.HitPoints);
        Console.WriteLine($"Total turns {turn}  points={totalHitPoints}");
        Advent.AssertAnswer1(turn * totalHitPoints, 0, 27730);

        (Fighter closestFighter, bool isInFightingDistance, (int y, int x) newPosition) GetClosestFighter(Fighter fighter)
        {
            var beenThere = new HashSet<(int y, int x)>();
            var queue = new Queue<((int y, int x) pos, int distance, (int y, int x) firstMove)>();

            QueueOptions(fighter.Position);
            beenThere.Add(fighter.Position);
            while (queue.Count > 0)
            {
                var (pos, distance, firstMove) = queue.Dequeue();
                if (beenThere.Contains(pos))
                    continue;
                beenThere.Add(pos);
                if (wall.Contains(pos))
                    continue;
                if (fighters.TryGetValue(pos, out Fighter opponent))
                {
                    if (opponent.C == fighter.C)
                        continue;
                    if (distance > 2)
                        return (opponent, isInFightingDistance: false, firstMove);
                    var newPos = distance == 1 ? fighter.Position : firstMove;
                    opponent =
                        (
                        from d in moves
                        let oponentPos = (newPos.y + d.dy, newPos.x + d.dx)
                        where fighters.ContainsKey(oponentPos)
                        let o = fighters[oponentPos]
                        where o.C != fighter.C
                        orderby o.HitPoints, o.Position.y, o.Position.x
                        select o
                        ).First();
                    return (opponent, isInFightingDistance: true, newPos);
                }
                QueueOptions(pos, distance + 1, firstMove);
            }
            return (fighter, false, fighter.Position);

            void QueueOptions((int y, int x) pos, int newDistance = 1, (int y, int x)? firstMove = null)
            {
                foreach (var move in moves)
                    Add((pos.y + move.dy, pos.x + move.dx), newDistance, firstMove);

                void Add((int y, int x) newPos, int newDistance, (int y, int x)? firstMove)
                    => queue.Enqueue((newPos, newDistance, firstMove ?? newPos));
            }
        }

        void Print()
        {
            //Console.Clear();
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