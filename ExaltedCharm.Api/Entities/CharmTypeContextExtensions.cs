using System;
using System.Collections.Generic;
using System.Linq;
using ExaltedCharm.Api.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExaltedCharm.Api.Entities
{
    public static class CharmTypeContextExtensions
    {
        public static void EnsureSeedDataForContext(this CharmContext context)
        {
            if (((RelationalDatabaseCreator)context.GetService<IDatabaseCreator>()).Exists())
            {
                if (context.CharmTypes.Any())
                {
                    return;
                }

                var exaltedTypes = new List<ExaltedType>()
                    {
                        new ExaltedType()
                        {
                            Name = "Solar",
                            Description = "Chosen of the Unconquered Sun",
                            CreatedDate = DateTime.Now,
                            NecromancyLimit = NecromancyLevels.Shadowlands.ToString(),
                            SorceryLimit = SorceryLevels.Solar.ToString()
                        }
                    };
                context.ExaltedTypes.AddRange(exaltedTypes);
                context.SaveChanges();

                var castes = new List<Caste>()
                {
                    new Caste()
                    {
                        Name = "Dawn",
                        Description = "Warriors of the unconquered sun",
                        CreatedDate = DateTime.Now,
                        ExaltedType = exaltedTypes.First()

                    },
                    new Caste()
                    {
                        Name = "Night",
                        Description = "Assasins, theives and spys of the unconquered sun",
                        CreatedDate = DateTime.Now,
                        ExaltedType = exaltedTypes.First()
                    },
                    new Caste()
                    {
                        Name = "Twilight",
                        Description = "Scholars and artisans of the unconquered sun",
                        CreatedDate = DateTime.Now,
                        ExaltedType = exaltedTypes.First()
                    },
                    new Caste()
                    {
                        Name = "Eclipse",
                        Description = "Diplomats of the unconquered sun",
                        CreatedDate = DateTime.Now,
                        ExaltedType = exaltedTypes.First()
                    },
                    new Caste()
                    {
                        Name = "Zenith",
                        Description = "Priest of the unconquered sun",
                        CreatedDate = DateTime.Now,
                        ExaltedType = exaltedTypes.First()
                    }
                };

                context.Castes.AddRange(castes);

                context.SaveChanges();

                var abilities = new List<Ability>()
                {
                    new Ability()
                    {
                        Name = "Archery",
                        Description = "Knowledge of ranged weapons",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Athletics",
                        Description = "Physical movement",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Awareness",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Brawl",
                        Description = "Unarmed combat",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Bureaucracy",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Craft",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Dodge",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Integrity",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Investigation",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Larcency",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Linguistics",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Lore",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Martial Arts",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Melee",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Medicine",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Occult",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Performance",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Presence",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Resistance",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Ride",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Sail",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Socialize",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Stealth",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Survival",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "Thrown",
                        Description = "",
                        CreatedDate = DateTime.Now
                    },
                    new Ability()
                    {
                        Name = "War",
                        Description = "",
                        CreatedDate = DateTime.Now
                    }
                };

                context.Abilities.AddRange(abilities);
                context.SaveChanges();

                var attibutes = new List<Attribute>()
                {
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Strength",
                        Type = AttributeType.Physical
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Dexterity",
                        Type = AttributeType.Physical
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Stamina",
                        Type = AttributeType.Physical
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Charisma",
                        Type = AttributeType.Social
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Manipulation",
                        Type = AttributeType.Social
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Appearance",
                        Type = AttributeType.Social
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Perception",
                        Type = AttributeType.Mental
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Intelligence",
                        Type = AttributeType.Mental
                    },
                    new Attribute()
                    {
                        CreatedDate = DateTime.Now,
                        Name = "Wits",
                        Type = AttributeType.Mental
                    }
                };

                context.Attributes.AddRange(attibutes);
                context.SaveChanges();

                var dawnCaste = castes.Single(x => x.Name == "Dawn");

                var dawnSkills = new List<CasteAbility>()
                {
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Archery"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Awareness"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Brawl"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Martial Arts"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Dodge"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Melee"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Resistance"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Thrown"),
                        Caste = dawnCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "War"),
                        Caste = dawnCaste
                    }
                };

                var zenithCaste = castes.Single(x => x.Name == "Zenith");
                var zenithSkills = new List<CasteAbility>()
                {
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Athletics"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Integrity"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Performance"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Lore"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Presence"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Resistance"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "Survival"),
                        Caste = zenithCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.SingleOrDefault(x => x.Name == "War"),
                        Caste = zenithCaste
                    },
                };

                var twilightCaste = castes.Single(x => x.Name == "Twilight");
                var twilightSkills = new List<CasteAbility>()
                {
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Bureaucracy"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Craft"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Integrity"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Investigation"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Linguistics"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Lore"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Medicine"),
                        Caste = twilightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Occult"),
                        Caste = twilightCaste
                    }
                };

                var nightCaste = castes.Single(x => x.Name == "Night");
                var nightSkills = new List<CasteAbility>()
                {
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Athletics"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Awareness"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Dodge"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Investigation"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Larcency"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Ride"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Stealth"),
                        Caste = nightCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Socialize"),
                        Caste = nightCaste
                    }
                };

                var eclipseCaste = castes.Single(x => x.Name == "Eclipse");
                var eclipseSkills = new List<CasteAbility>()
                {
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Bureaucracy"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Larcency"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Linguistics"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Occult"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Presence"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Ride"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Sail"),
                        Caste = eclipseCaste
                    },
                    new CasteAbility()
                    {
                        Ability = abilities.Single(x => x.Name == "Socialize"),
                        Caste = eclipseCaste
                    }
                };

                context.CasteAbilities.AddRange(dawnSkills);
                context.CasteAbilities.AddRange(zenithSkills);
                context.CasteAbilities.AddRange(twilightSkills);
                context.CasteAbilities.AddRange(nightSkills);
                context.CasteAbilities.AddRange(eclipseSkills);
                context.SaveChanges();

                var durations = new List<Duration>()
                    {
                        new Duration()
                        {
                            Name = "Instant",
                            Description = "Instantly take effect",
                            CreatedDate = DateTime.Now

                        },
                        new Duration()
                        {
                            Name = "One tick",
                            Description = "Last one combat tick",
                            CreatedDate = DateTime.Now
                        },
                        new Duration()
                        {
                            Name = "Indefinite",
                            Description = "Lasts until a given condition",
                            CreatedDate = DateTime.Now
                        },
                        new Duration()
                        {
                            Name = "One scene",
                            Description = "Lasts one scene",
                            CreatedDate = DateTime.Now
                        },
                        new Duration()
                        {
                            Name = "One action",
                            Description = "Last until the characters next action",
                            CreatedDate = DateTime.Now
                        }
                    };
                context.Durations.AddRange(durations);
                context.SaveChanges();

                var keywords = new List<Keyword>()
                    {
                        new Keyword()
                        {
                            Name = "Combo-Ok",
                            Description = "Can be used within a combo",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Obvious",
                            Description = "Obvious display of exalted power",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Knockback",
                            Description = "Causes the target to be moved",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Combo-Basic",
                            Description = "To be added",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Uniform",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Decisive-only",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Withering-only",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Dual",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Clash",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Perilous",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Counterattack",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Stackable",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Mute",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Psyche",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Salient",
                            Description = "",
                        },
                        new Keyword()
                        {
                            Name = "Bridge",
                            Description = "",
                        },
                        new Keyword()
                        {
                            Name = "Written-only",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Mastery",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Form",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Terrestrial",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Aggravated",
                            Description = "",
                            CreatedDate = DateTime.Now
                        },
                        new Keyword()
                        {
                            Name = "Apocryphal",
                            Description = "",
                            CreatedDate = DateTime.Now
                        }

                    };
                context.Keywords.AddRange(keywords);
                context.SaveChanges();
                var charmTypes = new List<CharmType>()
                    {
                        new CharmType()
                        {
                            Name = "Reflexive",
                            Description = "Can be used in response to an action",
                            CreatedDate = DateTime.Now,
                            Charms = new List<Charm>()
                            {
                                new Charm()
                                {
                                    Name = "War Lion Stance",
                                    Description =
                                        "The Solar Learns to defend others upon learing to defend herself. Like a pacing lion, she represents a formidable obstacle. The Exalt may take a reflexive defend other action to protect an ally within close range. This effect lasts a full scene, but only applies while the Solar and her charge are close to one another, and is cancelled if she moves out of close range. The Solar must drop commitment to this charm to defend a different person",
                                    CreatedDate = DateTime.Now,
                                    ExaltedType = exaltedTypes.Single(x => x.Name == "Solar"),
                                    Duration =  durations.Single(x => x.Name == "One scene")
                                },
                                new Charm()
                                {
                                    Name = "Bulward Stance",
                                    Description =
                                        "The Solar's mastery of defensive Essance flows guides her weapon to intercept all blows. Until her next turn, the Lawgiver ignores all penalties to her Parry Defense. The Chosen's definitive guard dampens her foes strikes. Any damage roll made against the solar takes a -1 penalty for each 1 rolled on the attack roll, up to a maximum of the Solar's Essence rating",
                                    CreatedDate = DateTime.Now,
                                    ExaltedType = exaltedTypes.Single(x => x.Name == "Solar"),
                                    Duration = durations.Single(x => x.Name == "One scene")
                                }
                            }
                        },
                        new CharmType()
                        {
                            Name = "Simple",
                            Description = "Improves an action on your turn",
                            CreatedDate = DateTime.Now,
                            Charms = new List<Charm>()
                            {
                                new Charm()
                                {
                                    Name = "Fivefold Bulwark Stance",
                                    Description =
                                        "Accepting no form of defeat, the Solar gazes along the edge of her blade and sees what it would see. THe ebb and flow of battle becomes clear to her, she sees the arcs of incomming attacks as glowing trails of Essence and moves with impossible, fluid speed to strike the path of all harm. For a full scene, the Exalt may ignore certain penalties to her Parry Defense and reduce the cost to use Bulwark Stance by two motes and Dipping Swallow Defense by one. In addition, when she uses Dipping Swallow Defense, it raises her Parry Defense by one.",
                                    CreatedDate = DateTime.Now,
                                    ExaltedType = exaltedTypes.Single(x => x.Name == "Solar"),
                                    Duration = durations.Single(x => x.Name == "One scene")
                                }
                            }
                        },
                        new CharmType()
                        {
                            Name = "Permanent",
                            Description = "Always active",
                            CreatedDate = DateTime.Now
                        },
                        new CharmType()
                        {
                            Name = "Supplemental",
                            Description = "Supplements a action",
                            CreatedDate = DateTime.Now,
                            Charms = new List<Charm>()
                            {
                                new Charm()
                                {
                                    Name = "There is no wind",
                                    Description =
                                        "The Solar’s heart knows the arrow’s path. She spends three motes and fires a single flawless shot, regardless of distance, visibility, weather and other prevailing conditions. This Charm nullifies all penalties, except wound and multiple action penalties, applying to a single Archery-based attack. If the Solar has Essence 3 or higher, she can spend two additional motes and this Charm will increase the Range of her weapon to her maximum visibility range.",
                                    CreatedDate = DateTime.Now,
                                    ExaltedType = exaltedTypes.Single(x => x.Name == "Solar"),
                                    Duration = durations.Single(x => x.Name == "Instant")
                                }
                            }
                        }
                    };
                context.CharmTypes.AddRange(charmTypes);
                context.SaveChanges();
                var keywordToCharm = new List<KeywordCharm>()
                    {
                        new KeywordCharm()
                        {
                            Charm = charmTypes.SelectMany(x => x.Charms).Single(x => x.Name == "There is no wind"),
                            Keyword = keywords.Single(x => x.Name == "Combo-Ok")
                        }
                    };
                context.KeywordCharms.AddRange(keywordToCharm);
                context.SaveChanges();
            }
        }
    }
}
