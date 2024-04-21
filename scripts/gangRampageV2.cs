using GTA;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GangRampageCS
{
	public class EnemyScript : Script
	{
		// Initalize our variables.
		public Blip blip;
		private string[]vehsHi = { "sentinel", "pmp600" };
		private string[]vehsLo = { "pony", "marbella" };
		private string[]pedsHi = { "M_Y_GMaf_Hi_01","M_Y_GMaf_Hi_02" };
		private string[]pedsLo = { "M_Y_GMaf_Lo_01","M_Y_GMaf_Lo_02" };
		private string[]weaponsHi = { "Handgun_DesertEagle", "Shotgun_Basic" };
		private string[]weaponsLo = { "HANDGUN_GLOCK", "SMG_UZI" };
		private string[]pedsAlly = { "F_M_PJERSEY_02", "F_Y_BUSINESS_01", "F_Y_PCOOL_01", "F_Y_WAITRESS_01", "F_Y_PMANHAT_02", "F_Y_PMANHAT_03", "M_Y_PCOOL_01", "M_M_PRICH_01", "M_Y_BUSINESS_01", "M_Y_BUSINESS_02", "M_M_PEASTEURO_01", "F_Y_GYMGAL_01", "F_Y_BANK_01", "F_Y_PCOOL_02", "F_Y_VILLBO_01", "F_Y_SHOP_04", "F_Y_STREET_02", "F_Y_STREET_05", "F_Y_STREET_09", "F_Y_STREET_12", "F_Y_STREET_34", "F_Y_HOOKER_03", "F_O_PJERSEY_01", "F_Y_PHARLEM_01", "F_Y_SHOPPER_05", "M_Y_BRONX_01", "M_Y_HARLEM_01", "M_Y_PBRONX_01", "M_Y_PINDUS_02", "M_Y_PJERSEY_01", "M_M_PLATIN_03", "M_M_PLATIN_01", "M_Y_CONSTRUCT_01", "M_Y_CONSTRUCT_03", "M_Y_PCOOL_02", "M_Y_STREETBLK_02", "M_M_TRAMPBLACK", "M_Y_DODGY_01", "M_M_GENBUM_01", "M_Y_PHARLEM_01"};
		private string[]weaponsAlly = { "HANDGUN_GLOCK", "SMG_UZI", "SMG_MP5" };
		int maxEnemies = 1;
		int maxAllies = 1;
		private static readonly RelationshipGroup relationshipGroupEnemies = RelationshipGroup.NetworkPlayer_32;
		private static readonly RelationshipGroup relationshipGroupAllies = RelationshipGroup.Player;
		private readonly List<Ped> spawnedEnemies = new List<Ped>();
		private readonly List<Ped> spawnedAlly = new List<Ped>();
		private readonly List<Ped> enemiesToDel = new List<Ped>();
		private readonly List<Ped> alliesToDel = new List<Ped>();
		Random random = new Random();

		enum eState {
			Off,
			Running
		}

		eState state = eState.Off;

		// ===============================================================================================================
		// HELPER FUNCTIONS
		// ===============================================================================================================
		// Dumb to create two of the same functions for single use GettingRandom numbers.
		// Unless I'm missing something here...
		public int GetRandomNumber( int iMax )
		{
			return random.Next( iMax );
		}

		public int GetRandomNumber( int iMin, int iMax )
		{
			return random.Next( iMin, iMax );
		}
		
		// Insert / Merge an array tables into 1.
		// Used for combining all gangs into 1.
		public string[] MergeArrays( params string[][] arrays )
		{
			int totalLength = 0;
			foreach ( string[] array in arrays )
			{
				totalLength += array.Length;
			}
			string[] result = new string[ totalLength ];

			int currentIndex = 0;
			foreach ( string[] array in arrays )
			{
				Array.Copy( array, 0, result, currentIndex, array.Length );
				currentIndex += array.Length;
			}

			return result;
		}

		// Get the current game episode the player currently loaded
		// so we can have specific vehicles/peds/weapons for the episode
		public GameEpisode GetCurrentEpisode()
		{
			return Game.CurrentEpisode;
		}

		// Pick a random array option
		private T RandomChoice<T>( T[] array )
		{
			return array[ GetRandomNumber( 0, array.Length ) ];
		}

		public void setGangs()
		{
			int[]ints = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			int selectGang = RandomChoice( ints );
			maxEnemies = GetRandomNumber( 4, 10 );
			//maxAllies = GetRandomNumber( 3, 6 );

			Dictionary<int, string> gangMessages = new Dictionary<int, string>
			{
				{ 0, "Badman has to teach a rude boy a ting." },
				{ 1, "N. Holland Husters has got some beef with you." },
				{ 2, "Irish Mobsters are looking for you." },
				{ 3, "Albanian Mobsters are after you." },
				{ 4, "Dimitri has sent out a hitsquad after you!" },
				{ 5, "The Mafia is onto you!" },
				{ 6, "The Triads want you dead!" },
				{ 7, "The Korean Mob is after you, waste them!" },
				{ 8, "The Spanish Lords want you dead!" },
				{ 9, "Some Eastern goons are after you!" },
				{ 10, "Angels of Death is after you!" },
				{ 11, "You're wanted by the LCPD!" },
				{ 12, "Sexy Ladies are after you!" },
				{ 13, "The locals had enough of you!" },
				{ 14, "The service people are after you!" },
				{ 15, "The Lost MC is after you!" },
				{ 16, "Everyone is after you!" },
			};

			if ( gangMessages.ContainsKey( selectGang ) )
			{
				// an " " is here to make the text look pretty.
				// So it doesn't look like "x is looking for you.4 Active goons.
				Game.DisplayText( gangMessages[ selectGang ] + " " + maxEnemies + " Active goons!", 5000 );
			}

			string[]vehsHiJam = { "huntley" };
			string[]vehsLoJam = { "voodoo", "VIRGO" };
			string[]pedsHiJam = { "M_M_GJam_Hi_01", "M_M_GJam_Hi_02", "M_M_GJam_Hi_03", "IG_LILJACOB", "IG_BADMAN" };
			string[]pedsLoJam = { "M_Y_GJam_Lo_01","M_Y_GJam_Lo_02" };
			string[]weaponsHiJam = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "SMG_UZI", "SMG_MP5", "SHOTGUN_BASIC" };
			string[]weaponsLoJam = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI" };

			string[]vehsHiAfr = { "patriot", "LANDSTALKER", "cavalcade" };
			string[]vehsLoAfr = { "emperor", "manana", "pres", "STALION" };
			string[]pedsHiAfr = { "M_Y_GAfr_Hi_01", "M_Y_GAfr_Hi_02", "IG_PLAYBOY_X", "IG_DWAYNE" };
			string[]pedsLoAfr = { "M_Y_GAfr_Lo_01", "M_Y_GAfr_Lo_02", "M_Y_PHARLEM_01" };
			string[]weaponsHiAfr = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SHOTGUN_BASIC", "SMG_UZI" };
			string[]weaponsLoAfr = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI" };

			string[]vehsHiIri = { "ORACLE", "E109" };
			string[]vehsLoIri = { "feroci", "minivan" };
			string[]pedsHiIri = { "M_Y_GIri_Lo_01", "M_Y_GIri_Lo_02", "M_Y_GIri_Lo_03", "IG_PACKIE_MC", "IG_GORDON" };
			string[]pedsLoIri = { "M_Y_GIri_Lo_01", "M_Y_GIri_Lo_02", "M_Y_GIri_Lo_03" };
			string[]weaponsHiIri = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "HANDGUN_GLOCK", "RIFLE_AK47", "SMG_MP5" };
			string[]weaponsLoIri = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI" };

			string[]vehsHiAlb = { "HAKUMAI", "FUTO", "WILLARD" };
			string[]vehsLoAlb = { "HAKUMAI", "FUTO", "schafter" };
			string[]pedsHiAlb = { "M_Y_GAlb_Lo_01","M_Y_GAlb_Lo_02","M_Y_GAlb_Lo_03","M_Y_GAlb_Lo_04" };
			string[]pedsLoAlb = { "M_Y_GAlb_Lo_01","M_Y_GAlb_Lo_02","M_Y_GAlb_Lo_03","M_Y_GAlb_Lo_04" };
			string[]weaponsHiAlb = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "HANDGUN_DESERTEAGLE", "SHOTGUN_BARETTA" };
			string[]weaponsLoAlb = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI" };

			string[]vehsHiRus = { "rebla", "schafter", "LANDSTALKER", "uranus" };
			string[]vehsLoRus = { "willard", "esperanto", "uranus", "ingot" };
			string[]pedsHiRus = { "M_Y_GRus_Hi_02", "M_O_GRus_Hi_01", "M_M_GRu2_Hi_01", "M_M_GRu2_Hi_02", "IG_DMITRI", "IG_FAUSTIN" };
			string[]pedsLoRus = { "M_Y_GRus_Lo_02", "M_Y_GRus_Lo_01", "M_Y_GRu2_Lo_01", "M_M_GRu2_Lo_02" };
			string[]weaponsHiRus = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "RIFLE_AK47" };
			string[]weaponsLoRus = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "RIFLE_AK47" };

			string[]vehsHiMaf = { "sentinel", "pmp600", "huntley" };
			string[]vehsLoMaf = { "pony", "marbella", "schafter", "VINCENT" };
			string[]pedsHiMaf = { "M_Y_GMaf_Hi_01","M_Y_GMaf_Hi_02", "M_Y_GOON_01" };
			string[]pedsLoMaf = { "M_Y_GMaf_Lo_01","M_Y_GMaf_Lo_02", "M_Y_GOON_01" };
			string[]weaponsHiMaf = { "Handgun_DesertEagle", "Handgun_DesertEagle", "SHOTGUN_BASIC" , "RIFLE_AK47" };
			string[]weaponsLoMaf = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_MP5", "RIFLE_AK47" };

			string[]vehsHiTri = { "intruder", "feroci" };
			string[]vehsLoTri = { "pony", "marbella", "huntley" };
			string[]pedsHiTri = { "M_M_GTri_Hi_01", "M_M_GTri_Hi_02" };
			string[]pedsLoTri = { "M_Y_GTri_Lo_01", "M_Y_GTri_Lo_02" };
			string[]weaponsHiTri = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "SMG_UZI", "SHOTGUN_BASIC", "Shotgun_Baretta", "RIFLE_AK47", "RIFLE_AK47" };
			string[]weaponsLoTri = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Handgun_DesertEagle", "SMG_MP5" };

			string[]vehsHiKor = { "sultan", "pres", "banshee" };
			string[]vehsLoKor = { "sultan", "pres" };
			string[]pedsHiKor = { "M_Y_Gkor_Lo_01","M_Y_Gkor_Lo_02" };
			string[]pedsLoKor = { "M_Y_Gkor_Lo_01","M_Y_Gkor_Lo_02" };
			string[]weaponsHiKor = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_MP5", "SHOTGUN_BASIC", "RIFLE_M4", "RIFLE_AK47" };
			string[]weaponsLoKor = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI"};

			string[]vehsHiLat = { "primo", "cavalcade", "FXT" };
			string[]vehsLoLat = { "sabre", "peyote" };
			string[]pedsHiLat = { "M_Y_GLAT_Hi_01","M_Y_GLAT_Hi_02" };
			string[]pedsLoLat = { "M_Y_GLAT_Lo_01","M_Y_GLAT_Lo_02" };
			string[]weaponsHiLat = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_MP5", "SHOTGUN_BASIC", "RIFLE_AK47"};
			string[]weaponsLoLat = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI" };

			string[]vehsHiHEAVY = { "FORTUNE","FEROCI" };
			string[]vehsLoHEAVY = { "FORTUNE","FEROCI" };
			string[]pedsHiHEAVY = { "M_M_EE_HEAVY_01","M_M_EE_HEAVY_02","M_M_FATMOB_01" };
			string[]pedsLoHEAVY = { "M_M_EE_HEAVY_01","M_M_EE_HEAVY_02","M_M_FATMOB_01" };
			string[]weaponsHiHEAVY = { "HANDGUN_GLOCK", "SMG_MP5", "RIFLE_AK47", "SHOTGUN_BARETTA" };
			string[]weaponsLoHEAVY = { "HANDGUN_GLOCK" };

			string[]vehsHiBik = { "hellfury", "zombieb" };
			string[]vehsLoBik = { "hellfury", "zombieb", "emperor" };
			string[]pedsHiBik = { "M_Y_GBik_Hi_01","M_Y_GBik_Hi_02" };
			string[]pedsLoBik = { "M_Y_GBik02_Lo_02","M_M_GBik_Lo_03 "};
			string[]weaponsHiBik = { "HANDGUN_GLOCK", "Handgun_DesertEagle", "Shotgun_Basic" };
			string[]weaponsLoBik = { "HANDGUN_GLOCK", "SMG_UZI"};

			string[]vehsHiLCPD = { "NOOSE", "NOOSE", "POLPATRIOT", "FBI", "PSTOCKADE" };
			string[]vehsLoLCPD = { "POLICE","POLICE2"};
			string[]pedsHiLCPD = { "M_Y_COP", "M_Y_COP", "M_Y_STROOPER", "M_Y_STROOPER", "M_M_FBI", "M_Y_SWAT"};
			string[]pedsLoLCPD = { "M_M_FATCOP_01", "M_Y_COP", "M_Y_STROOPER", "M_Y_COP_TRAFFIC"};
			string[]weaponsHiLCPD = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Shotgun_Basic", "Shotgun_Basic", "RIFLE_M4", "RIFLE_M4", "SMG_MP5", "SMG_MP5", "SniperRifle_Basic", "SniperRifle_Basic"};
			string[]weaponsLoLCPD = { "HANDGUN_GLOCK" };

			string[]vehsHiStr = { "sultan", "banshee", "ORACLE" };
			string[]vehsLoStr = { "primo","FEROCI"};
			string[]pedsHiStr = { "F_Y_STRIPPERC01", "F_Y_STRIPPERC02" };
			string[]pedsLoStr = { "F_Y_STRIPPERC01", "F_Y_STRIPPERC02" };
			string[]weaponsHiStr = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Shotgun_Basic", "SMG_MP5" , "RIFLE_AK47"};
			string[]weaponsLoStr = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_MP5" };

			string[]vehsHiLCP = { "intruder", "huntley", "cavalcade", "TAXI", "TAXI2", "DUKES" };
			string[]vehsLoLCP = { "FEROCI", "esperanto", "admiral", "pcj", "sanchez", "NRG900", "pony", "STALION", "sabre", "RANCHER" };
			string[]pedsHiLCP = { "F_M_PJERSEY_02", "F_Y_BUSINESS_01", "F_Y_PCOOL_01", "F_Y_WAITRESS_01", "F_Y_PMANHAT_02", "F_Y_PMANHAT_03", "M_Y_PCOOL_01", "M_M_PRICH_01", "M_Y_BUSINESS_01", "M_Y_BUSINESS_02", "M_M_PEASTEURO_01", "M_M_TELEPHONE" };
			string[]pedsLoLCP = { "F_Y_GYMGAL_01", "F_Y_BANK_01", "F_Y_PCOOL_02", "F_Y_VILLBO_01", "F_Y_STREET_02", "F_Y_STREET_05", "F_Y_STREET_09", "F_Y_STREET_12", "F_Y_STREET_34", "F_Y_HOOKER_03", "F_O_PJERSEY_01", "F_Y_PHARLEM_01", "F_Y_SHOPPER_05", "F_Y_TOURIST_01", "M_Y_BRONX_01", "M_Y_HARLEM_01", "M_Y_PBRONX_01", "M_Y_PINDUS_02", "M_Y_DRUG_01", "M_Y_PJERSEY_01", "M_M_PLATIN_03", "M_M_PLATIN_01", "M_Y_CONSTRUCT_01", "M_Y_CONSTRUCT_03", "M_M_TAXIDRIVER", "M_Y_PCOOL_02", "M_Y_STREETBLK_02", "M_M_TRAMPBLACK", "M_Y_DODGY_01", "M_M_GENBUM_01", "M_Y_PHARLEM_01", "M_M_FACTORY_01", "M_Y_COURIER", "M_O_JANITOR", "M_M_ARMOURED", "M_M_SECURITYMAN" };
			string[]weaponsHiLCP = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Shotgun_Basic", "SMG_MP5", "SMG_UZI", "RIFLE_M4"};
			string[]weaponsLoLCP = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "SMG_MP5", "Shotgun_Basic" };

			string[]vehsHiSrv = { "intruder", "huntley", "cavalcade", "TAXI", "TAXI2", "DUKES" };
			string[]vehsLoSrv = { "FEROCI", "esperanto", "admiral", "pcj", "sanchez", "NRG900", "pony", "STALION", "sabre", "RANCHER" };
			string[]pedsHiSrv = { "F_Y_WAITRESS_01", "F_Y_BANK_01", "F_Y_DOCTOR_01","F_Y_STRIPPERC01", "F_Y_STRIPPERC02", "F_Y_SHOP_04", "M_M_FIRECHIEF", "M_Y_CLUBFIT", "M_Y_CONSTRUCT_01", "M_Y_CONSTRUCT_02", "M_Y_CONSTRUCT_03", "M_Y_BOUNCER_01", "M_M_DOCTOR_01"};
			string[]pedsLoSrv = { "F_Y_NURSE", "F_Y_FF_BURGER_R", "F_Y_FF_CLUCK_R", "F_Y_FF_RSCAFE", "F_Y_FF_TWCAFE", "F_Y_FF_WSPIZZA_R", "M_Y_COP", "M_Y_VENDOR", "M_Y_CHINVEND_01", "M_Y_PMEDIC", "M_M_DOC_SCRUBS_01", "M_Y_FF_BURGER_R", "M_Y_FF_CLUCK_R", "M_Y_FF_RSCAFE", "M_Y_FF_TWCAFE", "M_Y_FF_WSPIZZA_R", "M_M_TAXIDRIVER", "M_Y_BOWL_01", "M_Y_BOWL_02", "M_M_SWEEPER", "M_M_STREETFOOD_01", "M_M_FACTORY_01", "M_Y_COURIER", "M_O_JANITOR", "M_M_ARMOURED", "M_M_SECURITYMAN", "M_M_POSTAL_01", "M_M_TELEPHONE" };
			string[]weaponsHiSrv = { "HANDGUN_GLOCK", "Handgun_DesertEagle", "Shotgun_Basic", "SMG_MP5", "SMG_UZI", "RIFLE_AK47", "RIFLE_M4" };
			string[]weaponsLoSrv = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Shotgun_Basic", "SMG_MP5", "RIFLE_M4" };

			string[]vehsHiBikMC = { "hellfury", "zombieb" };
			string[]vehsLoBikMC = { "hellfury", "zombieb" };
			string[]pedsHiBikMC = { "M_Y_GBik_Lo_01","M_Y_GBik_Lo_02" };
			string[]pedsLoBikMC = { "M_Y_GBik_Lo_01","M_Y_GBik_Lo_02" };
			string[]weaponsHiBikMC = { "HANDGUN_GLOCK", "Shotgun_Basic" };
			string[]weaponsLoBikMC = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI"};

			string[]vehsHiCombined = { "sultan", "intruder", "huntley", "cavalcade", "FORTUNE", "pmp600", "WILLARD" };
			string[]vehsLoCombined = { "primo", "FEROCI", "esperanto", "admiral", "pcj", "sanchez", "NRG900", "schafter", "sabre", "POLICE", "POLICE2", "pony", "STALION", "RANCHER" };
			string[]weaponsHiCombined = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "HANDGUN_GLOCK", "Handgun_DesertEagle", "Shotgun_Basic", "SMG_MP5", "SMG_UZI", "RIFLE_M4", "RIFLE_AK47", "SHOTGUN_BARETTA"};
			string[]weaponsLoCombined = { "HANDGUN_GLOCK", "HANDGUN_GLOCK", "HANDGUN_GLOCK", "SMG_UZI", "SMG_MP5", "Shotgun_Basic" };

			// Unique ped / vehicle / weapons for certain gangs for episodic content
			if ( GetCurrentEpisode() == GameEpisode.TLAD )
			{
				// BIKER GANG - AOD
				// ====================================================================================================================================================
				string[] additionalVehs_HiBik = { "ANGEL", "HEXER", "DAEMON", "NIGHTBLADE" };
				string[] additionalVehs_LoBik = { "ANGEL", "DAEMON", "GBURRITO", "REGINA" };
				string[] additionalPeds_HiBik = { "M_Y_GBik_Hi_01", "M_Y_GBik_Hi_02", "M_Y_GANGELS_02", "M_Y_GANGELS_03" };
				string[] additionalPeds_LoBik = { "M_Y_GANGELS_02", "M_Y_GANGELS_03", "M_Y_GANGELS_04", "M_Y_GANGELS_05", "M_Y_GANGELS_06", "F_Y_GANGELS_01", "F_Y_GANGELS_02", "F_Y_GANGELS_03" };
				string[] additionalWeaponsHi = { "TLAD_Automatic9mm", "TLAD_SawedOffShotgun", "SMG_UZI" };
				string[] additionalWeaponsLo = { "TLAD_Automatic9mm", "TLAD_SawedOffShotgun", "SMG_UZI" };
				vehsHiBik = MergeArrays( vehsHiBik, additionalVehs_HiBik );
				vehsLoBik = MergeArrays( vehsLoBik, additionalVehs_LoBik );
				pedsHiBik = MergeArrays( pedsHiBik, additionalPeds_HiBik );
				pedsLoBik = MergeArrays( pedsLoBik, additionalPeds_LoBik );
				weaponsHiBik = MergeArrays( weaponsHiBik, additionalWeaponsHi );
				weaponsLoBik = MergeArrays( weaponsLoBik, additionalWeaponsLo );

				// TRIAD GANG
				// ====================================================================================================================================================
				string[] additionalPeds_HiTri = { "M_Y_GTRIAD_HI_01" };
				string[] additionalPeds_LoTri = { "M_Y_GTRI_02" };
				string[] additionalWeapons_HiTri = { "TLAD_Automatic9mm", "TLAD_AssaultShotgun" };
				string[] additionalWeapons_LoTri = { "TLAD_Automatic9mm" };
				pedsHiTri = MergeArrays( pedsHiTri, additionalPeds_HiTri );
				pedsLoTri = MergeArrays( pedsLoTri, additionalPeds_LoTri );
				weaponsHiTri = MergeArrays( weaponsHiTri, additionalWeapons_HiTri );
				weaponsLoTri = MergeArrays( weaponsLoTri, additionalWeapons_LoTri );

				// BIKER GANG - MC (HOSTILE)
				// ====================================================================================================================================================
				string[] additionalVehs_HiBikMC = { "LYCAN", "HEXER", "DAEMON", "INNOVATION" };
				string[] additionalVehs_LoBikMC = { "LYCAN", "HEXER", "DAEMON", "DIABOLUS" };
				string[] additionalPeds_HiBikMC = { "IG_BILLY", "IG_TERRY", "IG_CLAY", "IG_JIM_FITZ", "IG_BRIANJ", "IG_ASHLEYA", "M_Y_GLOST_04", "M_Y_GLOST_01", "F_Y_GLOST_02", "F_Y_GLOST_03", "F_Y_GLOST_04" };
				string[] additionalPeds_LoBikMC = { "M_Y_GLOST_01", "M_Y_GLOST_02", "M_Y_GLOST_03", "M_Y_GLOST_04", "M_Y_GLOST_05", "M_Y_GLOST_06", "F_Y_GLOST_01", "F_Y_GLOST_02", "F_Y_GLOST_03", "F_Y_GLOST_04" };
				string[] additionalWeapons_HiBikMC = { "TLAD_Automatic9mm", "TLAD_SawedOffShotgun", "RIFLE_AK47", "SMG_UZI" };
				string[] additionalWeapons_LoBikMC = { "TLAD_Automatic9mm", "TLAD_SawedOffShotgun", "SMG_UZI" };
				vehsHiBikMC = MergeArrays( vehsHiBikMC, additionalVehs_HiBikMC );
				vehsLoBikMC = MergeArrays( vehsLoBikMC, additionalVehs_LoBikMC );
				pedsHiBikMC = MergeArrays( pedsHiBikMC, additionalPeds_HiBikMC );
				pedsLoBikMC = MergeArrays( pedsLoBikMC, additionalPeds_LoBikMC );
				weaponsHiBikMC = MergeArrays( weaponsHiBikMC, additionalWeapons_HiBikMC );
				weaponsLoBikMC = MergeArrays( weaponsLoBikMC, additionalWeapons_LoBikMC );

				// ALLY GANG - LOST
				// ====================================================================================================================================================
				pedsAlly = new string[] { "M_Y_GLOST_01", "M_Y_GLOST_02", "M_Y_GLOST_03", "M_Y_GLOST_04", "M_Y_GLOST_05", "M_Y_GLOST_06", "F_Y_GLOST_01", "F_Y_GLOST_02", "F_Y_GLOST_03", "F_Y_GLOST_04" };
				weaponsAlly = new string[] { "HANDGUN_GLOCK", "SMG_UZI", "Shotgun_Basic", "TLAD_SawedOffShotgun", "TLAD_Automatic9mm" };
				//string[] pedsAlly = { "M_Y_GLOST_01", "M_Y_GLOST_02", "M_Y_GLOST_03", "M_Y_GLOST_04", "M_Y_GLOST_05", "M_Y_GLOST_06", "M_Y_GANGELS_06", "F_Y_GLOST_01", "F_Y_GLOST_02", "F_Y_GLOST_03", "F_Y_GLOST_04" };
				//string[] weaponsAlly = { "HANDGUN_GLOCK", "SMG_UZI", "Shotgun_Basic", "TLAD_SawedOffShotgun", "TLAD_Automatic9mm" };
			}
			
			if ( GetCurrentEpisode() == GameEpisode.TBOGT )
			{
				// POLICE GANG
				// ====================================================================================================================================================
				string[] additionalVehs_HiLCPD = { "POLICE3", "POLICEB", "POLICE4" };
				string[] additionalVehs_LoLCPD = { "POLICE3", "POLICEB" };
				string[] additionalWeapons_HiLCPD = { "TBOGT_NormalShotgun", "TBOGT_AssaultSMG", "TBOGT_AdvancedMG" };
				vehsHiLCPD = MergeArrays( vehsHiLCPD, additionalVehs_HiLCPD );
				vehsLoLCPD = MergeArrays( vehsLoLCPD, additionalVehs_LoLCPD );
				weaponsHiLCPD = MergeArrays( weaponsHiLCPD, additionalWeapons_HiLCPD );

				// AFRICAN GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiAfr = { "TBOGT_GoldenSMG" };
				string[] additionalWeapons_LoAfr = { "TBOGT_GoldenSMG" };
				weaponsHiAfr = MergeArrays( weaponsHiAfr, additionalWeapons_HiAfr );
				weaponsLoAfr = MergeArrays( weaponsLoAfr, additionalWeapons_LoAfr );

				// MAFIA GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiMaf = { "TBOGT_Pistol44", "TBOGT_AdvancedSniper" };
				string[] additionalWeapons_LoMaf = { "TBOGT_Pistol44", "TBOGT_NormalShotgun", "TBOGT_AdvancedMG" };
				weaponsHiMaf = MergeArrays( weaponsHiMaf, additionalWeapons_HiMaf );
				weaponsLoMaf = MergeArrays( weaponsLoMaf, additionalWeapons_LoMaf );

				// KOREAN GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiKor = { "TBOGT_Pistol44", "TBOGT_AssaultSMG" };
				string[] additionalWeapons_LoKor = { "TBOGT_Pistol44", "TBOGT_NormalShotgun", "TBOGT_AssaultSMG" };
				weaponsHiKor = MergeArrays( weaponsHiKor, additionalWeapons_HiKor );
				weaponsLoKor = MergeArrays( weaponsLoKor, additionalWeapons_LoKor );

				// RUSSIAN GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiRus = { "TBOGT_Pistol44", "TBOGT_AdvancedSniper" };
				string[] additionalWeapons_LoRus = { "TBOGT_Pistol44" };
				weaponsHiRus = MergeArrays( weaponsHiRus, additionalWeapons_HiRus );
				weaponsLoRus = MergeArrays( weaponsLoRus, additionalWeapons_LoRus );

				// LATINO GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiLat = { "TBOGT_AssaultSMG" };
				string[] additionalWeapons_LoLat = { "TBOGT_NormalShotgun" };
				weaponsHiLat = MergeArrays( weaponsHiLat, additionalWeapons_HiLat );
				weaponsLoLat = MergeArrays( weaponsLoLat, additionalWeapons_LoLat );

				// STRIPPER GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiStr = { "TBOGT_GoldenSMG" };
				string[] additionalWeapons_LoStr = { "TBOGT_GoldenSMG" };
				weaponsHiStr = MergeArrays( weaponsHiStr, additionalWeapons_HiStr );
				weaponsLoStr = MergeArrays( weaponsLoStr, additionalWeapons_LoStr );

				// TRIAD GANG
				// ====================================================================================================================================================
				string[] additionalWeapons_HiTri = { "TBOGT_NormalShotgun", "TBOGT_Pistol44" };
				string[] additionalWeapons_LoTri = { "TBOGT_Pistol44", "TBOGT_AssaultSMG", "TBOGT_NormalShotgun" };
				weaponsHiTri = MergeArrays( weaponsHiTri, additionalWeapons_HiTri );
				weaponsLoTri = MergeArrays( weaponsLoTri, additionalWeapons_LoTri );

				// TODO: Make a ally gang for Luis (Probably party club, or the latino gang, or a mix of both)
				// ALLY GANG - LUIS
				// ====================================================================================================================================================
			}

			switch ( selectGang )
			{
				case 0: // Jamican Gang
					vehsHi = vehsHiJam;
					vehsLo = vehsLoJam;
					pedsHi = pedsHiJam;
					pedsLo = pedsLoJam;
					weaponsHi = weaponsHiJam;
					weaponsLo = weaponsLoJam;
				break;

				case 1: // African Gang
					vehsHi = vehsHiAfr;
					vehsLo = vehsLoAfr;
					pedsHi = pedsHiAfr;
					pedsLo = pedsLoAfr;
					weaponsHi = weaponsHiAfr;
					weaponsLo = weaponsLoAfr;
				break;

				case 2: // Irish Gang
					vehsHi = vehsHiIri;
					vehsLo = vehsLoIri;
					pedsHi = pedsHiIri;
					pedsLo = pedsLoIri;
					weaponsHi = weaponsHiIri;
					weaponsLo = weaponsLoIri;
				break;

				case 3: // Albainian Gang
					vehsHi = vehsHiAlb;
					vehsLo = vehsLoAlb;
					pedsHi = pedsHiAlb;
					pedsLo = pedsLoAlb;
					weaponsHi = weaponsHiAlb;
					weaponsLo = weaponsLoAlb;
				break;

				case 4: // Russian Gang
					vehsHi = vehsHiRus;
					vehsLo = vehsLoRus;
					pedsHi = pedsHiRus;
					pedsLo = pedsLoRus;
					weaponsHi = weaponsHiRus;
					weaponsLo = weaponsLoRus;
				break;

				case 5: // Mafia Gang
					vehsHi = vehsHiMaf;
					vehsLo = vehsLoMaf;
					pedsHi = pedsHiMaf;
					pedsLo = pedsLoMaf;
					weaponsHi = weaponsHiMaf;
					weaponsLo = weaponsLoMaf;
				break;

				case 6: // Triad Gang
					vehsHi = vehsHiTri;
					vehsLo = vehsLoTri;
					pedsHi = pedsHiTri;
					pedsLo = pedsLoTri;
					weaponsHi = weaponsHiTri;
					weaponsLo = weaponsLoTri;
				break;

				case 7: // Korean Gang
					vehsHi = vehsHiKor;
					vehsLo = vehsLoKor;
					pedsHi = pedsHiKor;
					pedsLo = pedsLoKor;
					weaponsHi = weaponsHiKor;
					weaponsLo = weaponsLoKor;
				break;

				case 8: // Latino Gang
					vehsHi = vehsHiLat;
					vehsLo = vehsLoLat;
					pedsHi = pedsHiLat;
					pedsLo = pedsLoLat;
					weaponsHi = weaponsHiLat;
					weaponsLo = weaponsLoLat;
				break;

				case 9: // Eastern Mob Gang
					vehsHi = vehsHiHEAVY;
					vehsLo = vehsLoHEAVY;
					pedsHi = pedsHiHEAVY;
					pedsLo = pedsLoHEAVY;
					weaponsHi = weaponsHiHEAVY;
					weaponsLo = weaponsLoHEAVY;
				break;

				case 10: // AOD Biker Gang
					vehsHi = vehsHiBik;
					vehsLo = vehsLoBik;
					pedsHi = pedsHiBik;
					pedsLo = pedsLoBik;
					weaponsHi = weaponsHiBik;
					weaponsLo = weaponsLoBik;
				break;

				case 11: // LCPD Cop Gang
					vehsHi = vehsHiLCPD;
					vehsLo = vehsLoLCPD;
					pedsHi = pedsHiLCPD;
					pedsLo = pedsLoLCPD;
					weaponsHi = weaponsHiLCPD;
					weaponsLo = weaponsLoLCPD;

					if ( GetRandomNumber( 100 ) > 60 )
						Player.WantedLevel = 1;
					else
						Player.WantedLevel = 2;
				break;

				case 12: // Stripper Gang
					vehsHi = vehsHiStr;
					vehsLo = vehsLoStr;
					pedsHi = pedsHiStr;
					pedsLo = pedsLoStr;
					weaponsHi = weaponsHiStr;
					weaponsLo = weaponsLoStr;
				break;

				case 13: // Local People Gang
					vehsHi = vehsHiLCP;
					vehsLo = vehsLoLCP;
					pedsHi = pedsHiLCP;
					pedsLo = pedsLoLCP;
					weaponsHi = weaponsHiLCP;
					weaponsLo = weaponsLoLCP;
				break;

				case 14: // Service Gang
					vehsHi = vehsHiSrv;
					vehsLo = vehsLoSrv;
					pedsHi = pedsHiSrv;
					pedsLo = pedsLoSrv;
					weaponsHi = weaponsHiSrv;
					weaponsLo = weaponsLoSrv;
				break;

				case 15: // Lost MC Gang
					vehsHi = vehsHiBikMC;
					vehsLo = vehsLoBikMC;
					pedsHi = pedsHiBikMC;
					pedsLo = pedsLoBikMC;
					weaponsHi = weaponsHiBikMC;
					weaponsLo = weaponsLoBikMC;
				break;

				case 16: // All gangs
					// Combine arrays from different cases
					string[] pedsHiCombined = MergeArrays( pedsHiRus, pedsHiAlb, pedsHiIri, pedsHiAfr, pedsHiJam, pedsHiMaf, pedsHiTri, pedsHiKor, pedsHiLat, pedsHiHEAVY, pedsHiBik, pedsHiLCPD, pedsHiStr, pedsHiLCP, pedsHiSrv, pedsHiBikMC );
					string[] pedsLoCombined = MergeArrays( pedsLoRus, pedsLoAlb, pedsLoIri, pedsLoAfr, pedsLoJam, pedsLoMaf, pedsLoTri, pedsLoKor, pedsLoLat, pedsLoHEAVY, pedsLoBik, pedsLoLCPD, pedsLoStr, pedsLoLCP, pedsLoSrv, pedsLoBikMC );

					// Assign combined arrays
					vehsHi = vehsHiCombined;
					vehsLo = vehsLoCombined;
					pedsHi = pedsHiCombined;
					pedsLo = pedsLoCombined;
					weaponsHi = weaponsHiCombined;
					weaponsLo = weaponsLoCombined;
				break;
			}
		}

		public EnemyScript()
		{
			Interval = 1500;
			this.KeyDown += new GTA.KeyEventHandler( this.EnemyScript_KeyDown );
			this.Tick += new EventHandler( this.EnemyScript_Tick );
			Game.DisplayText( "Gang Rampage Script Loaded! Press U to have random hostile encounters!", 5000 );

			/*switch( GetCurrentEpisode() )
			{
				case GameEpisode.GTAIV:
					Game.Console.Print( "Gang Rampage: IV Game Episode detected." );
				break;

				case GameEpisode.TBOGT:
					Game.Console.Print( "Gang Rampage: TBOGT Game Episode detected." );
				break;
				
				case GameEpisode.TLAD:
					Game.Console.Print( "Gang Rampage: TLAD Game Episode detected." );
				break;
			}*/

			Game.Console.Print( "Gang Rampage: DEBUG KEYBIND SPAWNS. 1 = Enemy Foot, 2 = Enemy Car, K = Ally Foot" );
		}

		private void EnemyScript_Tick( object sender, EventArgs e )
		{
			switch ( state )
			{
				case eState.Running:
					int enemiesCanFight = 0;
					int alliesCanFight = 0;
					foreach ( Ped pEnemy in spawnedEnemies )
					{
						float pedDistance = pEnemy.Position.DistanceTo( Player.Character.Position );

						// Check if the enemy is in a vehicle
						bool isInVehicle = pEnemy.isInVehicle();

						// Use a larger distance threshold if the enemy is in a vehicle
						float distanceThreshold = isInVehicle ? 150.0F : 110.0F;

						// Make sure our ped is alive and not far away from the player.
						// If they aren't, then they are able to fight.
						if ( pEnemy.isAliveAndWell && pedDistance < distanceThreshold )
							enemiesCanFight++;
						else // If not, then put them in the pool to be removed
							enemiesToDel.Add( pEnemy );
					}

					// Cull any far peds, or dead peds
					foreach ( Ped pEnemyRemove in enemiesToDel )
					{
						spawnedEnemies.Remove( pEnemyRemove );
						pEnemyRemove.NoLongerNeeded();
					}

					enemiesToDel.Clear();

					// Same thing for Ally Peds.
					foreach ( Ped ally in spawnedAlly )
					{
						float pedAllyDistance = ally.Position.DistanceTo( Player.Character.Position );
						
						if ( ally.isAliveAndWell && pedAllyDistance < 110.0F )
							alliesCanFight++;
						else
							alliesToDel.Add( ally );
					}

					foreach ( Ped allydel in alliesToDel )
					{
						spawnedAlly.Remove( allydel );
						allydel.NoLongerNeeded();
					}

					alliesToDel.Clear();

					//Game.DisplayText("Can Fight Enemies : " + enemiesCanFight);
					if ( enemiesCanFight < maxEnemies )
					{
						Vector3 pos;
						if (GetRandomNumber( 100 ) > 25 )
						{
							pos = Player.Character.Position.Around( GetRandomNumber( 30, 60 ) );
							SpawnFootEnemy( pos );
						}
						else
						{
							pos = Player.Character.Position.Around( GetRandomNumber( 60, 80 ) );
							SpawnVehicleEnemy( pos );
						}
					}

					/* if ( alliesCanFight < maxAllies )
					{
						Vector3 pos;
						if (GetRandomNumber( 100 ) > 75 )
						{
							pos = Player.Character.Position.Around( GetRandomNumber( 30, 60 ) );
							SpawnFootAlly( pos );
						}
						else
						{
							return; // Do not spawn our ally if RNG is low.
						}
					} */
				break;
			}
		}

		private void EnemyScript_KeyDown( object sender, GTA.KeyEventArgs e )
		{
			switch ( e.Key )
			{
				// Pressing U enables/disables the script
				case Keys.U:
				switch ( state )
				{
					// Go ahead and start the script up
					case eState.Off:
						setGangs();
						state = eState.Running;
					break;

					// Let's end the chaos
					case eState.Running:
						Game.DisplayText( "Your attackers gave up. Be weary for more.", 7500 );

						foreach ( Ped member in spawnedEnemies )
						{
							// Clear peds from memory and make them flee
							member.NoLongerNeeded();
							member.Weapons.RemoveAll();
						}
						foreach ( Ped allymember in spawnedAlly )
						{
							// Clear peds from memory and make them flee
							allymember.NoLongerNeeded();
							allymember.Weapons.RemoveAll();
						}

						spawnedEnemies.Clear();
						spawnedAlly.Clear();
						state = eState.Off;
						Player.WantedLevel = 0;

					break;
				}
				break;

				// =======================================================
				// Fun debug helpers
				// =======================================================
				// Pressing 1 manually spawns a foot member
				case Keys.D1:
					if ( state == eState.Running )
						ManualEnemySpawn( "Foot" );
					else
						DplyMsgDebug();
				break;

				// Pressing K manually spawns ally
				case Keys.K:
					if ( state == eState.Running )
						ManualEnemySpawn( "Foot_Ally" );
					else
						DplyMsgDebug();
				break;

				// Pressing 2 manually spawns a vehicle member
				case Keys.D2:
					if ( state == eState.Running )
						ManualEnemySpawn( "Vehicle" );
					else
						DplyMsgDebug();
				break;
			}
		}

		// Helper function to display a message when the script is not running
		private void DplyMsgDebug()
		{
			Game.DisplayText( "Warning: Script must be running to use debug keys!", 5000 );
		}

		// For debug purposes
		private void ManualEnemySpawn( string Type )
		{
			Vector3 pos;
			switch ( Type )
			{
				case "Foot":
					pos = Player.Character.Position.Around( GetRandomNumber( 30, 60 ) );
					SpawnFootEnemy( pos );
				break;

				case "Foot_Ally":
					pos = Player.Character.Position.Around( GetRandomNumber( 30, 60 ) );
					SpawnFootAlly( pos );
				break;

				case "Vehicle":
					pos = Player.Character.Position.Around( GetRandomNumber( 60, 80 ) );
					SpawnVehicleEnemy( pos );
				break;

				default:
					Game.DisplayText( "Invalid enemy type.", 7500 );
				break;
			}
		}

 		private void CreatePedInVehicle( Vehicle enemyVehicle, string pedmodel, bool isElite )
		{
			Ped pEnemy = enemyVehicle.CreatePedOnSeat( enemyVehicle.GetFreeSeat(), pedmodel );
			SetUpPed( pEnemy, isElite );
			pEnemy.WarpIntoVehicle( enemyVehicle, enemyVehicle.GetFreeSeat() );
			pEnemy.Task.DriveTo( Player, 105, false, true );
		}

		private void SpawnVehicleEnemy( Vector3 pos )
		{
			string enemyVehicleModel;
			string enemyPedModel;
			bool isElite = false;

			if ( GetRandomNumber( 100 ) > 60 )
			{
				enemyVehicleModel = RandomChoice( vehsHi );
				enemyPedModel = RandomChoice( pedsHi );
				isElite = true;
			}
			else
			{
				enemyVehicleModel = RandomChoice( vehsLo );
				enemyPedModel = RandomChoice( pedsLo );
			}

			Vehicle enemyVehicle = World.CreateVehicle( enemyVehicleModel, pos );
			enemyVehicle.PlaceOnNextStreetProperly();

			// Set the vehicle's orientation to face the player
			Vector3 directionToPlayer = ( Player.Character.Position - enemyVehicle.Position );
			directionToPlayer.Normalize();
			float heading = ( float )( Math.Atan2( directionToPlayer.X, directionToPlayer.Y ) * -180.0 / Math.PI );
			if ( heading < 0 ) heading += 360;
			enemyVehicle.Heading = heading;

			// Small speed boost (this is stupidly dumb sometimes, the peds can crash into something)
			enemyVehicle.Speed = 6f;

			// Create the driver
			CreatePedInVehicle( enemyVehicle, enemyPedModel, isElite );

			// Array of additionalpassengers for additional peds
			// 50% chance to create the 2nd passengers
			// 32% for the 3rd, 18% for 4th.
 			int[] additionalpassengers = { 50, 32, 18 };

			// Loop over the additionalpassengers
			foreach ( int probability in additionalpassengers )
			{
				// If GetRandomNumber(100) is less than the current probability, create an additional ped
				if ( GetRandomNumber( 100 ) < probability && enemyVehicle.GetFreeSeat() != VehicleSeat.None )
				{
					// Choose a new ped model for each passenger
					// Maybe we should have an asortment for random picks here???
					// But this could screw up the blips...
					string passengerModel;
					if ( isElite )
					{
						passengerModel = RandomChoice( pedsHi );
					}
					else
					{
						passengerModel = RandomChoice( pedsLo );
					}

					CreatePedInVehicle( enemyVehicle, passengerModel, isElite );
				}
			}

			GTA.Native.Function.Call( "SWITCH_CAR_SIREN", enemyVehicle, 1 ); // Wee woo wee woo
			blip = enemyVehicle.AttachBlip();

			if ( isElite )
			{
				blip.Color = BlipColor.Yellow;
				blip.Name = "Elite Hitman Vehicle";
			}
			else
			{
				blip.Color = BlipColor.DarkRed;
				blip.Name = "Hitman Vehicle";
			}

			blip.Display = BlipDisplay.MapOnly;
			blip.Scale = 1F;
			enemyVehicle.NoLongerNeeded();
		}

		// ===============================================================================================================
		// DEBUG
		// ===============================================================================================================
 		/* private void CreatePedInVehicle(Vehicle veh, string pedmodel, bool isHi)
		{
			try
			{
				Ped p = veh.CreatePedOnSeat(veh.GetFreeSeat(), pedmodel);
				SetUpPed(p, isHi);
				p.WarpIntoVehicle(veh, veh.GetFreeSeat());
				p.Task.DriveTo(Player, 75, false, true);
			}
			catch (GTA.NonExistingObjectException)
			{
				throw new Exception(string.Format("Invalid ped model: {0}", pedmodel));
			}
		}

		private void SpawnVehicleEnemy(Vector3 pos)
		{
			string vehmodel;
			string pedmodel;
			bool isHi = false;
			// Randomly pick a ped
			if (GetRandomNumber(100) > 60)
			{
				vehmodel = RandomChoice(vehsHi);
				pedmodel = RandomChoice(pedsHi);
				isHi = true;
			}
			else
			{
				vehmodel = RandomChoice(vehsLo);
				pedmodel = RandomChoice(pedsLo);
			}

			try
			{
				Vehicle veh = World.CreateVehicle(vehmodel, pos);
				veh.PlaceOnNextStreetProperly();

				// Set the vehicle's orientation to face the player
				Vector3 directionToPlayer = (Player.Character.Position - veh.Position);
				directionToPlayer.Normalize();
				float heading = (float)(Math.Atan2(directionToPlayer.X, directionToPlayer.Y) * -180.0 / Math.PI);
				if (heading < 0) heading += 360;
				veh.Heading = heading;

				// Create the driver
				// Create the driver
				CreatePedInVehicle( veh, pedmodel, isHi );

				// Array of additionalpassengers for additional peds
				int[] additionalpassengers = { 50, 32, 15 };

				// Loop over the additionalpassengers
				foreach ( int probability in additionalpassengers )
				{
					// If GetRandomNumber(100) is less than the current probability, create an additional ped
					if ( GetRandomNumber( 100 ) < probability && veh.GetFreeSeat() != VehicleSeat.None )
					{
						// Choose a new ped model for each passenger
						string passengerModel;
						if ( isHi )
						{
							passengerModel = RandomChoice( pedsHi );
						}
						else
						{
							passengerModel = RandomChoice( pedsLo );
						}

						CreatePedInVehicle( veh, passengerModel, isHi );
					}
				}
				GTA.Native.Function.Call("SWITCH_CAR_SIREN", veh, 1);

				blip = veh.AttachBlip();

				if (isHi)
				{
					blip.Color = BlipColor.Yellow;
					blip.Name = "Elite Hitman Vehicle";
				}
				else
				{
					blip.Color = BlipColor.DarkRed;
					blip.Name = "Hitman Vehicle";
				}
				blip.Display = BlipDisplay.MapOnly;
				blip.Scale = 1F;
				veh.NoLongerNeeded();
			}
			catch (GTA.NonExistingObjectException)
			{
				throw new Exception(string.Format("Invalid vehicle model: {0}", vehmodel));
			}
		}*/

		private Ped SpawnFootEnemy( Vector3 pos )
		{
			string enemyPedModel;
			bool isElite = false;

			enemyPedModel = RandomChoice( pedsLo );
			if (GetRandomNumber( 100 ) > 60 )
			{
				enemyPedModel = RandomChoice( pedsHi );
				isElite = true;
			}
			else
			{
				enemyPedModel = RandomChoice( pedsLo );
			}

			Ped pEnemy = World.CreatePed( enemyPedModel, World.GetNextPositionOnPavement( pos ), relationshipGroupEnemies );
			SetUpPed( pEnemy, isElite );

			return pEnemy;
		}

		private Ped SpawnFootAlly( Vector3 pos )
		{
			string allyPedModel;

			allyPedModel = RandomChoice( pedsAlly );

			Ped pAlly = World.CreatePed( allyPedModel, World.GetNextPositionOnPavement( pos ), relationshipGroupAllies );
			SetUpPedAlly( pAlly );

			return pAlly;
		}

		// Set's up the ped information
		private void SetUpPed( Ped pEnemy, bool isElite )
		{	
			if ( !Exists( pEnemy ) ) return;

			blip = pEnemy.AttachBlip();
			if ( isElite )
			{
				blip.Color = BlipColor.Yellow;
				blip.Name = "Elite Hitman";
			}
			else
			{
				blip.Color = BlipColor.DarkRed;
				blip.Name = "Hitman";
			}

			blip.Display = BlipDisplay.MapOnly;
			blip.Scale = 0.65F;

			World.SetGroupRelationship( RelationshipGroup.Player, Relationship.Respect, RelationshipGroup.Player );
			World.SetGroupRelationship( RelationshipGroup.Player, Relationship.Hate, relationshipGroupEnemies );
			World.SetGroupRelationship( relationshipGroupEnemies, Relationship.Respect, relationshipGroupEnemies );
			World.SetGroupRelationship( relationshipGroupEnemies, Relationship.Hate, RelationshipGroup.Player );
			pEnemy.CurrentRoom = Player.Character.CurrentRoom; // required, or ped won't be visible when spawned inside a building

			pEnemy.WillDoDrivebys = true;
			pEnemy.WantedByPolice = true;
			pEnemy.PriorityTargetForEnemies = true;
			pEnemy.SetPathfinding( true, true, true );
			//pEnemy.AlwaysDiesOnLowHealth = true;
			//pEnemy.DuckWhenAimedAtByGroupMember = true;

			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX", pEnemy, 0, GetRandomNumber( 0, 6 ) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX", pEnemy, 1, GetRandomNumber( 0, 6 ) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX_TEXTURE", pEnemy, 0, GetRandomNumber( 0, 6) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX_TEXTURE", pEnemy, 1, GetRandomNumber( 0, 6) );

			pEnemy.RelationshipGroup = relationshipGroupEnemies;
			pEnemy.ChangeRelationship( RelationshipGroup.Player, Relationship.Hate );
			spawnedEnemies.Add( pEnemy );

			pEnemy.Task.ClearAllImmediately();
			pEnemy.Task.AlwaysKeepTask = true;
			pEnemy.Task.FightAgainstHatedTargets( 200.0F ); // Always make sure we have our target

			string sWepName;
			if ( isElite )
			{
				sWepName = RandomChoice( weaponsHi );
			}
			else
			{
				sWepName = RandomChoice( weaponsLo );
			}

			Weapon giveGuns;

			if ( !Enum.TryParse( sWepName, true, out giveGuns ) )
			{
				throw new FormatException( "Weapon name " + sWepName + " does not exist!" );
			}

			pEnemy.Weapons.FromType( giveGuns ).Ammo = 30000;
			pEnemy.Weapons.Select( giveGuns );
			pEnemy.Health = GetRandomNumber( 75, 150 );

			if ( isElite && GetRandomNumber( 100 ) > 75 )
			{
				pEnemy.Armor = GetRandomNumber( 25, 100 );
			}
			else
			{
				pEnemy.Armor = 0;
			}

			pEnemy.Money = GetRandomNumber( 10, 64 );
		}

		private void SetUpPedAlly( Ped pAlly)
		{
			if ( !Exists( pAlly ) ) return;

			blip = pAlly.AttachBlip();
			blip.Color = BlipColor.Cyan;
			blip.Name = "Ally";
			blip.Display = BlipDisplay.MapOnly;
			blip.Scale = 0.65F;

			World.SetGroupRelationship( RelationshipGroup.Player, Relationship.Respect, RelationshipGroup.Player );
			World.SetGroupRelationship( RelationshipGroup.Player, Relationship.Hate, relationshipGroupEnemies );
			World.SetGroupRelationship( relationshipGroupAllies, Relationship.Respect, relationshipGroupAllies );
			World.SetGroupRelationship( relationshipGroupAllies, Relationship.Hate, relationshipGroupEnemies );
			pAlly.CurrentRoom = Player.Character.CurrentRoom; // required, or ped won't be visible when spawned inside a building
			pAlly.WillDoDrivebys = true;
			pAlly.WantedByPolice = true;
			pAlly.PriorityTargetForEnemies = true;
			pAlly.DuckWhenAimedAtByGroupMember = true;
			pAlly.AlwaysDiesOnLowHealth = true;
			pAlly.SetPathfinding( true, true, true );

			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX", pAlly, 0, GetRandomNumber( 0, 6 ) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX", pAlly, 1, GetRandomNumber( 0, 6 ) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX_TEXTURE", pAlly, 0, GetRandomNumber( 0, 6 ) );
			GTA.Native.Function.Call( "SET_CHAR_PROP_INDEX_TEXTURE", pAlly, 1, GetRandomNumber( 0, 6 ) );

			pAlly.RelationshipGroup = relationshipGroupAllies;
			pAlly.ChangeRelationship( RelationshipGroup.Player, Relationship.Companion );
			spawnedAlly.Add( pAlly );

			pAlly.Task.ClearAllImmediately();
			pAlly.Task.AlwaysKeepTask = true;
			pAlly.Task.FightAgainstHatedTargets( 200.0F );
			
			string sWepName;
			sWepName = RandomChoice( weaponsAlly );

			Weapon giveGuns;

			if ( !Enum.TryParse( sWepName, true, out giveGuns ) )
			{
				throw new FormatException( "Weapon name " + sWepName + " does not exist!" );
			}

			pAlly.Weapons.FromType( giveGuns ).Ammo = 30000;
			pAlly.Weapons.Select( giveGuns );
			pAlly.Health = GetRandomNumber( 100, 200 );

			if ( GetRandomNumber( 100 ) > 75 )
			{
				pAlly.Armor = GetRandomNumber( 25, 100 );
			}
			else
			{
				pAlly.Armor = 0;
			}

			pAlly.Money = GetRandomNumber( 10, 32 );
		}
	}
}