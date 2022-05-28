using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace AllegianceOverhaul.MigrationTweaks
{
    internal class AOSettlementDistance
    {
        private Vec2 _latestHeroPosition;
        private List<SettlementDistancePair> _sortedSettlements;
        private List<Settlement> _closestSettlements;

        public AOSettlementDistance()
        {
            _latestHeroPosition = new Vec2(-1f, -1f);
            _sortedSettlements = new List<SettlementDistancePair>(64);
            _closestSettlements = new List<Settlement>(3);
        }

        public List<Settlement> GetClosestSettlements(Vec2 position)
        {
            if (!position.NearlyEquals(_latestHeroPosition, 1E-05f))
            {
                _latestHeroPosition = position;
                MBReadOnlyList<Settlement> all = Settlement.All;
                int count = all.Count;
                for (int index = 0; index < count; ++index)
                {
                    Settlement settlement = all[index];
                    if (settlement.IsTown)
                    {
                        _sortedSettlements.Add(new SettlementDistancePair(position.DistanceSquared(settlement.GatePosition), settlement));
                    }
                }
                _sortedSettlements.Sort();
                _closestSettlements.Clear();
                _closestSettlements.Add(_sortedSettlements[0].Settlement);
                _closestSettlements.Add(_sortedSettlements[1].Settlement);
                _closestSettlements.Add(_sortedSettlements[2].Settlement);
                _sortedSettlements.Clear();
            }
            return _closestSettlements;
        }

        private struct SettlementDistancePair : IComparable<SettlementDistancePair>
        {
            private float _distance;
            public Settlement Settlement;

            public SettlementDistancePair(float distance, Settlement settlement)
            {
                _distance = distance;
                Settlement = settlement;
            }

            public int CompareTo(SettlementDistancePair other)
            {
                return (double) _distance == other._distance ? 0 : (double) _distance > other._distance ? 1 : -1;
            }
        }
    }
}