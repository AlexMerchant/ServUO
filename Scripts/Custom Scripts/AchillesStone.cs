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
                m_IsOwned = value;
            }
        }

        [CommandProperty(AccessLevel.Owner)]
        public Mobile Owner
        {
            get { return m_Owner; }
            set
            {
                m_Owner = value;
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
            IsOwned = false;
        }

        [Constructable]
        public AchillesStone(Serial serial)
            : base(serial)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Serial: {0} / Mobile: {1}", from.Serial.Value, from);

            // Checks if the AchillesStone already has an owner
            if (!IsOwned && !(Owner != null))
            {
                this.Owner = from;
                this.OwnerAccessLevel = from.AccessLevel;
                this.IsOwned = true;
                from.SendMessage("Owner set");
            }
        }

        // TODO: Read custom properties into when being deserialized
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        // TODO: Write custom properties into when being serialized
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }
    }
}
