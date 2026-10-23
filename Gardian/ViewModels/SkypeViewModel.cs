using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gardian.Data;
using Gardian.Models;

namespace Gardian.ViewModels
{
    public class SkypeViewModel : INotifyPropertyChanged
    {
        private Contact _selectedContact;
        public ObservableCollection<Contact> VisibleContacts { get; set; }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                // При смене контакта обновляем шапку чата (аватар и имя)
                OnPropertyChanged(nameof(CurrentContactName));
                OnPropertyChanged(nameof(CurrentContactAvatar));
                OnPropertyChanged(nameof(CanStartVN));
            }
        }

        // Свойства для привязки шапки чата
        public string CurrentContactName => SelectedContact?.Name ?? "No Contact Selected";
        public string CurrentContactAvatar => SelectedContact?.AvatarResourceName;
        public bool CanStartVN => SelectedContact?.HasVNDialogue == true && SelectedContact.IsCallEnabled;

        public SkypeViewModel(string currentUsername)
        {
            VisibleContacts = new ObservableCollection<Contact>();
            LoadContactsForUser(currentUsername);
        }

        public void LoadContactsForUser(string username)
        {
            VisibleContacts.Clear();
            var allContacts = ContactDatabase.GetAllContacts();
            var filtered = allContacts.Where(c => c.AllowedForUsers.Contains(username));

            foreach (var contact in filtered)
            {
                VisibleContacts.Add(contact);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}