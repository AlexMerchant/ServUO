#region References
using System;
using Server.Mobiles;
#endregion

namespace Server.Items
{
    public class MaxSkillGate : Item
    {

        [Constructable]
        public MaxSkillGate()
            : base(0xF6C)
        {
            Name = "Max Skill Gate";
            Movable = false;
            Light = LightType.Circle300;
            Hue = 316;
        }

        public MaxSkillGate(Serial serial)
            : base(serial)
        { }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 0))
            {
                UseGate(m);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player && m.CanSee(this))
            {
                UseGate(m);
            }

            return m.Map == Map && m.InRange(this, 1);
        }

        public virtual bool CanUseGate(Mobile m, bool message)
        {
            // TODO: Per expansion, calculate expected max skill total and if mobile matches, then reject

            /*if (m.SkillsTotal == 0)
            {
                m.SendMessage("Your skills have already been reset.");
                return false;
            }*/

            if (m.Spell != null)
            {
                // You are too busy to do that at the moment.
                m.SendLocalizedMessage(1049616);
                return false;
            }

            return true;
        }

        public bool UseGate(Mobile m)
        {
            if (!CanUseGate(m, true))
            {
                return false;
            }

            // Loop through all skills and set their base values to 0
            Skills m_Skills = m.Skills;
            for (int i = 0; i < m_Skills.Length; i++)
            {
                // TODO: Add skill exclusions based on enabled Expansion
                if (Core.AOS)
                {
                    // Exclude Bushido, Ninjitsu, Spellweaving, Imbuing, Mysticism etc.
                    if (m_Skills[i].SkillName == SkillName.Bushido ||
                        m_Skills[i].SkillName == SkillName.Ninjitsu ||
                        m_Skills[i].SkillName == SkillName.Spellweaving ||
                        m_Skills[i].SkillName == SkillName.Imbuing ||
                        m_Skills[i].SkillName == SkillName.Mysticism )
                    {
                        m_Skills[i].Base = 0;
                        continue;
                    }
                }

                //if (Core.SE) { }

                //if (Core.ML) { }

                // END-TODO

                m_Skills[i].Base = 120;
            }

            //PlaySound(m);

            return true;
        }

        /*public virtual void PlaySound(Mobile m)
        {
            if (!m.Hidden || m.IsPlayer())
            {
                Effects.PlaySound(m.Location, m.Map, 0x20E);
            }
            else
            {
                m.SendSound(0x20E);
            }
        }*/

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
