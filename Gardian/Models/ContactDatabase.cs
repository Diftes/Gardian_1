// Data/ContactDatabase.cs
using System.Collections.Generic;
using System.Windows.Documents;
using Gardian.Models;

namespace Gardian.Data
{
    public static class ContactDatabase
    {
        public static List<Contact> GetAllContacts()
        {
            return new List<Contact>
            {
                new Contact {
                    Name = "Echo-bot", Status = "Online",
                    AvatarResourceName = "avatar_echo.png",
                    IsCallEnabled = false, HasVNDialogue = false,
                    AllowedForUsers = new List<string> { "retro_user" }
                },
                new Contact {
                    Name = "Icarus (AI, Deputy Director)", Status = "Online",
                    AvatarResourceName = "avatar_icarus.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "retro_user" }
                },
                // Контакты для другого пользователя (будут скрыты для retro_user)
                new Contact {
                    Name = "Gifted Garden (First Vice Director)", Status = "Online",
                    AvatarResourceName = "avatar_garden.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "admin_user" }
                },
                new Contact {
                    Name = "Graphite Ruby (Second Vice Director)", Status = "DND",
                    AvatarResourceName = "avatar_ruby.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "admin_user" }
                },
                new Contact {
                    Name = "Light Wine", Status = "Away",
                    AvatarResourceName = "avatar_admin.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "admin_user" }
                },
                new Contact {
                    Name = "Gardy", Status = "Offline",
                    AvatarResourceName = "avatar_admin.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "admin_user" }
                },
                new Contact {
                    Name = "Coldwire", Status = "Offline",
                    AvatarResourceName = "avatar_admin.png",
                    IsCallEnabled = true, HasVNDialogue = true,
                    AllowedForUsers = new List<string> { "admin_user" }
                }
            };
        }
    }
}