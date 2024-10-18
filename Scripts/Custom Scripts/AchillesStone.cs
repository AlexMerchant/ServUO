using System;
using Server.Mobiles;

namespace Server.Items
{
    public class AchillesStone : Item
    {
        private bool m_IsOwned;
        private Mobile m_Owner;
        private AccessLevel m_OwnerAccessLevel;

        [CommandProperty(AccessLevel.Owner)]
        public bool IsOwned
        {
            get { return m_IsOwned; }
            set
            {
                // Prevents m_IsOwned from being set to 'true' if m_Owner is 'null' and vice-versa
                if (value && this.m_Owner == null)
                {
                    m_IsOwned = false;

                }
                else if (!value && this.m_Owner != null)
                {
                    m_IsOwned = true;
                }
                else
                {
                    m_IsOwned = value;
                }
            }
        }

        [CommandProperty(AccessLevel.Owner)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set
            {
                this.m_Owner = value;

                if (value == null)
                {
                    this.IsOwned = false;
                    this.OwnerAccessLevel = AccessLevel.Player;
                }
                else
                {
                    m_IsOwned = true;
                }
            }
        }

        [CommandProperty(AccessLevel.Owner)]
        public AccessLevel OwnerAccessLevel
        {
            get { return m_OwnerAccessLevel; }
            set { m_OwnerAccessLevel = value; }
        }

        [Constructable]
        public AchillesStone()
            : base(0x3192) // ItemID: 12690
        {
            Name = "a stone of mortality";

        }

        [Constructable]
        public AchillesStone(Serial serial)
            : base(serial)
        {

        }

        // TODO: Method to check if the stone clicker is the stone owner
        private bool IsUserOwner(Mobile m_Owner, Mobile pm_User)
        {
            return m_Owner.Equals(pm_User);
        }

        public override void OnDoubleClick(Mobile pm_Clicker)
        {
            // Checks if the AchillesStone already has an owner
            if (!IsOwned && !(Owner != null))
            {
                this.Owner = pm_Clicker;
                this.OwnerAccessLevel = pm_Clicker.AccessLevel;
                this.IsOwned = true;
                pm_Clicker.SendMessage("Your soul has been become bound to the stone.");
                return;
            }

            // TODO: Check if 'pm_Clicker' is 'm_Owner'; wrap this around rest of method
            if (IsUserOwner(this.Owner, pm_Clicker))
            {
                // If the registered owner's current AccessLevel is the same as the stored AccessLevel on the stone, then toggle the owner's AccessLevel to Player
                if (m_OwnerAccessLevel.Equals(pm_Clicker.AccessLevel))
                {
                    pm_Clicker.AccessLevel = AccessLevel.Player;
                    return;
                }

                // If the AccessLevel comparison above returns false, then toggle the owner's AccessLevel to Player.
                pm_Clicker.AccessLevel = m_OwnerAccessLevel;

            } else // 'pm_Clicker' is not 'm_Owner'
            {
                pm_Clicker.SendMessage("You squeeze the stone and nothing happens. Stupid rock.");
                return;
            }
        }

        // TODO: Add method to handle attempts to move item; if stone is owned the mover must be the Owner

        // TODO: Read custom properties into when being deserialized
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_IsOwned = reader.ReadBool();
            m_Owner = reader.ReadMobile();
            m_OwnerAccessLevel = (AccessLevel) reader.ReadInt();
        }

        // TODO: Write custom properties into when being serialized
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_IsOwned);
            writer.Write(m_Owner);
            writer.Write((int)m_OwnerAccessLevel);
        }
    }
}
