// Models/Contact.cs
using System.Collections.Generic;

namespace Gardian.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public string Status { get; set; }
        // Путь к ресурсу внутри сборки (без pack://, он добавится в XAML через конвертер или Binding)
        public string AvatarResourceName { get; set; }
        public bool IsCallEnabled { get; set; }
        public bool HasVNDialogue { get; set; } // Можно ли начать VN-диалог
        public List<string> AllowedForUsers { get; set; } = new();
    }
}