using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DiskCardGame;
using UnityEngine;
using APIPlugin;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using InscryptionAPI.Ascension;
using BepInEx.Bootstrap;
using System.Net.Sockets;
using Infiniscryption.PackManagement;

namespace GarethMod
{
    /*
     * GLOBAL TODO:
     * Split pig behavior code off from sigil
     * Add AI behavior for Shove, Flighty
     * Test, Test, Test
     * Consider cat tribe
     * credit: divisionbyzorro/Infiniscryption for garethsmod pack_icon
     * Flighty shouldnt ask you to move if you win
     * Rework Killer bee flavor - it becomes a swarm after eating
     */

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public partial class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "gareth48.inscryption.garethmod";
        private const string PluginName = "Gareth's Mod";
        private const string PluginVersion = "2.0.0";
        private const string PluginPrefix = "Garethmod";
        private const string env = "GarethMod.dll";

        private const string LESHY_PLACEHOLDER = "Curious, I don't seem to remember this creature...";
        private const string LESHY_EXPLOIT_DESC = "How did this creature get here?";

        //Fires during boot sequence, dependent on mod launch order
        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            RegisterModElements();
        }

        //Fires post boot after all mods are registered, keep other mod compatibility here for this reason
        private void Start()
        {
            if (Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.packmanager"))
            {
                CreatePack();
            }
        }

        public static void CreatePack()
        {
            PackInfo garethsmodpack = PackManager.GetPackInfo(PluginPrefix);
            garethsmodpack.Title = "Gareth's Mod";
            garethsmodpack.SetTexture(TextureHelper.GetImageAsTexture("pack_icon.png"));
            garethsmodpack.Description = "A part 1 booster pack created by Gareth48 and illustrated by Plutraser. Adds [count] cards and 7 sigils designed to fit right into vanilla Inscryption.";
            garethsmodpack.ValidFor.Add(PackInfo.PackMetacategory.LeshyPack);
        }

        private void RegisterModElements()
        {
            //ID, internal name, display name, attack, health, descryption

            //This card has to come first since certain sigils directly create this card as a snippy debug response

            Texture2D GarethmodDecal = TextureHelper.GetImageAsTexture("garethmodwatermark.png");

            CardInfo Pig;
            CardInfo Gareth48 = CardManager.New(
                modPrefix: PluginPrefix,
                "Gareth48",
                "Sorry, can't do that",
                1,
                1,
                description: "Whats... he doing here?"
            ).SetCost(bloodCost: 0)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .AddDecal(GarethmodDecal)
            .SetPortrait("garethmod_gareth48.png")
            .SetEmissivePortrait("garethmod_gareth48_emission.png");
            CardManager.Add(PluginPrefix, Gareth48);


            //Sigil registration
            AddDrawAlly();
            AddTastyMorsel();
            AddIdentityTheft();
            AddShove();
            AddStandoffish();
            AddHungry();
            AddFlighty();

            //Begin Identity Theft Block
            CardInfo Skinwalker = CardManager.New(
                modPrefix: PluginPrefix,
                "Skinwalker",
                "Skinwalker",
                3,
                2,
                description: "The enigmatic skinwalker. It mimics the identity of the first creature it slays."
            ).SetCost(bloodCost: 2)
            .AddAbilities(Garethmod_IdentityTheft.ability)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .AddDecal(GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.Rare)
            .SetPortrait("garethmod_skinwalker.png")
            .SetEmissivePortrait("garethmod_skinwalker_emission.png")
            .SetPixelPortrait("garethmod_skinwalker_pixel.png");
            CardManager.Add(PluginPrefix, Skinwalker);


            //Begin Tasty Morsel Block
            CardInfo WoundedAnimal = CardManager.New(
                modPrefix: PluginPrefix,
                "WoundedAnimal",
                "Wounded Animal",
                0,
                1,
                description: "An easy target for hungry predators."
            ).SetCost(bonesCost: 3)
            .AddAbilities(Garethmod_TastyMorsel.ability)
            .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
            .AddAppearances(CardAppearanceBehaviour.Appearance.AlternatingBloodDecal)
            .AddDecal(GarethmodDecal)
            .SetPortrait("garethmod_woundedanimal.png")
            .SetEmissivePortrait("garethmod_woundedanimal_emission.png")
            .SetPixelPortrait("garethmod_woundedanimal_pixel.png");


            CardManager.Add(PluginPrefix, WoundedAnimal);


            CardInfo Lemming = CardManager.New(
                modPrefix: PluginPrefix,
                "Lemming",
                "Lemming",
                1,
                1,
                description: "The paranoid lemming. Fragile and a favorite treat among many animals."
            ).SetCost(bloodCost: 2)
            .AddAbilities(Garethmod_TastyMorsel.ability, Ability.Brittle)
            .AddDecal(GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
            .SetPortrait("garethmod_lemming.png")
            .SetEmissivePortrait("garethmod_lemming_emission.png")
             .SetPixelPortrait("garethmod_lemming_pixel.png");
            CardManager.Add(PluginPrefix, Lemming);


            //From this point forward my workflow was augmented by a formatting AI

            //Token: can only be obtained via evolution - Stage 2
            CardInfo Treant = CardManager.New(
                modPrefix: PluginPrefix,
                "Treant",
                "Treant",
                1,
                9,
                description: LESHY_EXPLOIT_DESC
            ).SetCost(bloodCost: 3)
            .AddAbilities(Ability.WhackAMole, Ability.Sharp)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .AddDecal(GarethmodDecal)
            .SetPortrait("garethmod_treant.png")
            .SetEmissivePortrait("garethmod_treant_emission.png")
            .SetPixelPortrait("garethmod_treant_pixel.png");
            CardManager.Add(PluginPrefix, Treant);


            //Token: can only be obtained via evolution - Stage 1
            CardInfo Snag = CardManager.New(
                modPrefix: PluginPrefix,
                "Snag",
                "Snag",
                0,
                5,
                description: LESHY_EXPLOIT_DESC
            ).SetCost(bloodCost: 2)
            .AddAbilities(Ability.Evolve, Ability.Sharp)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .AddDecal(GarethmodDecal)
            .SetPortrait("garethmod_snag.png")
            .SetEmissivePortrait("garethmod_snag_emission.png")
            .SetPixelPortrait("garethmod_snag_pixel.png")
            .SetEvolve(Treant, 1);
            CardManager.Add(PluginPrefix, Snag);


            //Rare low damage high health evolution tree - literally
            CardInfo Sapling = CardManager.New(
                modPrefix: PluginPrefix,
                "Sapling",
                "Sapling",
                0,
                3,
                description: "With time, even the smallest sapling becomes a massive, imposing obstacle."
            ).SetCost(bloodCost: 1)
            .AddAbilities(Ability.Evolve)
            .AddMetaCategories(CardMetaCategory.Rare)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .AddDecal(GarethmodDecal)
            .SetPortrait("garethmod_sapling.png")
            .SetEmissivePortrait("garethmod_sapling_emission.png")
            .SetPixelPortrait("garethmod_sapling_pixel.png")
            .SetEvolve(Snag, 1);
            CardManager.Add(PluginPrefix, Sapling);

            //Begin Allies Block
            //GPT guessed AddTribes
            CardInfo Oxpecker = CardManager.New(
                modPrefix: PluginPrefix,
                "Oxpecker",
                "Oxpecker",
                1,
                1,
                description: "The adored Oxpecker. A bird capable of befriending even the fiercest of foes."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_DrawAlly.ability, Ability.Flying)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .AddTribes(Tribe.Bird)
             .SetPortrait("garethmod_oxpecker.png")
             .SetEmissivePortrait("garethmod_oxpecker_emission.png")
             .SetPixelPortrait("garethmod_oxpecker_pixel.png");
            CardManager.Add(PluginPrefix, Oxpecker);


            CardInfo Elephant = CardManager.New(
                modPrefix: PluginPrefix,
                "Elephant",
                "Elephant",
                2,
                8,
                description: "The sturdy elephant. Friendly but not weak."
            ).SetCost(bloodCost: 3)
             .AddAbilities(Garethmod_TastyMorsel.ability, Ability.WhackAMole)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_elephant.png")
             .SetEmissivePortrait("garethmod_elephant_emission.png")
             .SetPixelPortrait("garethmod_elephant_pixel.png");
            CardManager.Add(PluginPrefix, Elephant);


            CardInfo Hyena = CardManager.New(
                modPrefix: PluginPrefix,
                "Hyena",
                "Hyena",
                1,
                2,
                description: "The cackling hyena. It always has an ally lurking in the shadows."
            ).SetCost(bonesCost: 3)
             .AddAbilities(Garethmod_DrawAlly.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .AddTribes(Tribe.Canine)
             .SetPortrait("garethmod_hyena.png")
             .SetEmissivePortrait("garethmod_hyena_emission.png")
             .SetPixelPortrait("garethmod_hyena_pixel.png");
            CardManager.Add(PluginPrefix, Hyena);

            //Standoffish Block

            CardInfo Panther = CardManager.New(
                modPrefix: PluginPrefix,
                "Panther",
                "Panther",
                2,
                3,
                description: "The aggressive panther. Never one to back down from a fight."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Standoffish.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_panther.png")
             .SetEmissivePortrait("garethmod_panther_emission.png")
             .SetPixelPortrait("garethmod_panther_pixel.png");
            CardManager.Add(PluginPrefix, Panther);

            CardInfo PantherCub = CardManager.New(
                modPrefix: PluginPrefix,
                "PantherCub",
                "Panther Cub",
                0,
                1,
                description: "The young panther. Even now its caution is unwavering"
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Standoffish.ability, Ability.Evolve)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_panthercub.png")
             .SetEmissivePortrait("garethmod_panthercub_emission.png")
             .SetPixelPortrait("garethmod_panthercub_pixel.png")
             .SetEvolve(Panther, 1);
            CardManager.Add(PluginPrefix, PantherCub);

            CardInfo TasmanianDevil = CardManager.New(
                modPrefix: PluginPrefix,
                "TasmanianDevil",
                "Tasmanian Devil",
                1,
                4,
                description: "The vicious Tasmanian devil. A fierce carnivore with a voracious appetite."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Standoffish.ability, Ability.GuardDog)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_tasmaniandevil.png")
             .SetEmissivePortrait("garethmod_tasmaniandevil_emission.png")
             .SetPixelPortrait("garethmod_tasmaniandevil_pixel.png");
            CardManager.Add(PluginPrefix, TasmanianDevil);


            //Vanilla card
            CardInfo Archerfish = CardManager.New(
                modPrefix: PluginPrefix,
                "Archerfish",
                "Archerfish",
                2,
                1,
                description: "The archerfish, an excellent marksman, known to shoot bugs out of the air with a blast of water."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Ability.Submerge, Ability.Sniper)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_archerfish.png")
             .SetEmissivePortrait("garethmod_archerfish_emission.png")
             .SetPixelPortrait("garethmod_archerfish_pixel.png");
            CardManager.Add(PluginPrefix, Archerfish);

            //Shove Block

            CardInfo HerculesBeetle = CardManager.New(
                modPrefix: PluginPrefix,
                "HerculesBeetle",
                "Hercules Beetle",
                0,
                4,
                description: "The forceful Hercules beetle. It can move creatures many times its size."
            ).SetCost(bloodCost: 1)
             .AddTribes(Tribe.Insect)
             .AddAbilities(Garethmod_Shove.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_herculesbeetle.png")
             .SetEmissivePortrait("garethmod_herculesbeetle_emission.png")
             .SetPixelPortrait("garethmod_herculesbeetle_pixel.png");
            CardManager.Add(PluginPrefix, HerculesBeetle);

            CardInfo Golem = CardManager.New(
                modPrefix: PluginPrefix,
                "Golem",
                "Golem",
                0,
                1,
                description: "An ancient golem. The magic that binds it is old but powerful. It will reform after death."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Shove.ability, Ability.DrawCopyOnDeath)
             .AddMetaCategories(CardMetaCategory.Rare)
             .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_golem.png")
             .SetEmissivePortrait("garethmod_golem_emission.png")
             .SetPixelPortrait("garethmod_golem_pixel.png");
            CardManager.Add(PluginPrefix, Golem);


            CardInfo Lion = CardManager.New(
                modPrefix: PluginPrefix,
                "Lion",
                "Lion",
                3,
                5,
                description: "The awe-inspiring lion. Sure to lead your creatures to victory."
            ).SetCost(bloodCost: 3)
             .AddAbilities(Ability.BuffNeighbours)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_lion.png")
             .SetEmissivePortrait("garethmod_lion_emission.png")
             .SetPixelPortrait("garethmod_lion_pixel.png");
            CardManager.Add(PluginPrefix, Lion);


            CardInfo YoungLion = CardManager.New(
                modPrefix: PluginPrefix,
                "YoungLion",
                "Juvenile Lion",
                1,
                3,
                description: LESHY_EXPLOIT_DESC
            ).SetCost(bloodCost: 2)
             .AddAbilities(Ability.Evolve)
             .AddDecal(GarethmodDecal)
             .SetEvolve(Lion, 1)
             //.SetEvolveId(new EvolveIdentifier("Garethmod_Lion", 1))
             .SetPortrait("garethmod_younglion.png")
             .SetEmissivePortrait("garethmod_younglion_emission.png")
             .SetPixelPortrait("garethmod_younglion_pixel.png");
            CardManager.Add(PluginPrefix, YoungLion);

            CardInfo LionCub = CardManager.New(
                modPrefix: PluginPrefix,
                "LionCub",
                "Lion Cub",
                1,
                1,
                description: "The princely lion cub. Given time it will become the king of its pride."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Ability.Evolve)
             .SetEvolve(YoungLion, 1)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_lioncub.png")
             .SetEmissivePortrait("garethmod_lioncub_emission.png")
             .SetPixelPortrait("garethmod_lioncub_pixel.png");
            CardManager.Add(PluginPrefix, LionCub);

            //Hungry
            CardInfo Piranha = CardManager.New(
                modPrefix: PluginPrefix,
                "Piranha",
                "Piranha",
                1,
                1,
                description: "The ferocious piranha. Schools can devour any living thing in seconds."
            ).SetCost(bonesCost: 3)
             .AddAbilities(Garethmod_Hungry.ability, Ability.Submerge)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_piranha.png")
             .SetEmissivePortrait("garethmod_piranha_emission.png")
             .SetPixelPortrait("garethmod_piranha_pixel.png");
            CardManager.Add(PluginPrefix, Piranha);

            CardInfo Leopard = CardManager.New(
                modPrefix: PluginPrefix,
                "Leopard",
                "Leopard",
                2,
                3,
                description: "The stealthy leopard. Invisible to its prey until it's too late."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Hungry.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_leopard.png")
             .SetEmissivePortrait("garethmod_leopard_emission.png")
             .SetPixelPortrait("garethmod_leopard_pixel.png");
            CardManager.Add(PluginPrefix, Leopard);

            CardInfo KillerBees = CardManager.New(
                modPrefix: PluginPrefix,
                "KillerBees",
                "Killer Bee Swarm",
                1,
                1,
                description: "The vicious killer bee. The more one fights the more will swarm."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Hungry.ability, Ability.BeesOnHit)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .AddTribes(Tribe.Insect)
             .SetPortrait("garethmod_killerbees.png")
             .SetEmissivePortrait("garethmod_killerbees_emission.png");
            CardManager.Add(PluginPrefix, KillerBees);


            //TODO: UNTEATHER PIG AND HUNGRY SIGIL CODE, SHOULD BE A CARD BEHAVIOR NOT IN BUILT
            Pig = CardManager.New(
                modPrefix: PluginPrefix,
                "Pig",
                "Pig",
                1,
                2,
                description: "The ravenous pig. Its hunger is insatiable, it will eat and eat until nothing remains."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Hungry.ability)
             .AddMetaCategories(CardMetaCategory.Rare)
             .AddTribes(Tribe.Hooved)
             .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_pig.png")
             .SetAltPortrait("garethmod_pig_alt.png")
             .SetPixelPortrait("garethmod_pig_pixel.png")
             .SetEmissivePortrait("garethmod_pig_emission.png");
            CardManager.Add(PluginPrefix, Pig);


            //Flighty Block

            CardInfo Badger = CardManager.New(
                modPrefix: PluginPrefix,
                "Badger",
                "Badger",
                2,
                3,
                description: "The bothersome badger. Quick to strike and retreat back into the shadows."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_badger.png")
             .SetEmissivePortrait("garethmod_badger_emission.png");
            CardManager.Add(PluginPrefix, Badger);

            CardInfo Bobcat = CardManager.New(
                modPrefix: PluginPrefix,
                "Bobcat",
                "Bobcat",
                1,
                2,
                description: "The wily bobcat. Unafraid to defend its territory but smart enough to know when to retreat."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Standoffish.ability, Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .SetPortrait("garethmod_bobcat.png")
             .SetEmissivePortrait("garethmod_bobcat_emission.png");
            CardManager.Add(PluginPrefix, Bobcat);

            CardInfo Dragonfly = CardManager.New(
                modPrefix: PluginPrefix,
                "Dragonfly",
                "Dragonfly",
                2,
                1,
                description: "The hasty dragonfly. Few can hit an insect this fast."
            ).SetCost(bonesCost: 6)
             .AddAbilities(Ability.Flying, Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddDecal(GarethmodDecal)
             .AddTribes(Tribe.Insect)
             .SetPortrait("garethmod_dragonfly.png")
             .SetEmissivePortrait("garethmod_dragonfly_emission.png");
            CardManager.Add(PluginPrefix, Dragonfly);

            /*

        private Texture2D generateTex(String fileName)
        {
            Texture2D tex = new(2, 2);
            tex.LoadImage(System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace(env, ""), "Artwork/" + fileName + ".png")));
            return tex;
        }

        private Texture2D loadTex(byte[] resourceFile)
        {
            Texture2D tex = new(2, 2);
            tex.LoadImage(resourceFile);
            tex.filterMode = FilterMode.Point;
            return tex;
        }
            */

            //Test decks down here since all cards are defined

            StarterDeckInfo garethskinwalkerdeck = ScriptableObject.CreateInstance<StarterDeckInfo>();
            garethskinwalkerdeck.title = "GarethSkinwalkerDeck";
            garethskinwalkerdeck.iconSprite = TextureHelper.GetImageAsSprite("garethmod_deck_skinwalker_icon.png", TextureHelper.SpriteType.StarterDeckIcon);
            garethskinwalkerdeck.cards = new() { Skinwalker, Skinwalker, };

            StarterDeckInfo garethevolvedeck = ScriptableObject.CreateInstance<StarterDeckInfo>();
            garethevolvedeck.title = "GarethEvolveDeck";
            garethevolvedeck.iconSprite = TextureHelper.GetImageAsSprite("garethmod_deck_evolve_icon.png", TextureHelper.SpriteType.StarterDeckIcon);
            garethevolvedeck.cards = new() { PantherCub, LionCub, Sapling };

            StarterDeckManager.Add(PluginGuid, garethskinwalkerdeck);
            StarterDeckManager.Add(PluginGuid, garethevolvedeck);
        }
    }
}