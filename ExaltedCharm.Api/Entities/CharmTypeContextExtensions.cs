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
                        },
                        new ExaltedType()
                        {
                            Name = "Abyssal",
                            Description = "Servants of the Deathlords",
                            CreatedDate = DateTime.Now,
                            NecromancyLimit = NecromancyLevels.Void.ToString(),
                            SorceryLimit = SorceryLevels.Celestial.ToString()
                        }
                    };
                context.ExaltedTypes.AddRange(exaltedTypes);
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
