using System.Text;

namespace AdventOfCode2018;

public class Day13 : IAdvent
{
    public void Run()
    {
        // initial state: .##..##.
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine()
            .Select(inp => inp.SplitByNewLine().Select(line => line.ToCharArray()).ToArray())
            .ToArray();
        var (grid, carts) = FixGridAndReturnCarts(input[0]);
        carts.Sort();

        var collistion = Go(grid, carts, stopAfterCollision: true);
        Advent.AssertAnswer1($"{collistion.x},{collistion.y}", expected: "119,41", sampleExpected: "7,3");

        (grid, carts) = FixGridAndReturnCarts(input[^1]);
        collistion = Go(grid, carts, stopAfterCollision: false);
        Advent.AssertAnswer1($"{collistion.x},{collistion.y}", expected: "45,136", sampleExpected: "6,4");
    }

    private static (int x, int y) Go(char[][] grid, List<Cart> carts, bool stopAfterCollision)
    {
        var result = (x: -1, y: -1);

        while (result.x < 0)
        {
            carts.Sort();

            //Print();

            for (int i = 0; i < carts.Count; i++)
            {
                var cart = carts[i];
                if (cart.Direction == Direction.Up)
                {
                    cart.Y -= 1;
                    char c = grid[cart.Y][cart.X];
                    if (c == '\\')
                        cart.Direction = Direction.Left;
                    if (c == '/')
                        cart.Direction = Direction.Right;
                }
                else if (cart.Direction == Direction.Down)
                {
                    cart.Y += 1;
                    char c = grid[cart.Y][cart.X];
                    if (c == '\\')
                        cart.Direction = Direction.Right;
                    if (c == '/')
                        cart.Direction = Direction.Left;
                }
                else if (cart.Direction == Direction.Left)
                {
                    cart.X -= 1;
                    char c = grid[cart.Y][cart.X];
                    if (c == '\\')
                        cart.Direction = Direction.Up;
                    if (c == '/')
                        cart.Direction = Direction.Down;
                }
                else if (cart.Direction == Direction.Right)
                {
                    cart.X += 1;
                    char c = grid[cart.Y][cart.X];
                    if (c == '\\')
                        cart.Direction = Direction.Down;
                    if (c == '/')
                        cart.Direction = Direction.Up;
                }
                if (grid[cart.Y][cart.X] == '+')
                {
                    cart.Direction = (Direction)(((int)cart.Direction + cart.NextTurnCount + 3) % 4);
                    cart.NextTurnCount = (cart.NextTurnCount + 1) % 3;
                }
                if (carts.Count(c => c.X == cart.X && c.Y == cart.Y) > 1)
                {
                    if (stopAfterCollision)
                    {
                        result = (cart.X, cart.Y);
                        break;
                    }
                    for (int j = carts.Count - 1; j >= 0; j--)
                    {
                        if (carts[j].X == cart.X && carts[j].Y == cart.Y)
                        {
                            carts.RemoveAt(j);
                            if (j <= i)
                                i--;
                        }
                    }
                }
            }
            if (carts.Count == 1)
                result = (carts[0].X, carts[0].Y);
        }
        return result;


        void Print()
        {
            Console.Clear();
            var toPrint = grid.Select(ca => new StringBuilder(new string(ca))).ToArray();
            foreach (var c in carts)
                toPrint[c.Y][c.X] = CartSign[(int)c.Direction];
            foreach (var sb in toPrint)
                Console.WriteLine(sb.ToString());
        }
    }

    private static (char[][] grid, List<Cart> carts) FixGridAndReturnCarts(char[][] grid)
    {
        grid = grid.Select(row => row.ToArray()).ToArray();
        List<Cart> carts;
        carts = new List<Cart>();
        for (int y = 0; y < grid.Length; y++)
            for (int x = 0; x < grid[0].Length; x++)
            {
                int cartSign = Array.IndexOf(CartSign, grid[y][x]);
                if (cartSign >= 0)
                {
                    carts.Add(new Cart() { X = x, Y = y, Direction = (Direction)cartSign });
                    grid[y][x] = (cartSign % 2) == 0 ? '|' : '-';
                }
            }
        return (grid, carts);
    }

    public class Cart : IComparable<Cart>
    {
        public int Y { get; set; }
        public int X { get; set; }
        public Direction Direction { get; set; }

        public int NextTurnCount { get; set; }

        public int CompareTo(Cart other)
        {
            int result = Y.CompareTo(other.Y);
            if (result != 0)
                return result;
            return X.CompareTo(other.X);
        }
    }

    public enum Direction { Up, Right, Down, Left };
    public static char[] CartSign = new[] { '^', '>', 'v', '<' };
}