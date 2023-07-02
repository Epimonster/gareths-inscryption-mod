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

namespace GarethMod
{
    /*
     * GLOBAL TODO:
     * Split pig behavior code off from sigil
     * Add AI behavior for Shove, Flighty
     * Test, Test, Test
     * Consider cat tribe
     * 
     * 
     * 
     */

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public partial class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "gareth48.inscryption.garethmod";
        private const string PluginName = "Gareth's Mod";
        private const string PluginVersion = "2.0.0";
        private const string PluginPrefix = "GarethMod";
        private const string env = "GarethMod.dll";

        private string LESHY_PLACEHOLDER = "Curious, I don't seem to remember this card..."

        private void Awake()
        {
            Logger.LogInfo($"Loaded {PluginName}!");
            RegisterModElements();
        }


        private void RegisterModElements()
        {
            //ID, internal name, display name, attack, health, descryption

            //This card has to come first since certain sigils directly create this card as a snippy debug response
            CardInfo Gareth48 = CardManager.New(
                modPrefix: PluginPrefix,
                "Gareth48",
                "Sorry, can't do that",
                1,
                1, 
                description: "Whats... he doing here?"
            ).SetCost(bloodCost:0)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground, GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.Rare)
            .SetPortrait("garethmod_gareth48.png")
            .SetEmissivePortrait("garethmod_gareth48_emission.png");
            CardManager.Add(PluginPrefix, Gareth48);


            AddIdentityTheft();
            //Sigil registration
            AddDrawAlly();
            AddTastyMorsel();
            AddIdentityTheft();
            //AddShove();
            AddStandoffish();
            //AddHungry();
            AddFlighty();

