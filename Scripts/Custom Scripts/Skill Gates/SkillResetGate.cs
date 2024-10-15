#region References
using System;
using Server.Mobiles;
#endregion

namespace Server.Items
{
    public class SkillResetGate : Item
    {

        [Constructable]
        public SkillResetGate()
            : base(0xF6C)
        {
            Name = "Skill Reset Gate";
            Movable = false;
            Light = LightType.Circle300;
            Hue = 916;
        }

        public SkillResetGate(Serial serial)
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

            if (m.SkillsTotal == 0)
            {
                m.SendMessage("Your skills have already been reset.");
                return false;
            }

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
                m_Skills[i].Base = 0;
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
