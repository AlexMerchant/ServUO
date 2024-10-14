using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class FisherSmith : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public FisherSmith()
            : base("the fishersmith")
        {
            this.SetSkill(SkillName.Fishing, 100.0, 120.0);
            this.SetSkill(SkillName.Blacksmith, 100.0, 120.0);
        }

        public FisherSmith(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.FishermensGuild;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBFisherman());
            this.m_SBInfos.Add(new SBBlacksmith());
        }

        // public override void InitOutfit()
        // {
        //     base.InitOutfit();

        //     this.AddItem(new Server.Items.FishingPole());
        // }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}