            //Begin Identity Theft Block
            CardInfo Skinwalker = CardManager.New(
                modPrefix: PluginPrefix, 
                "Garethmod_Skinwalker", 
                "Skinwalker", 
                2, 
                1,
                description: "The enigmatic skinwalker. It mimics the identity of the first creature it slays."
            ).SetCost(bloodCost:2)
            .AddAbilities(Garethmod_IdentityTheft.ability)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground, GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.Rare)
            .SetPortrait("garethmod_skinwalker.png")
            .SetEmissivePortrait("garethmod_skinwalker_emission.png");
            CardManager.Add(PluginPrefix, Skinwalker);


            //Begin Tasty Morsel Block
            CardInfo WoundedAnimal = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_WoundedAnimal",
                "Wounded Animal",
                0,
                1,
                description: "An easy target for hungry predators."
            ).SetCost(bonesCost: 3)
            .AddAbilities(Garethmod_TastyMorsel.ability)
            .AddAppearances(CardAppearanceBehaviour.Appearance.AlternatingBloodDecal, GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
            .SetPortrait("garethmod_woundedanimal.png")
            .SetEmissivePortrait("garethmod_skinwalker_emission.png");
            CardManager.Add(PluginPrefix, WoundedAnimal);


            CardInfo Lemming = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Lemming",
                "Lemming",
                1,
                1,
                description: "The paranoid lemming. Fragile and a favorite treat among many animals."
            ).SetCost(bloodCost: 2)
            .AddAbilities(Garethmod_TastyMorsel.ability, Ability.Brittle)
            .AddAppearances(GarethmodDecal)
            .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
            .SetPortrait("garethmod_lemming.png")
            .SetEmissivePortrait("garethmod_lemming_emission.png");
            CardManager.Add(PluginPrefix, Lemming);


            //From this point forward my workflow was augmented by a formatting AI

            //Token: can only be obtained via evolution - Stage 2
            CardInfo Treant = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Treant",
                "Treant",
                1,
                9,
                description: ""
            ).SetCost(bloodCost: 3)
            .AddAbilities(Ability.WhackAMole, Ability.Sharp)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .SetPortrait("garethmod_treant.png")
            .SetEmissivePortrait("garethmod_treant_emission.png");
            CardManager.Add(PluginPrefix, Treant);


            //Token: can only be obtained via evolution - Stage 1
            CardInfo Snag = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Snag",
                "Snag",
                0,
                5,
                description: ""
            ).SetCost(bloodCost: 2)
            .AddAbilities(Ability.Evolve, Ability.Sharp)
            .AddMetaCategories(CardMetaCategory.Rare)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .SetPortrait("garethmod_snag.png")
            .SetEmissivePortrait("garethmod_snag_emission.png")
            .SetEvolve(Treant, 1);
            CardManager.Add(PluginPrefix, Snag);


            //Rare low damage high health evolution tree - literally
            CardInfo Sapling = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Sapling",
                "Sapling",
                0,
                3,
                description: "With time, even the smallest sapling becomes a massive, imposing obstacle."
            ).SetCost(bloodCost: 1)
            .AddAbilities(Ability.Evolve)
            .AddMetaCategories(CardMetaCategory.Rare)
            .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
            .SetPortrait("garethmod_sapling.png")
            .SetEmissivePortrait("garethmod_sapling_emission.png")
            .SetEvolve(Snag, 1);
            CardManager.Add(PluginPrefix, Sapling);

            //Begin Allies Block
            //GPT guessed AddTribes
            CardInfo Oxpecker = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Oxpecker",
                "Oxpecker",
                1,
                1,
                description: "The adored Oxpecker. A bird capable of befriending even the fiercest of foes."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_DrawAlly.ability, Ability.Flying)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddTribes(Tribe.Bird)
             .SetPortrait("garethmod_oxpecker.png")
             .SetEmissivePortrait("garethmod_oxpecker_emission.png");
            CardManager.Add(PluginPrefix, Oxpecker);


            CardInfo Elephant = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Elephant",
                "Elephant",
                2,
                8,
                description: "The sturdy elephant. Friendly but not weak."
            ).SetCost(bloodCost: 3)
             .AddAbilities(Garethmod_TastyMorsel.ability, Ability.WhackAMole)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_elephant.png")
             .SetEmissivePortrait("garethmod_elephant_emission.png");
            CardManager.Add(PluginPrefix, Elephant);


            CardInfo Hyena = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Hyena",
                "Hyena",
                1,
                2,
                description: "The cackling hyena. It always has an ally lurking in the shadows."
            ).SetCost(bonesCost: 3)
             .AddAbilities(Garethmod_DrawAlly.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddTribes(Tribe.Canine)
             .SetPortrait("garethmod_hyena.png")
             .SetEmissivePortrait("garethmod_hyena_emission.png");
            CardManager.Add(PluginPrefix, Hyena);

            //Standoffish Block

            CardInfo Panther = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Panther",
                "Panther",
                2,
                3,
                description: "The aggressive panther. Never one to back down from a fight."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Standoffish.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_panther.png")
             .SetEmissivePortrait("garethmod_panther_emission.png");
            CardManager.Add(PluginPrefix, Panther);

            CardInfo PantherCub = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_PantherCub",
                "Panther Cub",
                0,
                1,
                description: "The young panther. Even now its caution is unwavering"
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Standoffish.ability, Ability.Evolve)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_panthercub.png")
             .SetEmissivePortrait("garethmod_panthercub_emission.png")
            .SetEvolve(Panther, 1);
            CardManager.Add(PluginPrefix, PantherCub);

            CardInfo TasmanianDevil = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_TasmanianDevil",
                "Tasmanian Devil",
                1,
                4,
                description: "The vicious Tasmanian devil. A fierce carnivore with a voracious appetite."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Standoffish.ability, Ability.GuardDog)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_tasmaniandevil.png")
             .SetEmissivePortrait("garethmod_tasmaniandevil_emission.png");
            CardManager.Add(PluginPrefix, TasmanianDevil);


            //Vanilla card
            CardInfo Archerfish = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Archerfish",
                "Archerfish",
                2,
                1,
                description: "The archerfish, an excellent marksman known to shoot bugs out of the air with a blast of water."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Ability.Submerge, Ability.Sniper)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_archerfish.png")
             .SetEmissivePortrait("garethmod_archerfish_emission.png");
            CardManager.Add(PluginPrefix, Archerfish);

            //Shove Block

            CardInfo HerculesBeetle = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_HerculesBeetle",
                "Hercules Beetle",
                0,
                4,
                description: "The forceful Hercules beetle. It can move creatures many times its size."
            ).SetCost(bloodCost: 1)
             .AddTribes(Tribe.Insect)
             .AddAbilities(Garethmod_Shove.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_herculesbeetle.png")
             .SetEmissivePortrait("garethmod_herculesbeetle_emission.png");
            CardManager.Add(PluginPrefix, HerculesBeetle);

            CardInfo Golem = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Golem",
                "Golem",
                0,
                1,
                description: "An ancient golem. Strong, but fragile. The magic that binds it is old but powerful. It will reform after death."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Shove.ability, Ability.DrawCopyOnDeath)
             .AddMetaCategories(CardMetaCategory.Rare)
             .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
             .SetPortrait("garethmod_golem.png")
             .SetEmissivePortrait("garethmod_golem_emission.png");
            CardManager.Add(PluginPrefix, Golem);


            CardInfo Lion = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Lion",
                "Lion",
                3,
                5,
                description: "The awe-inspiring lion. Sure to lead your creatures to victory."
            ).SetCost(bloodCost: 3)
             .AddAbilities(Ability.BuffNeighbours)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_lion.png")
             .SetEmissivePortrait("garethmod_lion_emission.png");
            CardManager.Add(PluginPrefix, Lion);

            CardInfo YoungLion = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_YoungLion",
                "Juvenile Lion",
                1,
                3,
                description: ""
            ).SetCost(bloodCost: 2)
             .AddAbilities(Ability.Evolve)
             .SetEvolve(Lion, 1)
             //.SetEvolveId(new EvolveIdentifier("Garethmod_Lion", 1))
             .SetPortrait("garethmod_younglion.png")
             .SetEmissivePortrait("garethmod_younglion_emission.png");
            CardManager.Add(PluginPrefix, YoungLion);

            CardInfo LionCub = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_LionCub",
                "Lion Cub",
                1,
                1,
                description: "The princely lion cub. Given time it will become the king of its pride."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Ability.Evolve)
             .SetEvolve(YoungLion, 1)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_lioncub.png")
             .SetEmissivePortrait("garethmod_lioncub_emission.png");
            CardManager.Add(PluginPrefix, LionCub);

            //Hungry
            CardInfo Piranha = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Piranha",
                "Piranha",
                1,
                1,
                description: "The ferocious piranha. Schools can devour any living thing in seconds."
            ).SetCost(bonesCost: 3)
             .AddAbilities(Garethmod_Hungry.ability, Ability.Submerge)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_piranha.png")
             .SetEmissivePortrait("garethmod_piranha_emission.png");
            CardManager.Add(PluginPrefix, Piranha);

            CardInfo Leopard = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Leopard",
                "Leopard",
                2,
                3,
                description: "The stealthy leopard. Invisible to its prey until it's too late."
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Hungry.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_leopard.png")
             .SetEmissivePortrait("garethmod_leopard_emission.png");
            CardManager.Add(PluginPrefix, Leopard);

            CardInfo KillerBees = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_KillerBees",
                "Killer Bee Swarm",
                1,
                1,
                description: ""
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Hungry.ability, Ability.BeesOnHit)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddTribes(Tribe.Insect)
             .SetPortrait("garethmod_killerbees.png")
             .SetEmissivePortrait("garethmod_killerbees_emission.png");
            CardManager.Add(PluginPrefix, KillerBees);


            //TODO: UNTEATHER PIG AND HUNGRY SIGIL CODE, SHOULD BE A CARD BEHAVIOR NOT IN BUILT
            CardInfo Pig = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Pig",
                "Pig",
                1,
                2,
                description: "The ravenous pig. Its hunger is insatiable, it will eat and eat until nothing remains."
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Hungry.ability)
             .AddMetaCategories(CardMetaCategory.Rare)
             .AddTribes(Tribe.Hooved)
             .AddAppearances(CardAppearanceBehaviour.Appearance.RareCardBackground)
             .SetPortrait("garethmod_pig.png")
             .SetAltPortrait("garethmod_pig_alt.png")
             .SetEmissivePortrait("garethmod_pig_emission.png");
            CardManager.Add(PluginPrefix, Pig);


            //Flighty Block

            CardInfo Badger = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Badger",
                "Badger",
                2,
                3,
                description: LESHY_PLACEHOLDER
            ).SetCost(bloodCost: 2)
             .AddAbilities(Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_leopard.png")
             .SetEmissivePortrait("garethmod_leopard_emission.png");
            CardManager.Add(PluginPrefix, Badger);

            CardInfo Lynx = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Lynx",
                "Lynx",
                1,
                2,
                description: LESHY_PLACEHOLDER
            ).SetCost(bloodCost: 1)
             .AddAbilities(Garethmod_Standoffish.ability, Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .SetPortrait("garethmod_leopard.png")
             .SetEmissivePortrait("garethmod_leopard_emission.png");
            CardManager.Add(PluginPrefix, Lynx);

            CardInfo Dragonfly = CardManager.New(
                modPrefix: PluginPrefix,
                "Garethmod_Dragonfly",
                "Dragonfly",
                3,
                1,
                description: LESHY_PLACEHOLDER
            ).SetCost(bonesCost: 6)
             .AddAbilities(Ability.Flying, Garethmod_Flighty.ability)
             .AddMetaCategories(CardMetaCategory.ChoiceNode, CardMetaCategory.TraderOffer)
             .AddTribes(Tribe.Insect)
             .SetPortrait("garethmod_leopard.png")
             .SetEmissivePortrait("garethmod_leopard_emission.png");
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

            StarterDeckInfo garethtestdeck = ScriptableObject.CreateInstance<StarterDeckInfo>();
            garethtestdeck.title = "GarethDebugDeck";
            garethtestdeck.iconSprite = TextureHelper.GetImageAsSprite("starterdeck_icon_example.png", TextureHelper.SpriteType.StarterDeckIcon);
            garethtestdeck.cards = new() { Skinwalker, Skinwalker, Skinwalker };

            StarterDeckManager.Add(PluginGuid, garethtestdeck);
        }
    }
}