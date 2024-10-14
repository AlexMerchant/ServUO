#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Accounting;
using Server.ContextMenus;
using Server.Items;
using Server.Network;

using Acc = Server.Accounting.Account;
#endregion

namespace Server.Mobiles
{
    public class SRBanker : Banker
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public SRBanker()
            : base()
        { 
            this.Name = "Pharyn";
        }

        public SRBanker(Serial serial)
            : base(serial)
        { }

        public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }


        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBanker());
        }

#region Speech Handlers
        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 12))
            {
                return true;
            }

            return base.HandlesOnSpeech(from);
        }

	    public override void OnSpeech(SpeechEventArgs e)
	    {
		    HandleSpeech(this, e);

		    base.OnSpeech(e);
	    }

	    public static void HandleSpeech(Mobile vendor, SpeechEventArgs e)
	    {
            if (!e.Handled && e.Mobile.InRange(vendor, 12))
			{
                if (e.Mobile.Map.Rules != MapRules.FeluccaRules && vendor is BaseVendor && !((BaseVendor)vendor).CheckVendorAccess(e.Mobile))
                {
                    return;
                }

				foreach (var keyword in e.Keywords)
				{
					switch (keyword)
					{
						case 0x0000: // *withdraw*
							{
								e.Handled = true;

								/* Disabling Criminal status check
                                if (e.Mobile.Criminal)
								{
									// I will not do business with a criminal!
									vendor.Say(500389);
									break;
								}
                                */

								var split = e.Speech.Split(' ');

								if (split.Length >= 2)
								{
									int amount;

									var pack = e.Mobile.Backpack;

									if (!int.TryParse(split[1], out amount))
									{
										break;
									}

									if ((!Core.ML && amount > 5000) || (Core.ML && amount > 60000))
									{
										// Thou canst not withdraw so much at one time!
										vendor.Say(500381);
									}
									else if (pack == null || pack.Deleted || !(pack.TotalWeight < pack.MaxWeight) ||
											 !(pack.TotalItems < pack.MaxItems))
									{
										// Your backpack can't hold anything else.
										vendor.Say(1048147);
									}
									else if (amount > 0)
									{
										var box = e.Mobile.Player ? e.Mobile.BankBox : e.Mobile.FindBankNoCreate();

										if (box == null || !Withdraw(e.Mobile, amount))
										{
											// Ah, art thou trying to fool me? Thou hast not so much gold!
											vendor.Say(500384);
										}
										else
										{
											pack.DropItem(new Gold(amount));

											// Thou hast withdrawn gold from thy account.
											vendor.Say(1010005);
										}
									}
								}
							}
							break;
						case 0x0001: // *balance*
							{
								e.Handled = true;

								/* Disabling Criminal status check
                                if (e.Mobile.Criminal)
								{
									// I will not do business with a criminal!
									vendor.Say(500389);
									break;
								}
                                */

								if (AccountGold.Enabled && e.Mobile.Account is Account)
								{
                                    vendor.Say(1155855, String.Format("{0:#,0}\t{1:#,0}",
                                        e.Mobile.Account.TotalPlat,
                                        e.Mobile.Account.TotalGold), 0x3BC);

                                    vendor.Say(1155848, String.Format("{0:#,0}", ((Account)e.Mobile.Account).GetSecureAccountAmount(e.Mobile)), 0x3BC);
								}
								else
								{
									// Thy current bank balance is ~1_AMOUNT~ gold.
									vendor.Say(1042759, GetBalance(e.Mobile).ToString("#,0"));
								}
							}
							break;
						case 0x0002: // *bank*
							{
								e.Handled = true;

								/* Disabling Criminal status check
                                if (e.Mobile.Criminal)
								{
									// Thou art a criminal and cannot access thy bank box.
									vendor.Say(500378);
									break;
								}
                                */

								e.Mobile.BankBox.Open();
							}
							break;
						case 0x0003: // *check*
							{
								e.Handled = true;

                                /* Disabling Criminal status check
								if (e.Mobile.Criminal)
								{
									// I will not do business with a criminal!
									vendor.Say(500389);
									break;
								}
                                */

								if (AccountGold.Enabled && e.Mobile.Account != null)
								{
									vendor.Say("We no longer offer a checking service.");
									break;
								}

								var split = e.Speech.Split(' ');

								if (split.Length >= 2)
								{
									int amount;

									if (!int.TryParse(split[1], out amount))
									{
										break;
									}

									if (amount < 5000)
									{
										// We cannot create checks for such a paltry amount of gold!
										vendor.Say(1010006);
									}
									else if (amount > 1000000)
									{
										// Our policies prevent us from creating checks worth that much!
										vendor.Say(1010007);
									}
									else
									{
										var check = new BankCheck(amount);

										var box = e.Mobile.BankBox;

										if (!box.TryDropItem(e.Mobile, check, false))
										{
											// There's not enough room in your bankbox for the check!
											vendor.Say(500386);
											check.Delete();
										}
										else if (!box.ConsumeTotal(typeof(Gold), amount))
										{
											// Ah, art thou trying to fool me? Thou hast not so much gold!
											vendor.Say(500384);
											check.Delete();
										}
										else
										{
											// Into your bank box I have placed a check in the amount of:
											vendor.Say(1042673, AffixType.Append, amount.ToString("#,0"), "");
										}
									}
								}
							}
							break;
					}
				}
			}
	    }
#endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}