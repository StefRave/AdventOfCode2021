using System.Text.RegularExpressions;

namespace AdventOfCode2018;

public class Day24 : IAdvent
{
    static Regex parseLine = new Regex(@"^(?<Unit>\d+) units each with (?<hitPoints>\d+) hit points (\((?<weakImmune>.*)\) )?" +
        @"with an attack that does (?<damage>\d+) (?<damageType>\w+) damage at initiative (?<initiative>\d+)", RegexOptions.Multiline);
    static Regex parseWeakImumune = new Regex(@"(?:(\w+) to (?:(\w+),? ?)+)");

    public void Run()
    {
        // 889 units each with 3275 hit points (weak to bludgeoning, radiation; immune to cold) with an attack that does 36 bludgeoning damage at initiative 12
        var input = Advent.ReadInput()
            .SplitByDoubleNewLine();
        var immuneSystem = Parse(input[0], "Immune System");
        var infections = Parse(input[1], "Infection");

        (int immuneSystemUnitsLeft, int infectionUnitsLeft) = Fight(immuneSystem, infections);
        Advent.AssertAnswer1(immuneSystemUnitsLeft + infectionUnitsLeft, 18532, 5216);

        int boost = 0;
        while (infectionUnitsLeft != 0)
            (immuneSystemUnitsLeft, infectionUnitsLeft) = Fight(immuneSystem, infections, ++boost);

        Advent.AssertAnswer1(immuneSystemUnitsLeft, 6523, 51);
    }

    static (int immuneSystemUnitsLeft, int infectionUnitsLeft) Fight(IReadOnlyList<Participant> immuneSystemStart, IReadOnlyList<Participant> infectionsStart, int immuneSystemBoost = 0)
    {
        var immuneSystem = immuneSystemStart.Select(p => p.Clone(immuneSystemBoost)).ToList();
        var infections = infectionsStart.Select(p => p.Clone()).ToList();

        bool damageDone = true;
        while (damageDone)
        {
            damageDone = false;
            var attackerAndAttacked =
                GetDamagePerDefender(immuneSystem, infections)
                .Union(GetDamagePerDefender(infections, immuneSystem))
                .OrderByDescending(d => d.attacker.Initiative)
                .ToArray();
            foreach (var (attacker, targeted) in attackerAndAttacked)
            {
                int dammage = attacker.GetInflictedDamageTo(targeted);
                int killing = Math.Min(targeted.UnitCount, dammage / targeted.HitPoints);

                targeted.UnitCount = targeted.UnitCount - killing;
                if (killing > 0)
                    damageDone = true;
            }
            immuneSystem = immuneSystem.Where(p => p.UnitCount > 0).ToList();
            infections = infections.Where(p => p.UnitCount > 0).ToList();
        }
        int unitsLeft = immuneSystem.Concat(infections).Sum(p => p.UnitCount);
        return (immuneSystem.Sum(p => p.UnitCount), infections.Sum(p => p.UnitCount));
    }

    static IEnumerable<(Participant attacker, Participant targeted)> GetDamagePerDefender(IReadOnlyList<Participant> attackers, IReadOnlyList<Participant> defenders)
    {
        var defendersLeft = defenders.ToHashSet();

        foreach (var attacker in attackers.OrderByDescending(g => (g.EffectivePower, g.Initiative)))
        {
            var defender =
                (
                    from def in defendersLeft
                    let dammage = attacker.GetInflictedDamageTo(def)
                    where dammage > 0
                    orderby dammage descending, def.EffectivePower descending, def.Initiative descending
                    select def
                ).FirstOrDefault();
            if (defender == null)
                continue;

            yield return (attacker, defender);
            defendersLeft.Remove(defender);
            if (defendersLeft.Count == 0)
                yield break;
        }
    }

    static IReadOnlyList<Participant> Parse(string text, string groupType)
    {
        int groupNumber = 0;
        var result =
            from m in parseLine.Matches(text).Cast<Match>()
            let weakImmune = ParseWeakImmune(m.Groups["weakImmune"].Value)
            select new Participant(++groupNumber, groupType,
                int.Parse(m.Groups["Unit"].Value),
                int.Parse(m.Groups["hitPoints"].Value),
                weakImmune.weakTo, weakImmune.immuneTo,
                int.Parse(m.Groups["damage"].Value),
                m.Groups["damageType"].Value,
                int.Parse(m.Groups["initiative"].Value));
        return result.ToArray();


        (string[] weakTo, string[] immuneTo) ParseWeakImmune(string text)
        {
            string[] weakTo = new string[0];
            string[] immuneTo = new string[0];
            foreach (Match m in parseWeakImumune.Matches(text))
            {
                string[] values = m.Groups[2].Captures.Cast<Capture>().Select(c => c.Value).ToArray();
                if (m.Groups[1].Value == "weak")
                    weakTo = values;
                else
                    immuneTo = values;
            }
            return (weakTo, immuneTo);
        }
    }

    public class Participant
    {
        public int GroupNumber { get; }
        public string GroupType { get; }
        public int UnitCount { get; set; }
        public int HitPoints { get; }
        public IReadOnlyCollection<string> WeakTo { get; }
        public IReadOnlyCollection<string> ImmuneTo { get; }
        public int AttackDamage { get; private set; }
        public string DamageType { get; }
        public int Initiative { get; }

        public int EffectivePower => UnitCount * AttackDamage;
        public int GetInflictedDamageTo(Participant targeted)
            => (targeted.WeakTo.Contains(DamageType) ? 2 : 1) * (targeted.ImmuneTo.Contains(DamageType) ? 0 : 1) * EffectivePower;
        public Participant(int groupNumber, string groupType, int unitCount, int hitPoints, string[] weakTo, string[] immuneTo, int attackDamage, string damageType, int initiative)
        {
            GroupNumber = groupNumber;
            GroupType = groupType;
            UnitCount = unitCount;
            HitPoints = hitPoints;
            WeakTo = weakTo;
            ImmuneTo = immuneTo;
            AttackDamage = attackDamage;
            DamageType = damageType;
            Initiative = initiative;
        }

        public Participant Clone(int boost = 0)
        {
            Participant clone = (Participant)MemberwiseClone();
            clone.AttackDamage += boost;
            return clone;
        }
    }
}